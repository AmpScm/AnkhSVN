using System;

namespace Ankh.Tests
{
    /// <summary>
    /// Summary description for ContextBase.
    /// </summary>
    public class ContextBase : IContext
    {        
        #region IContext Members

        public void EndOperation()
        {
            // TODO:  Add ContextBase.EndOperation implementation
        }

        public OutputPaneWriter OutputPane
        {
            get
            {
                // TODO:  Add ContextBase.OutputPane getter implementation
                return null;
            }
        }

        public System.Windows.Forms.IWin32Window HostWindow
        {
            get
            {
                // TODO:  Add ContextBase.HostWindow getter implementation
                return null;
            }
        }

        public EnvDTE._DTE DTE
        {
            get
            {
                // TODO:  Add ContextBase.DTE getter implementation
                return null;
            }
        }

        public Ankh.RepositoryExplorer.Controller RepositoryExplorer
        {
            get
            {
                // TODO:  Add ContextBase.RepositoryExplorer getter implementation
                return null;
            }
        }

        public void SolutionClosing()
        {
            // TODO:  Add ContextBase.SolutionClosing implementation
        }

        public bool ReloadSolutionIfNecessary()
        {
            // TODO:  Add ContextBase.ReloadSolutionIfNecessary implementation
            return false;
        }

        public Ankh.Config.Config Config
        {
            get
            {
                // TODO:  Add ContextBase.Config getter implementation
                return null;
            }
        }

        public ConflictManager ConflictManager
        {
            get
            {
                // TODO:  Add ContextBase.ConflictManager getter implementation
                return null;
            }
        }

        public StatusCache StatusCache
        {
            get
            {
                // TODO:  Add ContextBase.StatusCache getter implementation
                return null;
            }
        }

        public IUIShell UIShell
        {
            get
            {
                // TODO:  Add ContextBase.UIShell getter implementation
                return null;
            }
        }

        public IErrorHandler ErrorHandler
        {
            get
            {
                // TODO:  Add ContextBase.ErrorHandler getter implementation
                return null;
            }
        }

        public FileWatcher ProjectFileWatcher
        {
            get
            {
                // TODO:  Add ContextBase.ProjectFileWatcher getter implementation
                return null;
            }
        }

        public void StartOperation(string description)
        {
            // TODO:  Add ContextBase.StartOperation implementation
        }

        public void Shutdown()
        {
            // TODO:  Add ContextBase.Shutdown implementation
        }

        public Ankh.Config.ConfigLoader ConfigLoader
        {
            get
            {
                // TODO:  Add ContextBase.ConfigLoader getter implementation
                return null;
            }
        }

        public void SolutionOpened()
        {
            // TODO:  Add ContextBase.SolutionOpened implementation
        }

        public SvnClient Client
        {
            get
            {
                // TODO:  Add ContextBase.Client getter implementation
                return null;
            }
        }

        public bool OperationRunning
        {
            get
            {
                // TODO:  Add ContextBase.OperationRunning getter implementation
                return false;
            }
        }

        public bool SolutionIsOpen
        {
            get
            {
                // TODO:  Add ContextBase.SolutionIsOpen getter implementation
                return false;
            }
        }

        public EnvDTE.AddIn AddIn
        {
            get
            {
                // TODO:  Add ContextBase.AddIn getter implementation
                return null;
            }
        }

        public Ankh.Solution.Explorer SolutionExplorer
        {
            get
            {
                // TODO:  Add ContextBase.SolutionExplorer getter implementation
                return null;
            }
        }

        public event System.EventHandler Unloading;

        public bool AnkhLoadedForSolution
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
