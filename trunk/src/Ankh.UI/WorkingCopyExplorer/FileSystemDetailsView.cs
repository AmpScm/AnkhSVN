using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using Utils.Win32;
using System.Runtime.InteropServices;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using Ankh.Scc;

namespace Ankh.UI.WorkingCopyExplorer
{
    class FileSystemDetailsView : ListViewWithSelection<FileSystemListViewItem>, IWorkingCopyExplorerSubControl
    {
        public event EventHandler CurrentDirectoryChanged;

        public FileSystemDetailsView()
        {
            this.View = View.Details;

            this.HideSelection = false;

            this.ListViewItemSorter = ListViewItemComparer.Instance;
        }

        IAnkhServiceProvider _context;
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set 
            {
                if(_context != value)
                {
                    _context = value;

                    OnContextChanged(EventArgs.Empty);
                }
            }
        }

        IFileIconMapper _mapper;
        
        IFileIconMapper Mapper
        {
            get { return _mapper ?? (_mapper = Context.GetService<IFileIconMapper>()); }
        }

        protected virtual void OnContextChanged(EventArgs e)
        {
            if(SmallImageList == null)
                SmallImageList = Mapper.ImageList;            
        }



        public void SetDirectory(IFileSystemItem directory)
        {
            // Initialize these the first time they are needed.
            if (this.textPropertyDescriptors == null)
            {
                this.InitializePropertyDescriptors(directory);
                this.InitializeCharacterWidth();
                this.InitializeColumns();
            }

            AddChildren(directory);
        }



        public IFileSystemItem CurrentDirectory
        {
            get { return this.currentDirectory; }
        }

        public System.Drawing.Point GetSelectionPoint()
        {
            if (this.SelectedItems.Count > 0)
            {
                ListViewItem item = this.SelectedItems[0];
                int offset = item.Bounds.Height / 3;
                return this.PointToScreen(new Point(item.Bounds.X + offset + this.StateImageList.ImageSize.Width,
                    item.Bounds.Y + offset));
            }
            else
            {
                return Point.Empty;
            }
        }


        public IFileSystemItem[] GetSelectedItems()
        {
            if (this.SelectedItems.Count == 1)
            {
                return new IFileSystemItem[] { (IFileSystemItem)this.SelectedItems[0].Tag };
            }
            else
            {
                ArrayList list = new ArrayList();
                foreach (ListViewItem item in this.SelectedItems)
                {
                    list.Add(item.Tag);
                }
                return (IFileSystemItem[])list.ToArray(typeof(IFileSystemItem));
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            ListViewHitTestInfo hti = this.HitTest(PointToClient(Control.MousePosition));
            if (hti.Item == null || hti.Location == ListViewHitTestLocations.None)
            {
                return;
            }

            // Double clicks either opens a folder or the item
            IFileSystemItem fileSystemItem = hti.Item.Tag as IFileSystemItem;
            if (fileSystemItem != null)
            {
                OpenItem(fileSystemItem);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Enter means open if there's only one selected item
            if (e.KeyCode == Keys.Enter && this.SelectedItems.Count == 1)
            {
                IFileSystemItem item = this.SelectedItems[0].Tag as IFileSystemItem;
                if (item != null)
                {
                    this.OpenItem(item);
                }
            }
        }

        private void OpenItem(IFileSystemItem item)
        {
            if (item.IsContainer)
            {
                this.SetDirectory(item);
                this.OnCurrentDirectoryChanged();
            }
            else
            {
                item.Open(Context);
            }
        }

        protected virtual void OnCurrentDirectoryChanged()
        {
            if (this.CurrentDirectoryChanged != null)
            {
                this.CurrentDirectoryChanged(this, EventArgs.Empty);
            }
        }


        private void AddChildren(IFileSystemItem directory)
        {
            this.UnhookEvents();

            this.currentDirectory = directory;

            this.currentDirectory.ItemChanged += new EventHandler<ItemChangedEventArgs>(this.item_Changed);

            this.BeginUpdate();
            try
            {
                this.Items.Clear();

                foreach (IFileSystemItem item in directory.GetChildren())
                {
                    FileSystemListViewItem lvi = new FileSystemListViewItem(item.SvnItem);

                    Items.Add(lvi);

                    FormatListViewItem(item, lvi);

                    lvi.Tag = item;

                    // we need to know when this item changes
                    item.ItemChanged += new EventHandler<ItemChangedEventArgs>(item_Changed);
                }

                this.Sort();
            }
            finally
            {
                this.EndUpdate();
            }
        }

        private void InitializePropertyDescriptors(IFileSystemItem directory)
        {
            PropertyDescriptorCollection unfilteredProperties = TypeDescriptor.GetProperties(directory);
            this.textPropertyDescriptors = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
            foreach (PropertyDescriptor pd in unfilteredProperties)
            {
                if (pd.Attributes[typeof(TextPropertyAttribute)] != null)
                {
                    this.textPropertyDescriptors.Add(pd);
                }
                if (pd.Attributes[typeof(StateImagePropertyAttribute)] != null)
                {
                    this.stateImagePropertyDescriptor = pd;
                }
            }

            this.textPropertyDescriptors = this.textPropertyDescriptors.Sort(PropertyOrderComparer.Instance);

        }

        private void InitializeColumns()
        {
            this.Columns.Clear();
            this.Columns.Add("Name", this.characterWidth * NameColumnNumberOfCharacters, HorizontalAlignment.Left);

            foreach (PropertyDescriptor pd in this.textPropertyDescriptors)
            {
                TextPropertyAttribute attr = pd.Attributes[typeof(TextPropertyAttribute)] as TextPropertyAttribute;
                if (attr == null)
                {
                    continue;
                }

                this.Columns.Add(attr.Text, attr.TextWidth * this.characterWidth, HorizontalAlignment.Right);
            }
        }

        private void InitializeCharacterWidth()
        {
            using (Graphics g = this.CreateGraphics())
            {
                string measureString = "Name of Something To Measure";
                this.characterWidth = (int)(g.MeasureString(measureString, this.Font).Width / measureString.Length);
            }
        }

        protected override void OnRetrieveSelection(ListViewWithSelection<FileSystemListViewItem>.RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new SvnItemData(Context, e.Item.SvnItem);
        }

        protected override void OnResolveItem(ListViewWithSelection<FileSystemListViewItem>.ResolveItemEventArgs e)
        {
            SvnItemData sid = e.SelectionItem as SvnItemData;

            if(sid == null)
                return;

            foreach (FileSystemListViewItem lvi in Items)
            {
                if (lvi.SvnItem == sid.SvnItem)
                    e.Item = lvi;
            }
        }

        private void UnhookEvents()
        {
            foreach (ListViewItem lvi in this.Items)
            {
                IFileSystemItem fileSystemItem = lvi.Tag as IFileSystemItem;
                if (fileSystemItem != null)
                {
                    fileSystemItem.ItemChanged -= new EventHandler<ItemChangedEventArgs>(this.item_Changed);
                }
            }

            if (this.currentDirectory != null)
            {
                this.currentDirectory.ItemChanged -= new EventHandler<ItemChangedEventArgs>(this.item_Changed);
            }
        }

        void item_Changed(object sender, ItemChangedEventArgs e)
        {
            IFileSystemItem fileSystemItem = sender as IFileSystemItem;
            if (fileSystemItem != null)
            {
                HandleItemChange(fileSystemItem, e.ItemChangedType);
            }
        }

        private void HandleItemChange(IFileSystemItem fileSystemItem, ItemChangedType changeType)
        {
            if (fileSystemItem == this.currentDirectory && changeType == ItemChangedType.ChildrenInvalidated)
            {
                this.InvalidateChildren();
            }
            else
            {
                ListViewItem lvi = this.FindListViewItemWithTag(fileSystemItem);
                if (lvi != null)
                {
                    // Reformat this listview item.
                    lvi.SubItems.Clear();
                    lvi.Text = fileSystemItem.Text;
                    this.FormatListViewItem(fileSystemItem, lvi);
                }
            }
        }

        private void InvalidateChildren()
        {
            this.AddChildren(this.currentDirectory);
        }

        protected override string GetCanonicalName(FileSystemListViewItem item)
        {
            if (item != null)
            {
                SvnItem i = item.SvnItem;

                string name = i.FullPath;

                if (i.IsDirectory && !name.EndsWith("\\"))
                    name += "\\"; // VS usualy ends returned folders with '\\'

                return name;
            }

            return base.GetCanonicalName(item);
        }

        private ListViewItem FindListViewItemWithTag(IFileSystemItem fileSystemItem)
        {
            foreach (ListViewItem lvi in this.Items)
            {
                if (Object.ReferenceEquals(lvi.Tag, fileSystemItem))
                {
                    return lvi;
                }
            }
            return null;
        }

        private void FormatListViewItem(IFileSystemItem item, ListViewItem lvi)
        {
            foreach (PropertyDescriptor pd in this.textPropertyDescriptors)
            {
                string value = pd.GetValue(item).ToString();
                lvi.SubItems.Add(value);
            }

            if (item.IsContainer)
            {
                lvi.ImageIndex = Mapper.DirectoryIcon;
            }
            else
            {
                lvi.ImageIndex = Mapper.GetIcon(item.SvnItem.FullPath);
            }

            if (this.StateImageList != null && this.stateImagePropertyDescriptor != null)
            {
                int index = (int)this.stateImagePropertyDescriptor.GetValue(item);
                lvi.StateImageIndex = index;
            }
        }


        private class PropertyOrderComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                PropertyDescriptor pd1 = (PropertyDescriptor)x;
                PropertyDescriptor pd2 = (PropertyDescriptor)y;

                TextPropertyAttribute attr1 = (TextPropertyAttribute)pd1.Attributes[typeof(TextPropertyAttribute)];
                TextPropertyAttribute attr2 = (TextPropertyAttribute)pd2.Attributes[typeof(TextPropertyAttribute)];

                return Comparer.Default.Compare(attr1.Order, attr2.Order);
            }
            public static readonly PropertyOrderComparer Instance = new PropertyOrderComparer();
        }

        private class ListViewItemComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                ListViewItem lvi1 = (ListViewItem)x;
                ListViewItem lvi2 = (ListViewItem)y;

                IFileSystemItem item1 = (IFileSystemItem)lvi1.Tag;
                IFileSystemItem item2 = (IFileSystemItem)lvi2.Tag;

                // If one of them is null (happens when a new item is added), just use the default comparer
                // which handles one of the items being null
                if ((item1 == null) ^ (item2 == null))
                {
                    return Comparer.Default.Compare(item1, item2);
                }

                // folders always first
                if (item1.IsContainer ^ item2.IsContainer)
                {
                    // one is a container, the other is not
                    // The minus is necessary to get folders first
                    return -Comparer.Default.Compare(item1.IsContainer, item2.IsContainer);
                }

                return Comparer.Default.Compare(item1.Text, item2.Text);
            }
            public static readonly ListViewItemComparer Instance = new ListViewItemComparer();
        }
        
        private PropertyDescriptorCollection textPropertyDescriptors = null;
        private PropertyDescriptor stateImagePropertyDescriptor = null;
        private int characterWidth;

        private IFileSystemItem currentDirectory;

        private const int NameColumnNumberOfCharacters = 50;
    }
}
