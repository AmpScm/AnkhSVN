using System;
using System.Text;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;
using System.Reflection;
using NSvn.Core;
using Utils;

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

            this.LoadRoots();

            this.Context.Unloading += new EventHandler( Context_Unloading );

            this.control.WantNewRoot += new EventHandler( control_WantNewRoot );
            this.control.ValidatingNewRoot += new System.ComponentModel.CancelEventHandler( control_ValidatingNewRoot );
        }

        

        

        

        public IContext Context
        {
            get { return this.context; }
        }

        #region IWorkingCopyExplorer Members

        public void AddRoot( string directory )
        {
            FileSystemRootItem root = CreateRoot(directory);

            DoAddRoot(root);
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
                if ( PathUtils.GetName( item.Path ) != Client.AdminDirectoryName )
                {
                    items.Add( FileSystemItem.Create( this, item ) );
                }
            }

            foreach ( SvnItem item in this.statusCache.GetDeletions( directoryItem.Path ) )
            {
                items.Add( FileSystemItem.Create( this, item ) );
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

        void Context_Unloading( object sender, EventArgs e )
        {
            this.SaveRoots();
        }

        void control_ValidatingNewRoot( object sender, System.ComponentModel.CancelEventArgs e )
        {
            try
            {
                e.Cancel = !IsRootValid( this.control.NewRootPath );
            }
            catch ( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
            }
        }

        private bool IsRootValid( string path )
        {
            return Directory.Exists( path ) && SvnUtils.IsWorkingCopyPath( path );
        }

        void control_WantNewRoot( object sender, EventArgs e )
        {
            try
            {
                if ( this.IsRootValid( this.control.NewRootPath ) )
                {
                    this.AddRoot( this.control.NewRootPath );
                }
            }
            catch ( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
            }
        }

        private void SaveRoots()
        {
            string[] rootPaths = new string[ this.roots.Count ];
            for ( int i = 0; i < rootPaths.Length; i++ )
            {
                rootPaths[ i ] = ((FileSystemItem)this.roots[ i ]).SvnItem.Path;
            }
            this.Context.ConfigLoader.SaveWorkingCopyExplorerRoots( rootPaths );
        }

        private void LoadRoots()
        {
            System.Threading.Thread t = new System.Threading.Thread( new ThreadStart( this.DoLoadRoots ) );
            t.Start();
        }

        private delegate void DoAddRootDelegate(FileSystemRootItem rootItem);

        private void DoLoadRoots()
        {
            string[] rootPaths;
            try
            {
                rootPaths = this.Context.ConfigLoader.LoadWorkingCopyExplorerRoots();
                if ( rootPaths == null )
                {
                    return;
                }

                foreach ( string root in rootPaths )
                {
                    if ( Directory.Exists( root ) )
                    {
                        FileSystemRootItem rootItem = CreateRoot( root );
                        this.Context.UIShell.SynchronizingObject.Invoke(
                            new DoAddRootDelegate( this.DoAddRoot ), new object[] { rootItem } );
                    }
                }

            }
            catch ( TargetInvocationException ex )
            {
                this.Context.ErrorHandler.Handle( ex.InnerException );
            }
            catch ( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
            }
        }

        private void DoAddRoot( FileSystemRootItem root )
        {
            this.control.AddRoot( root );
            this.roots.Add( root );
        }

        private FileSystemRootItem CreateRoot( string directory )
        {
            this.statusCache.Status( directory );
            SvnItem item = this.statusCache[ directory ];
            FileSystemRootItem root = new FileSystemRootItem( this, item );
            return root;
        }


        private WorkingCopyExplorerControl control;
        private IContext context;
        private StatusCache statusCache;
        private ArrayList roots;
    }
}
