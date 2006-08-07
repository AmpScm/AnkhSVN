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
                    item.Refresh(false);
                }
            }
        }

        public void SyncAll()
        {
            foreach ( FileSystemItem item in this.roots )
            {
                item.Refresh( true );
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

        static void Main()
        {
            //IContext context = new Context();

            //WorkingCopyExplorer ex = new WorkingCopyExplorer( context );
            //ex.control = new WorkingCopyExplorerControl();

            //ex.AddRoot( @"D:\mydocs\vcsharp\ankh" );


            //using ( Form form = new Form() )
            //{
            //    form.Size = new System.Drawing.Size( 1100, 670 );
            //    form.Controls.Add( ex.control );
            //    ex.control.Dock = DockStyle.Fill;
            //    Application.Run( form );
            //}

        }

        //private class Context : IContext
        //{
        //    private StatusCache cache;

        //    public Context()
        //    {
        //        this.cache = new StatusCache( new SvnClient( this ) );
        //    }
        //    #region IContext Members

        //    public event EventHandler Unloading;

        //    public EnvDTE._DTE DTE
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public IUIShell UIShell
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public EnvDTE.AddIn AddIn
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public ISolutionExplorer SolutionExplorer
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public string SolutionDirectory
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public OutputPaneWriter OutputPane
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public SvnClient Client
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public Ankh.RepositoryExplorer.Controller RepositoryExplorer
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public bool SolutionIsOpen
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public bool AnkhLoadedForSolution
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public bool ReloadSolutionIfNecessary()
        //    {
        //        throw new Exception( "The method or operation is not implemented." );
        //    }

        //    public Ankh.Config.Config Config
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public IErrorHandler ErrorHandler
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public Ankh.Config.ConfigLoader ConfigLoader
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public StatusCache StatusCache
        //    {
        //        get { return this.cache; }
        //    }

        //    public bool OperationRunning
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public IWin32Window HostWindow
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public ConflictManager ConflictManager
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public FileWatcher ProjectFileWatcher
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public VSCommandBars CommandBars
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public Microsoft.VisualStudio.OLE.Interop.IServiceProvider ServiceProvider
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public bool EnableAnkhForLoadedSolution()
        //    {
        //        throw new Exception( "The method or operation is not implemented." );
        //    }

        //    public void SolutionClosing()
        //    {
        //        throw new Exception( "The method or operation is not implemented." );
        //    }

        //    public void StartOperation( string description )
        //    {
        //        throw new Exception( "The method or operation is not implemented." );
        //    }

        //    public void EndOperation()
        //    {
        //        throw new Exception( "The method or operation is not implemented." );
        //    }

        //    public void Shutdown()
        //    {
        //        throw new Exception( "The method or operation is not implemented." );
        //    }

        //    public IWorkingCopyExplorer WorkingCopyExplorer
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    public ISelectionContainer Selection
        //    {
        //        get { throw new Exception( "The method or operation is not implemented." ); }
        //    }

        //    #endregion
        //}

       
    }
}
