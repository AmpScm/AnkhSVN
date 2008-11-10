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

        public void Write(string message, Exception ex, TextWriter writer)
        {
            // empty
        }

        #endregion

        #region IErrorHandler Members

        public void LogException(Exception exception, string message, params object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
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

        public virtual System.ComponentModel.ISynchronizeInvoke SynchronizingObject
        {
            get { return new NoSynch(); }
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

        public virtual void ToggleCommitDialog(bool show)
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
            return new DialogResult();
        }

        public virtual DialogResult ShowMessageBox(string text,
            string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon)
        {
            // TODO:  Add UIShellImpl.ShowMessageBox implementation
            return new DialogResult();
        }

        public virtual System.Windows.Forms.DialogResult ShowMessageBox(string text,
            string caption, System.Windows.Forms.MessageBoxButtons buttons)
        {
            // TODO:  Add UIShellImpl.Ankh.IUIShell.ShowMessageBox implementation
            return new System.Windows.Forms.DialogResult();
        }

        public virtual void DisplayHtml(string caption, string html, bool reuse)
        {
            // TODO: 
        }

        public virtual PathSelectorInfo ShowPathSelector(PathSelectorInfo info)
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

        #endregion

        #region IUIShell Members


        public DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return DialogResult.None;
        }

        #endregion

        #region IUIShell Members


        public Ankh.UI.WorkingCopyExplorer.WorkingCopyExplorerControl WorkingCopyExplorer
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool WorkingCopyExplorerHasFocus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool SolutionExplorerHasFocus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string ShowAddWorkingCopyExplorerRootDialog()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ShowWorkingCopyExplorer(bool p)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IUIShell Members

        void IUIShell.DisplayHtml(string caption, string html, bool reuse)
        {
            throw new NotImplementedException();
        }

        PathSelectorResult IUIShell.ShowPathSelector(PathSelectorInfo info)
        {
            throw new NotImplementedException();
        }

        bool IUIShell.EditEnlistmentState(Ankh.Scc.EnlistmentState state)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

}
