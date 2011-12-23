using System;
using Ankh.Config;
using EnvDTE;
using NSvn.Core;
using System.Collections;
using System.Windows.Forms;

namespace Ankh.Tests
{
    /// <summary>
    /// Summary description for ContextBase.
    /// </summary>
    public class ContextBase : IContext
    {        
        public ContextBase()
        {
            this.config = this.CreateConfig();
        }
        #region IContext Members

        public virtual void EndOperation()
        {
            // TODO:  Add ContextBase.EndOperation implementation
        }

        public virtual OutputPaneWriter OutputPane
        {
            get
            {
                if ( this.outputPane == null )
                    this.outputPane = new OutputPaneWriter( this.DTE, "Test" );
                return this.outputPane;
            }
        }

        public virtual System.Windows.Forms.IWin32Window HostWindow
        {
            get
            {
                // TODO:  Add ContextBase.HostWindow getter implementation
                return null;
            }
        }

        public virtual EnvDTE._DTE DTE
        {
            get
            {
                if ( this.dte == null )
                {
                    this.dte = DteFactory.Create2003().Create();
                }
                return this.dte;
            }
        }

        public virtual Ankh.RepositoryExplorer.Controller RepositoryExplorer
        {
            get
            {
                // TODO:  Add ContextBase.RepositoryExplorer getter implementation
                return null;
            }
        }

        public virtual void SolutionClosing()
        {
            // TODO:  Add ContextBase.SolutionClosing implementation
        }

        public virtual bool ReloadSolutionIfNecessary()
        {
            // TODO:  Add ContextBase.ReloadSolutionIfNecessary implementation
            return false;
        }

        public virtual Ankh.Config.Config Config
        {
            get
            {
                return this.config;
            }
        }

        public virtual ConflictManager ConflictManager
        {
            get
            {
                // TODO:  Add ContextBase.ConflictManager getter implementation
                return null;
            }
        }

        public virtual StatusCache StatusCache
        {
            get
            {
                if ( this.statusCache == null )
                    this.statusCache = new StatusCache( this.Client );
                return this.statusCache;
            }
        }

        public virtual IUIShell UIShell
        {
            get
            {
                if ( this.uiShell == null )
                    this.uiShell = new UIShellImpl();
                return this.uiShell;
            }
        }

        public virtual IErrorHandler ErrorHandler
        {
            get
            {
                if ( this.errorHandler == null )
                    this.errorHandler = new ErrorHandlerImpl();
                return this.errorHandler;
            }
        }

        public virtual FileWatcher ProjectFileWatcher
        {
            get
            {
                if ( this.projectFileWatcher == null )
                    this.projectFileWatcher = new FileWatcher( this.Client );
                return this.projectFileWatcher;
            }
        }

        public virtual void StartOperation(string description)
        {
            // TODO:  Add ContextBase.StartOperation implementation
        }

        public virtual void Shutdown()
        {
            // TODO:  Add ContextBase.Shutdown implementation
        }

        public virtual Ankh.Config.ConfigLoader ConfigLoader
        {
            get
            {
                // TODO:  Add ContextBase.ConfigLoader getter implementation
                return null;
            }
        }

        public virtual void SolutionOpened()
        {
            // TODO:  Add ContextBase.SolutionOpened implementation
        }

        public virtual SvnClient Client
        {
            get
            {
                if ( this.client == null )
                    this.client = new SvnClient( this );
                return this.client;
            }
        }

        public virtual bool OperationRunning
        {
            get
            {
                // TODO:  Add ContextBase.OperationRunning getter implementation
                return false;
            }
        }

        public virtual bool SolutionIsOpen
        {
            get
            {
                // TODO:  Add ContextBase.SolutionIsOpen getter implementation
                return false;
            }
        }

        public virtual EnvDTE.AddIn AddIn
        {
            get
            {
                // TODO:  Add ContextBase.AddIn getter implementation
                return null;
            }
        }

        public virtual ISolutionExplorer SolutionExplorer
        {
            get
            {
                if ( this.explorer == null )
                    this.explorer = new ExplorerImpl(this);
                return this.explorer;
            }
        }

        public event System.EventHandler Unloading;

        public virtual bool AnkhLoadedForSolution
        {
            get
            {
                // TODO:  Add ContextBase.AnkhLoadedForSolution getter implementation
                return false;
            }
        }

        #endregion

        public void CheckForException()
        {
            ContextBase.ErrorHandlerImpl handler = 
                (ContextBase.ErrorHandlerImpl)this.ErrorHandler;
            if ( handler.Exception != null )
                throw new Exception( "Exception thrown", handler.Exception );            
        }

        public class ErrorHandlerImpl : IErrorHandler
        {
            public Exception Exception;

            #region IErrorHandler Members

            public virtual void Handle(Exception ex)
            {
                this.Exception = ex;
            }

            #endregion
        }

        /// <summary>
        /// An ISynchronizeInvoke for which InvokeRequired will always return false.
        /// </summary>
        public class NoSynch : System.ComponentModel.ISynchronizeInvoke
        {
            #region ISynchronizeInvoke Members

            public object EndInvoke(IAsyncResult result)
            {
                // TODO:  Add NoSynch.EndInvoke implementation
                return null;
            }

            public object Invoke(Delegate method, object[] args)
            {
                // TODO:  Add NoSynch.Invoke implementation
                return null;
            }

            public bool InvokeRequired
            {
                get
                {
                    return false;
                }
            }

            public IAsyncResult BeginInvoke(Delegate method, object[] args)
            {
                // TODO:  Add NoSynch.BeginInvoke implementation
                return null;
            }

            #endregion

        }

        public class ExplorerImpl : ISolutionExplorer
        {
            public ExplorerImpl( IContext ctx )
            {
                this.context = ctx;
            }

            #region ISolutionExplorer Members

            public virtual System.Collections.IList GetItemResources(ProjectItem item, bool recursive)
            {
                ArrayList list = new ArrayList();
                for( short i = 1; i <= item.FileCount; i++ )
                {
                    string path = item.get_FileNames(i);
                    list.Add( new SvnItem( path, this.context.Client.SingleStatus(path) ) );
                }
                return list;
            }

            public virtual void Unload()
            {
                // TODO:  Add Explorer.Unload implementation
            }

            public virtual void VisitSelectedNodes(Ankh.Solution.INodeVisitor visitor)
            {
                // TODO:  Add Explorer.VisitSelectedNodes implementation
            }

            public virtual ProjectItem GetSelectedProjectItem()
            {
                // TODO:  Add Explorer.GetSelectedProjectItem implementation
                return null;
            }

            public virtual void Refresh(ProjectItem item)
            {
                // TODO:  Add Explorer.Refresh implementation
            }

            void Ankh.ISolutionExplorer.Refresh(Project project)
            {
                // TODO:  Add Explorer.Ankh.ISolutionExplorer.Refresh implementation
            }

            void Ankh.ISolutionExplorer.UpdateSelection()
            {
                // TODO:  Add Explorer.Ankh.ISolutionExplorer.UpdateSelection implementation
            }

            IList Ankh.ISolutionExplorer.GetSelectionFileNames()
            {
                // TODO:  Add Explorer.Ankh.ISolutionExplorer.GetSelectionFileNames implementation
                return null;
            }

            #endregion

            #region ISelectionContainer Members

            public virtual void RefreshSelectionParents()
            {
                // TODO:  Add Explorer.RefreshSelectionParents implementation
            }

            public virtual void StartBackgroundInitialization()
            {
                // TODO:  Add Explorer.StartBackgroundInitialization implementation
            }

            public virtual System.Collections.IList GetSelectionResources(bool getChildItems, Ankh.ResourceFilterCallback filter)
            {
                // TODO:  Add Explorer.GetSelectionResources implementation
                return null;
            }

            System.Collections.IList Ankh.ISelectionContainer.GetSelectionResources(bool getChildItems)
            {
                // TODO:  Add Explorer.Ankh.ISelectionContainer.GetSelectionResources implementation
                return null;
            }

            public virtual void RefreshSelection()
            {
                // TODO:  Add Explorer.RefreshSelection implementation
            }

            public virtual System.Collections.IList GetAllResources(Ankh.ResourceFilterCallback filter)
            {
                // TODO:  Add Explorer.GetAllResources implementation
                return null;
            }

            #endregion

            private IContext context;

        }


        public class UIShellImpl : IUIShell
        {
            #region IUIShell Members

            public virtual Ankh.UI.RepositoryExplorerControl RepositoryExplorer
            {
                get
                {
                    // TODO:  Add UIShellImpl.RepositoryExplorer getter implementation
                    return null;
                }
            }

            public virtual IContext Context
            {
                get
                {
                    // TODO:  Add UIShellImpl.Context getter implementation
                    return null;
                }
                set
                {
                    // TODO:  Add UIShellImpl.Context setter implementation
                }
            }

            public virtual void SetRepositoryExplorerSelection(object[] selection)
            {
                // TODO:  Add UIShellImpl.SetRepositoryExplorerSelection implementation
            }

            public virtual void ShowRepositoryExplorer(bool show)
            {
                // TODO:  Add UIShellImpl.ShowRepositoryExplorer implementation
            }

            public virtual DialogResult QueryWhetherAnkhShouldLoad()
            {
                // TODO:  Add UIShellImpl.QueryWhetherAnkhShouldLoad implementation
                return new DialogResult ();
            }

            public virtual DialogResult ShowMessageBox(string text, 
                string caption, MessageBoxButtons buttons, 
                MessageBoxIcon icon)
            {
                // TODO:  Add UIShellImpl.ShowMessageBox implementation
                return new DialogResult ();
            }

            public virtual System.Windows.Forms.DialogResult ShowMessageBox(string text, 
                string caption, System.Windows.Forms.MessageBoxButtons buttons)
            {
                // TODO:  Add UIShellImpl.Ankh.IUIShell.ShowMessageBox implementation
                return new System.Windows.Forms.DialogResult ();
            }

            #endregion
        }


        protected virtual Config.Config CreateConfig()
        {
            Config.Config config = new Config.Config();
            config.AutoAddNewFiles = true;
            config.AutoAddNewFiles = true;
            config.LogMessageTemplate = "";
            config.DisableSolutionReload = true;
            return config;
        }

        public FileWatcher projectFileWatcher;
        public Config.Config config;
        public _DTE dte;
        public SvnClient client;
        public StatusCache statusCache;
        public IErrorHandler errorHandler;
        public OutputPaneWriter outputPane;
        public ISolutionExplorer explorer;
        public IUIShell uiShell;
    }
}