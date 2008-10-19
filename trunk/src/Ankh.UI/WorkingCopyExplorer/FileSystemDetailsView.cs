using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using Ankh.Scc;
using System.IO;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.WorkingCopyExplorer
{
    class FileSystemDetailsView : ListViewWithSelection<FileSystemListViewItem>
    {
        public event EventHandler CurrentDirectoryChanged;

        public FileSystemDetailsView()
        {
            View = View.Details;
            HideSelection = false;
            FullRowSelect = true;
            AllowColumnReorder = true;
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

        bool _initialized;
        void TryInitialize()
        {
            if (!_initialized)
            {
                _initialized = true;
                InitializeCharacterWidth();
                InitializeColumns();
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (IsHandleCreated)
            {
                TryInitialize();
                
            }
        }

        IFileIconMapper _mapper;
        IStatusImageMapper _statusMapper;
        
        internal IFileIconMapper IconMapper
        {
            get { return _mapper ?? (_mapper = Context.GetService<IFileIconMapper>()); }
        }

        internal IStatusImageMapper StatusMapper
        {
            get { return _statusMapper ?? (_statusMapper = Context.GetService<IStatusImageMapper>()); }
        }        

        protected virtual void OnContextChanged(EventArgs e)
        {
            if(SmallImageList == null)
                SmallImageList = IconMapper.ImageList;            
        }

        public void SetDirectory(IFileSystemItem directory)
        {
            TryInitialize();

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
            this.currentDirectory = directory;

            this.BeginUpdate();
            try
            {
                this.Items.Clear();

                foreach (IFileSystemItem item in directory.GetChildren())
                {
                    SvnItem svnItem = item.SvnItem;
                    FileSystemInfo fif;
                    if(svnItem.IsDirectory)
                        fif = new DirectoryInfo(svnItem.FullPath);
                    else 
                        fif = new FileInfo(svnItem.FullPath);
                        
                    FileSystemListViewItem lvi = new FileSystemListViewItem(this, item.SvnItem, fif);

                    Items.Add(lvi);

                    lvi.Tag = item;

                    // we need to know when this item changes
                }
            }
            finally
            {
                this.EndUpdate();
            }
        }

        private void InitializeColumns()
        {
            AllColumns.Clear();
            SortColumns.Clear();
            this.Columns.Clear();
            SmartColumn name = new SmartColumn(this, "&Name", characterWidth * NameColumnNumberOfCharacters);
            SmartColumn modified = new SmartColumn(this, "&Modified", characterWidth * 20);
            SmartColumn extension = new SmartColumn(this, "&Type", characterWidth * 7);
            SmartColumn contStatus = new SmartColumn(this, "&Content Status", characterWidth * 15);
            SmartColumn propStatus = new SmartColumn(this, "&Property Status", characterWidth * 15);
            SmartColumn locked = new SmartColumn(this, "&Locked", characterWidth * 8);
            SmartColumn revision = new SmartColumn(this, "&Revision", characterWidth * 8);
            SmartColumn lastChangeTime = new SmartColumn(this, "Last C&hange", characterWidth * 20);
            SmartColumn lastRev = new SmartColumn(this, "Last Re&vision", characterWidth * 8);
            SmartColumn lastAuthor = new SmartColumn(this, "Last &Author", characterWidth * 8);
            SmartColumn isCopied = new SmartColumn(this, "C&opied", characterWidth * 6);
            SmartColumn isConficted = new SmartColumn(this, "Co&nfliced", characterWidth * 6);
            SmartColumn fullPath = new SmartColumn(this, "Fu&ll Path", characterWidth * 60);

            name.Sorter = new SortWrapper(
                delegate(FileSystemListViewItem x, FileSystemListViewItem y)
                {
                    if (x.IsDirectory ^ y.IsDirectory)
                        return x.IsDirectory ? -1 : 1;

                    return StringComparer.OrdinalIgnoreCase.Compare(x.Text, y.Text);
                });

            modified.Sorter = new SortWrapper(
                delegate(FileSystemListViewItem x, FileSystemListViewItem y)
                {
                    return x.Modified.CompareTo(y.Modified);
                });

            lastChangeTime.Sorter = new SortWrapper(
                delegate(FileSystemListViewItem x, FileSystemListViewItem y)
                {
                    return x.SvnItem.Status.LastChangeTime.CompareTo(y.SvnItem.Status.LastChangeTime);
                });

            AllColumns.Add(name);
            AllColumns.Add(modified);
            AllColumns.Add(extension);
            AllColumns.Add(contStatus);
            AllColumns.Add(propStatus);
            AllColumns.Add(locked);
            AllColumns.Add(revision);
            AllColumns.Add(lastChangeTime);
            AllColumns.Add(lastRev);
            AllColumns.Add(lastAuthor);
            AllColumns.Add(isCopied);
            AllColumns.Add(isConficted);
            AllColumns.Add(fullPath);

            Columns.AddRange(
                new ColumnHeader[]
                {
                    name,
                    modified,
                    extension,
                    contStatus,
                    propStatus,
                    locked,
                    revision
                });

            SortColumns.Add(name);
            FinalSortColumn = name;
            UpdateSortGlyphs();
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

        public override void OnShowContextMenu(MouseEventArgs e)
        {
            base.OnShowContextMenu(e);

            Point screen = (e.Location != new Point(-1, -1)) ? e.Location : PointToScreen(new Point(0, 0));

            IAnkhCommandService sc = Context.GetService<IAnkhCommandService>();

            Point p = PointToClient(e.Location);

            AnkhCommandMenu menu;
            if (p.Y < HeaderHeight)
            {
                Select(); // Must be the active control for the menu to work
                menu = AnkhCommandMenu.ListViewHeader;
            }
            else
                menu = AnkhCommandMenu.WorkingCopyExplorerContextMenu;

            sc.ShowContextMenu(menu, screen);
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

        void item_Changed(object sender, ItemChangedEventArgs e)
        {
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
        
        private int characterWidth;
        private IFileSystemItem currentDirectory;
        private const int NameColumnNumberOfCharacters = 50;
    }
}
