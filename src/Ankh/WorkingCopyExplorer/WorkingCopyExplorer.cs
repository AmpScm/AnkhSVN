using System;
using System.Text;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace Ankh.WorkingCopyExplorer
{
    class WorkingCopyExplorer : Ankh.IWorkingCopyExplorer
    {

        public WorkingCopyExplorer( IContext context )
        {
            this.context = context;
            this.control = context.UIShell.WorkingCopyExplorer;
            this.control.StateImages = StatusImages.StatusImageList;
            this.statusCache = this.context.StatusCache;

            this.roots = new ArrayList();
        }


        public IContext Context
        {
            get { return this.context; }
        }

        #region IWorkingCopyExplorer Members

        public void AddRoot( string directory )
        {
            this.statusCache.Status( directory );
            SvnItem item = this.statusCache[ directory ];
            FileSystemRootItem root = new FileSystemRootItem( this, item );

            this.control.AddRoot( root );
            this.roots.Add( root );
        }

        public void RemoveRoot( string directory )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        #region ISelectionContainer Members

        public void RefreshSelectionParents()
        {
            foreach ( IFileSystemItem selectedItem in this.control.GetSelectedItems() )
            {
                FileSystemItem item = selectedItem as FileSystemItem;
                if ( item != null && item.Parent != null )
                {
                    item.Parent.Refresh();
                }
            }
        }

        public void RefreshSelection()
        {
            foreach ( IFileSystemItem selectedItem in this.control.GetSelectedItems() )
            {
                FileSystemItem item = selectedItem as FileSystemItem;
                if ( item != null )
                {
                    item.Refresh();
                }
            }
        }

        public void SyncAll()
        {
            foreach ( FileSystemItem item in this.roots )
            {
                item.Refresh();
            }
        }

        public System.Collections.IList GetSelectionResources( bool getChildItems )
        {
            return this.GetSelectionResources( getChildItems, new ResourceFilterCallback( SvnItem.NoFilter ) );
        }

        public IList GetSelectionResources( bool getChildItems, ResourceFilterCallback filter )
        {
            ArrayList selectedResources = new ArrayList();
            foreach ( IFileSystemItem sel in this.control.GetSelectedItems() )
            {
                FileSystemItem fileSystemItem = sel as FileSystemItem;
                if ( fileSystemItem != null )
                {
                    fileSystemItem.GetResources( selectedResources, getChildItems, filter );
                }
            }
            return selectedResources;
        }

        public IList GetAllResources( ResourceFilterCallback filter )
        {
            ArrayList resources = new ArrayList();
            foreach ( FileSystemItem item in this.roots )
            {
                item.GetResources( resources, true, filter );
            }
            return resources;
        }

        #endregion

        public IContextMenu ContextMenu
        {
            get
            {
                return this.control.CustomContextMenu;
            }
            set
            {
                this.control.CustomContextMenu = value;
            }
        }

        internal IFileSystemItem[] GetFileSystemItemsForDirectory( SvnItem directoryItem )
        {
            ArrayList items = new ArrayList();
            foreach ( string path in Directory.GetFileSystemEntries( directoryItem.Path ) )
            {
                SvnItem item = this.statusCache[ path ];
                items.Add( FileSystemItem.Create(this, item) );
            }

            return (IFileSystemItem[])items.ToArray( typeof( IFileSystemItem ) );
        }

         internal void OpenItem( string path )
        {
            if ( File.Exists( path ) )
            {
                if ( path.ToLower().EndsWith( ".sln" ) )
                {
                    this.Context.DTE.Solution.Open( path );
                }
                else if ( path.ToLower().EndsWith( "proj" ) )
                {
                    this.Context.DTE.ExecuteCommand( "File.OpenProject", path );
                }
                else
                {
                    this.context.DTE.ItemOperations.OpenFile( path, EnvDTE.Constants.vsViewKindPrimary );
                }
            }
        }

        private WorkingCopyExplorerControl control;
        private IContext context;
        private StatusCache statusCache;
        private ArrayList roots;
    }
}
