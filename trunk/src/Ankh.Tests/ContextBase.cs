using System;
using Ankh.Configuration;
using EnvDTE;
using NSvn.Core;
using System.Collections;
using System.Windows.Forms;
using System.IO;

using IServiceProvider = System.IServiceProvider;
using SharpSvn;
using Ankh.UI.Services;
using Ankh.UI;
using System.Diagnostics;

namespace Ankh.Tests
{
    /// <summary>
    /// Summary description for ContextBase.
    /// </summary>
    public class ContextBase : AnkhServiceContainer, IContext
    {        
        public ContextBase()
        {
            this.control = new Control();
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
                    this.outputPane = new OutputPaneWriter( this, "Test" );
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

        public virtual Ankh.RepositoryExplorer.RepositoryBrowserController RepositoryExplorerService
        {
            get
            {
                // TODO:  Add ContextBase.RepositoryExplorer getter implementation
                return null;
            }
        }

        public virtual string SolutionDirectory
        {
            get{ return null; }
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

        public virtual IUIShell UIShell
        {
            get
            {
                if ( this.uiShell == null )
                    this.uiShell = new UIShellImpl();
                return this.uiShell;
            }

            set
            {
                this.uiShell = value;
            }
        }

        public virtual IAnkhErrorHandler ErrorHandler
        {
            get
            {
                if ( this.errorHandler == null )
                    this.errorHandler = new ErrorHandlerImpl();
                return this.errorHandler;
            }
        }

        public virtual IDisposable StartOperation(string description)
        {
            // TODO:  Add ContextBase.StartOperation implementation
            return null;
        }

        public virtual void Shutdown()
        {
            // TODO:  Add ContextBase.Shutdown implementation
        }

        public virtual void EnableAnkhForLoadedSolution()
        {
            // TODO:  Add ContextBase.SolutionOpened implementation
        }

        public virtual SvnClient Client
        {
            get
            {
                if ( this.client == null )
                    this.client = new SvnClient();
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

        public class ErrorHandlerImpl : IAnkhErrorHandler
        {
            public Exception Exception;

            #region IErrorHandler Members

            public virtual void OnError(Exception ex)
            {
                this.Exception = ex;
            }

            public virtual void SendReport()
            {
                // empty
            }

            public void Write( string message, Exception ex, TextWriter writer )
            {
                // empty
            }

            #endregion

            #region IErrorHandler Members

            public void LogException( Exception exception, string message, params object[] args )
            {
                throw new Exception( "The method or operation is not implemented." );
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

        public class UIShellImpl : IUIShell
        {
            #region IUIShell Members

            public virtual Ankh.UI.RepositoryExplorer.RepositoryExplorerControl RepositoryExplorer
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
                   return this.context;
                }
                set
                {
                    this.context = value;
                }
            }

            public virtual System.ComponentModel.ISynchronizeInvoke SynchronizingObject
            {
                get{ return new NoSynch(); }
            }

            public virtual void SetRepositoryExplorerSelection(object[] selection)
            {
                // TODO:  Add UIShellImpl.SetRepositoryExplorerSelection implementation
            }

            public virtual void ShowRepositoryExplorer(bool show)
            {
                // TODO:  Add UIShellImpl.ShowRepositoryExplorer implementation
            }

            public virtual bool RepositoryExplorerHasFocus()
            {
                return false;
            }

            public virtual void ToggleCommitDialog( bool show )
            {
                // TODO: add implementation
            }

            public virtual void ResetCommitDialog()
            {
                // TODO: add implementation
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

            public virtual void DisplayHtml( string caption, string html, bool reuse )
            {
                // TODO: 
            }

            public virtual PathSelectorInfo ShowPathSelector( PathSelectorInfo info )
            {
                return null;
            }

            public virtual LockDialogInfo ShowLockDialog( LockDialogInfo info )
            {
                return null;
            }

            public virtual LogDialogInfo ShowLogDialog( LogDialogInfo info )
            {
                return null;
            }
            
            public virtual SwitchDialogInfo ShowSwitchDialog( SwitchDialogInfo info )
            {
                return null;
            }

            public virtual string ShowNewDirectoryDialog()
            {
                return null;
            }

            public virtual Uri ShowAddRepositoryRootDialog()
            {
                return null;
            }

            private IContext context;

            #endregion

            #region IUIShell Members


            public DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
            {
                return DialogResult.None;
            }

            #endregion

            #region IUIShell Members


            public Ankh.UI.WorkingCopyExplorerControl WorkingCopyExplorer
            {
                get { throw new Exception( "The method or operation is not implemented." ); }
            }

            public bool WorkingCopyExplorerHasFocus()
            {
                throw new Exception( "The method or operation is not implemented." );
            }

            public bool SolutionExplorerHasFocus()
            {
                throw new Exception( "The method or operation is not implemented." );
            }

            public string ShowAddWorkingCopyExplorerRootDialog()
            {
                throw new Exception( "The method or operation is not implemented." );
            }

            public void ShowWorkingCopyExplorer( bool p )
            {
                throw new Exception( "The method or operation is not implemented." );
            }

            #endregion

            #region IUIShell Members

            DialogResult IUIShell.ShowMessageBox(string text, string caption, MessageBoxButtons buttons)
            {
                throw new NotImplementedException();
            }

            DialogResult IUIShell.ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
            {
                throw new NotImplementedException();
            }

            DialogResult IUIShell.ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
            {
                throw new NotImplementedException();
            }

            void IUIShell.DisplayHtml(string caption, string html, bool reuse)
            {
                throw new NotImplementedException();
            }

            PathSelectorResult IUIShell.ShowPathSelector(PathSelectorInfo info)
            {
                throw new NotImplementedException();
            }

            #endregion
        }


       

        protected virtual void OnUnloading()
        {
            if ( this.Unloading != null )
            {
                this.Unloading( this, EventArgs.Empty );
            }

        }

        SvnClient client;
        IAnkhErrorHandler errorHandler;
        OutputPaneWriter outputPane;
        IUIShell uiShell;
        Control control;

        #region IContext Members
    

        public IWorkingCopyExplorer WorkingCopyExplorer
        {
            get { throw new Exception( "The method or operation is not implemented." ); }
        }

        public IAnkhSelectionContainer Selection
        {
            get { throw new Exception( "The method or operation is not implemented." ); }
        }

        public IAnkhPackage Package
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

		public Ankh.Selection.ISelectionContext SelectionContext
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public System.ComponentModel.ISynchronizeInvoke SynchronizingObject
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		#endregion        

        #region IContext Members


        public ISvnClientPool ClientPool
        {
            get { return GetService<ISvnClientPool>(); }
        }

        #endregion

        #region IContext Members


        public IAnkhConfigurationService Configuration
        {
            get { return GetService<IAnkhConfigurationService>(); }
        }

        #endregion
    }
}
