using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using Utils.Win32;
using System.Runtime.InteropServices;

namespace Ankh.UI
{
    public class FileSystemDetailsView : ListView
    {
        public event EventHandler CurrentDirectoryChanged;
        
        public FileSystemDetailsView()
        {
            this.View = View.Details;

            this.HideSelection = false;

            this.ListViewItemSorter = ListViewItemComparer.Instance;
        }

        public void SetDirectory( IFileSystemItem directory )
        {
            // Initialize these the first time they are needed.
            if ( this.textPropertyDescriptors == null )
            {
                this.GetSystemImageList();

                this.InitializePropertyDescriptors( directory );
                this.InitializeCharacterWidth();
                this.InitializeColumns();
            }

            AddChildren( directory );
        }

        

        public IFileSystemItem CurrentDirectory
        {
            get { return this.currentDirectory; }
        }


        internal IFileSystemItem[] GetSelectedItems()
        {
            if ( this.SelectedItems.Count == 1 )
            {
                return new IFileSystemItem[] { (IFileSystemItem)this.SelectedItems[ 0 ].Tag };
            }
            else
            {
                ArrayList list = new ArrayList();
                foreach ( ListViewItem item in this.SelectedItems )
                {
                    list.Add( item.Tag );
                }
                return (IFileSystemItem[])list.ToArray( typeof( IFileSystemItem ) );
            }
        }

        

        protected override void OnMouseDown( MouseEventArgs e )
        {
            // ensure right mouse clicks cause a selection
            if ( e.Button == MouseButtons.Right )
            {
                ListViewItem item = this.GetItemAt( e.X, e.Y );
                if ( item != null )
                {
                    item.Selected = true; 
                }
            }
            // Double clicks either opens a folder or the item
            else if ( e.Clicks == 2 && e.Button == MouseButtons.Left )
            {
                ListViewItem lvi = this.GetItemAt( e.X, e.Y );
                IFileSystemItem item = lvi.Tag as IFileSystemItem;
                if ( item != null )
                {
                    OpenItem( item );
                }
            }
            base.OnMouseDown( e );
        }

        protected override void OnKeyDown( KeyEventArgs e )
        {
            base.OnKeyDown( e );

            // Enter means open if there's only one selected item
            if ( e.KeyCode == Keys.Enter && this.SelectedItems.Count == 1)
            {
                IFileSystemItem item = this.SelectedItems[ 0 ].Tag as IFileSystemItem;
                if ( item != null )
                {
                    this.OpenItem( item );
                }
            }
        }

        private void OpenItem( IFileSystemItem item )
        {
            if ( item.IsContainer )
            {
                this.SetDirectory( item );
                this.OnCurrentDirectoryChanged();
            }
            else
            {
                item.Open();
            }
        }

        protected virtual void OnCurrentDirectoryChanged()
        {
            if ( this.CurrentDirectoryChanged != null )
            {
                this.CurrentDirectoryChanged( this, EventArgs.Empty );
            }
        }


        private void AddChildren( IFileSystemItem directory )
        {
            this.UnhookEvents();

            this.currentDirectory = directory;

            this.currentDirectory.ItemChanged += new ItemChangedEventHandler( this.item_Changed );

            this.BeginUpdate();
            try
            {
                this.Items.Clear();

                foreach ( IFileSystemItem item in directory.GetChildren() )
                {
                    ListViewItem lvi = this.Items.Add( item.Text );

                    FormatListViewItem( item, lvi );

                    lvi.Tag = item;

                    // we need to know when this item changes
                    item.ItemChanged += new ItemChangedEventHandler( item_Changed );
                }

                this.Sort();
            }
            finally
            {
                this.EndUpdate();
            }
        }

        private void InitializePropertyDescriptors( IFileSystemItem directory )
        {
            PropertyDescriptorCollection unfilteredProperties = TypeDescriptor.GetProperties( directory );
            this.textPropertyDescriptors = new PropertyDescriptorCollection( new PropertyDescriptor[] { } );
            foreach ( PropertyDescriptor pd in unfilteredProperties )
            {
                if ( pd.Attributes[ typeof( TextPropertyAttribute ) ] != null )
                {
                    this.textPropertyDescriptors.Add( pd );
                }
                if ( pd.Attributes[ typeof( StateImagePropertyAttribute ) ] != null )
                {
                    this.stateImagePropertyDescriptor = pd;
                }
            }

            this.textPropertyDescriptors.Sort( PropertyOrderComparer.Instance );
            
        }

        private void InitializeColumns()
        {
            this.Columns.Add("Name", this.characterWidth * NameColumnNumberOfCharacters, HorizontalAlignment.Left );

            foreach ( PropertyDescriptor pd in this.textPropertyDescriptors )
            {
                TextPropertyAttribute attr = pd.Attributes[ typeof( TextPropertyAttribute ) ] as TextPropertyAttribute;
                if ( attr == null )
                {
                    continue;
                }

                this.Columns.Add( attr.Text, attr.TextWidth * this.characterWidth, HorizontalAlignment.Right );
            }
        }

        private void InitializeCharacterWidth()
        {
            using ( Graphics g = this.CreateGraphics() )
            {
                string measureString = "Name of Something To Measure";
                this.characterWidth = (int)(g.MeasureString( measureString, this.Font ).Width / measureString.Length);
            }
        }

        private void UnhookEvents()
        {
            foreach ( ListViewItem lvi in this.Items )
            {
                IFileSystemItem fileSystemItem = lvi.Tag as IFileSystemItem;
                if ( fileSystemItem != null )
                {
                    fileSystemItem.ItemChanged -= new ItemChangedEventHandler( this.item_Changed );
                }
            }

            if ( this.currentDirectory != null )
            {
                this.currentDirectory.ItemChanged -= new ItemChangedEventHandler( this.item_Changed );
            }
        }

        void item_Changed( object sender, ItemChangedEventArgs e )
        {
            IFileSystemItem fileSystemItem = sender as IFileSystemItem;
            if ( fileSystemItem != null )
            {
                HandleItemChange( fileSystemItem, e.ItemChangedType );
            }
        }

        private void HandleItemChange( IFileSystemItem fileSystemItem, ItemChangedType changeType )
        {
            if ( fileSystemItem == this.currentDirectory && changeType == ItemChangedType.ChildrenInvalidated )
            {
                this.InvalidateChildren();
            }
            else
            {
                ListViewItem lvi = this.FindListViewItemWithTag( fileSystemItem );
                if ( lvi != null )
                {
                    // Reformat this listview item.
                    lvi.SubItems.Clear();
                    lvi.Text = fileSystemItem.Text;
                    this.FormatListViewItem( fileSystemItem, lvi );
                }
            }
        }

        private void InvalidateChildren()
        {
            this.AddChildren( this.currentDirectory );
        }

        private ListViewItem FindListViewItemWithTag( IFileSystemItem fileSystemItem )
        {
            foreach ( ListViewItem lvi in this.Items )
            {
                if (Object.ReferenceEquals(lvi.Tag, fileSystemItem))
                {
                    return lvi;
                }
            }
            return null;
        }

        private void FormatListViewItem( IFileSystemItem item, ListViewItem lvi )
        {
            foreach ( PropertyDescriptor pd in this.textPropertyDescriptors )
            {
                string value = pd.GetValue( item ).ToString();
                lvi.SubItems.Add( value );
            }

            if ( item.IsContainer )
            {
                lvi.ImageIndex = this.closedFolderIconIndex;
            }
            else
            {
                lvi.ImageIndex = this.fileIconIndex;
            }

            if ( this.StateImageList != null && this.stateImagePropertyDescriptor != null )
            {
                int index = (int)this.stateImagePropertyDescriptor.GetValue( item );
                lvi.StateImageIndex = index;
            }
        }


        private class PropertyOrderComparer : IComparer
        {
            public int Compare( object x, object y )
            {
                PropertyDescriptor pd1 = (PropertyDescriptor)x;
                PropertyDescriptor pd2 = (PropertyDescriptor)y;

                TextPropertyAttribute attr1 = (TextPropertyAttribute)pd1.Attributes[ typeof( TextPropertyAttribute ) ];
                TextPropertyAttribute attr2 = (TextPropertyAttribute)pd2.Attributes[ typeof( TextPropertyAttribute ) ];

                return Comparer.Default.Compare( attr1.Order, attr2.Order );
            }
            public static readonly PropertyOrderComparer Instance = new PropertyOrderComparer();
        }

        private class ListViewItemComparer : IComparer
        {
            public int Compare( object x, object y )
            {
                ListViewItem lvi1 = (ListViewItem)x;
                ListViewItem lvi2 = (ListViewItem)y;

                IFileSystemItem item1 = (IFileSystemItem)lvi1.Tag;
                IFileSystemItem item2 = (IFileSystemItem)lvi2.Tag;

                // If one of them is null (happens when a new item is added), just use the default comparer
                // which handles one of the items being null
                if ( ( item1 == null ) ^ ( item2 == null ) )
                {
                    return Comparer.Default.Compare( item1, item2 );
                }

                // folders always first
                if ( item1.IsContainer ^ item2.IsContainer )
                {
                    // one is a container, the other is not
                    // The minus is necessary to get folders first
                    return -Comparer.Default.Compare( item1.IsContainer, item2.IsContainer );
                }

                return Comparer.Default.Compare( item1.Text, item2.Text );
            }
            public static readonly ListViewItemComparer Instance = new ListViewItemComparer();
        }

        /// <summary>
        /// Retrieve the system image list and assign it to this listview.
        /// </summary>
        private void GetSystemImageList()
        {
            // get the system image list
            SHFILEINFO fileinfo = new SHFILEINFO(); ;
            IntPtr sysImageList = Win32.SHGetFileInfo( "", 0, ref fileinfo,
                (uint)Marshal.SizeOf( fileinfo ), Constants.SHGFI_SHELLICONSIZE |
                Constants.SHGFI_SYSICONINDEX | Constants.SHGFI_SMALLICON );

            // assign it to this listview
            Win32.SendMessage( this.Handle, Msg.LVM_SETIMAGELIST, Constants.LVSIL_SMALL,
                sysImageList );

            // get the closed folder icon
            Win32.SHGetFileInfo( "", Constants.FILE_ATTRIBUTE_DIRECTORY, ref fileinfo,
                (uint)Marshal.SizeOf( fileinfo ), Constants.SHGFI_SHELLICONSIZE |
                Constants.SHGFI_SYSICONINDEX | Constants.SHGFI_SMALLICON |
                Constants.SHGFI_USEFILEATTRIBUTES );
            this.closedFolderIconIndex = fileinfo.iIcon.ToInt32();

            // get the icon for a file
            Win32.SHGetFileInfo( "file.txt", 0, ref fileinfo,
                (uint)Marshal.SizeOf( fileinfo ), Constants.SHGFI_SHELLICONSIZE |
                Constants.SHGFI_SYSICONINDEX | Constants.SHGFI_SMALLICON |
                Constants.SHGFI_USEFILEATTRIBUTES );
            this.fileIconIndex = fileinfo.iIcon.ToInt32();
        }    

        

        private PropertyDescriptorCollection textPropertyDescriptors = null;
        private PropertyDescriptor stateImagePropertyDescriptor = null;
        private int characterWidth;

        private int closedFolderIconIndex;
        private int fileIconIndex;
        private IFileSystemItem currentDirectory;

        private const int NameColumnNumberOfCharacters = 50;

       
    }
}
