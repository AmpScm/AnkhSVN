using System;

namespace Ankh.Tests
{
    /// <summary>
    /// Summary description for ContextBase.
    /// </summary>
    public class ContextBase : IContext
    {        
        #region IContext Members

        public virtual void EndOperation()
        {
            // TODO:  Add ContextBase.EndOperation implementation
        }

        public virtual OutputPaneWriter OutputPane
        {
            get
            {
                // TODO:  Add ContextBase.OutputPane getter implementation
                return null;
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
                // TODO:  Add ContextBase.DTE getter implementation
                return null;
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
                // TODO:  Add ContextBase.Config getter implementation
                return null;
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
                // TODO:  Add ContextBase.StatusCache getter implementation
                return null;
            }
        }

        public virtual IUIShell UIShell
        {
            get
            {
                // TODO:  Add ContextBase.UIShell getter implementation
                return null;
            }
        }

        public virtual IErrorHandler ErrorHandler
        {
            get
            {
                // TODO:  Add ContextBase.ErrorHandler getter implementation
                return null;
            }
        }

        public virtual FileWatcher ProjectFileWatcher
        {
            get
            {
                // TODO:  Add ContextBase.ProjectFileWatcher getter implementation
                return null;
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
                // TODO:  Add ContextBase.Client getter implementation
                return null;
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

        public virtual Ankh.Solution.Explorer SolutionExplorer
        {
            get
            {
                // TODO:  Add ContextBase.SolutionExplorer getter implementation
                return null;
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
    }
}
