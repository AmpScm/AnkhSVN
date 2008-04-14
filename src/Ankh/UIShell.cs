// $Id$
using System;
using Ankh.UI;
using EnvDTE;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using Utils.Win32;

using SharpSvn;
using Ankh.ContextServices;
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;

namespace Ankh
{
    /// <summary>
    /// Summary description for UIShell.
    /// </summary>
    public class UIShell : AnkhService, IUIShell
    {
        public UIShell(IAnkhServiceProvider context)
            : base(context)
        {
            
        }

        #region IUIShell Members
        public RepositoryExplorerControl RepositoryExplorer
        {
            get
            {
                if (this.repositoryExplorerControl == null)
                    this.CreateRepositoryExplorer();
                return this.repositoryExplorerControl;
            }
            set
            {
                Debug.Assert(this.repositoryExplorerControl == null);
                this.repositoryExplorerControl = value;

                if (value != null)
                    GetService<IContext>().RepositoryExplorer.SetControl(value);
            }
        }

        public WorkingCopyExplorerControl WorkingCopyExplorer
        {
            get
            {
                return this.workingCopyExplorerControl;
            }
            set
            {
                Debug.Assert(this.workingCopyExplorerControl == null);
                this.workingCopyExplorerControl = value;

                if (value != null)
                    GetService<IContext>().WorkingCopyExplorer.SetControl(value);
            }
        }

        public CommitDialog CommitDialog
        {
            get { return this.commitDialog; }
            set
            {
                Debug.Assert(this.commitDialog == null);
                this.commitDialog = value;
            }
        }

        public System.ComponentModel.ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                // TODO: Fix someway; probably just removing
				return this.RepositoryExplorer ?? null;
            }
        }

        public void SetRepositoryExplorerSelection(object[] selection)
        {
            //this.repositoryExplorerWindow.SetSelectionContainer( ref selection );
        }

        public bool RepositoryExplorerHasFocus()
        {
            // The new command routing should make this method obsolete
            if (this.repositoryExplorerControl != null)
                return this.repositoryExplorerControl.ContainsFocus;
            else
                return false;
        }

        public bool WorkingCopyExplorerHasFocus()
        {
            // The new command routing should make this method obsolete
            if (this.workingCopyExplorerControl != null)
                return this.workingCopyExplorerControl.ContainsFocus;
            else
                return false;
        }

        [Obsolete]
        public bool SolutionExplorerHasFocus()
        {
            // The new command routing should make this method obsolete
            return false;// this.Context.DTE.ActiveWindow.Type == vsWindowType.vsWindowTypeSolutionExplorer;
        }



        /// <summary>
        /// Shows the commit dialog, blocking until the user hits cancel or commit.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        //public DialogResult ShowCommitDialogModal(CommitContext ctx)
        //{
            /*
            //if ( this.commitDialogWindow == null ) 
                this.CreateCommitDialog();
            Debug.Assert( this.commitDialog != null );

            this.commitDialog.UrlPaths = ctx.UrlPaths;
            this.commitDialog.CommitItems = ctx.CommitItems;
            this.commitDialog.LogMessageTemplate = ctx.LogMessageTemplate;
            this.commitDialog.KeepLocks = ctx.KeepLocks;

            this.commitDialog.LogMessage = ctx.LogMessage;

            // we want to preserve the original state.
            bool originalVisibility = this.commitDialogWindow.Visible;

            this.commitDialogWindow.Visible = true;
            this.commitDialogModal = true;

            this.EnsureWindowSize( this.commitDialogWindow );

            // Fired when user clicks Commit or Cancel.
            this.commitDialog.Proceed += new EventHandler( this.ProceedCommit );

            // we need the buttons enabled now, since it's pseudo-modal.
            this.commitDialog.ButtonsEnabled = true;
            this.commitDialog.Initialize();

            // run a message pump while the commit dialog's open
            Utils.Win32.Message msg;
            while ( this.commitDialogModal && this.commitDialogWindow.Visible ) 
            {
                if ( Win32.GetMessage( out msg, IntPtr.Zero, 0, 0 ))
                {
                    Win32.TranslateMessage( out msg );
                    Win32.DispatchMessage( out msg );                    
                }
            }

            ctx.LogMessage = this.commitDialog.LogMessage;
            ctx.RawLogMessage = this.commitDialog.RawLogMessage;
            ctx.CommitItems = this.commitDialog.CommitItems;
            ctx.KeepLocks = this.commitDialog.KeepLocks;

            ctx.Cancelled = this.commitDialog.CommitDialogResult == CommitDialogResult.Cancel;

            // restore the pre-modal state.
            this.commitDialog.ButtonsEnabled = false;
            this.commitDialogWindow.Visible = originalVisibility;
            this.commitDialog.CommitItems = new object[]{};

            return ctx;
             */
        //    return DialogResult.OK;
        //}

        /// <summary>
        /// Executes the worker.Work method while displaying a progress dialog.
        /// </summary>
        /// <param name="worker"></param>
        internal bool RunWithProgressDialog(IProgressWorker worker, string caption)
        {
            ProgressRunner runner = new ProgressRunner(this.Context, worker);
            runner.Start(caption);

            return !runner.Cancelled;
        }

        AnkhMessageBox _box;
        AnkhMessageBox MessageBox
        {
            get { return _box ?? (_box = new AnkhMessageBox(this)); }
        }

        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public DialogResult ShowMessageBox(string text, string caption,
            MessageBoxButtons buttons)
        {
            return MessageBox.Show(text, caption, buttons);
        }

        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public DialogResult ShowMessageBox(string text, string caption,
            MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public DialogResult ShowMessageBox(string text, string caption,
            MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton);
        }

        public void DisplayHtml(string caption, string html, bool reuse)
        {
            IAnkhWebBrowser browser = Context.GetService<IAnkhWebBrowser>();
            
            string htmlFile = Path.GetTempFileName();
            using (StreamWriter w = new StreamWriter(htmlFile, false, System.Text.Encoding.UTF8))
                w.Write(html);
            
            // have it show the html
            Uri url = new Uri("file://" + htmlFile);
            BrowserArgs args = new BrowserArgs();
            args.BaseCaption = caption;

            //if(reuse)
            // args.CreateFlags |= __VSCREATEWEBBROWSER.VSCWB_ReuseExisting;

            browser.Navigate(url, args);
        }

        public PathSelectorResult ShowPathSelector(PathSelectorInfo info)
        {
            IAnkhDialogOwner dlgOwner = GetService<IAnkhDialogOwner>();

            using (PathSelector selector = new PathSelector(info))
            {
                selector.Context = Context;



                bool succeeded = selector.ShowDialog(dlgOwner.DialogOwner) == DialogResult.OK;
                PathSelectorResult result = new PathSelectorResult(succeeded, selector.CheckedItems);
                result.Depth = selector.Recursive ? SvnDepth.Infinity : SvnDepth.Empty;
                result.RevisionStart = selector.RevisionStart;
                result.RevisionEnd = selector.RevisionEnd;
                return result;
            }
        }

        /// <summary>
        /// Shows the lock dialog.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public LockDialogInfo ShowLockDialog(LockDialogInfo info)
        {
            return null;
            //using (LockDialog dlg = new LockDialog())
            //{
            //    dlg.GetPathInfo += new EventHandler<ResolvingPathEventArgs>(GetPathInfo);

            //    dlg.Items = info.VisibleItems;
            //    dlg.CheckedFilter = info.CheckedFilter;
            //    dlg.Message = info.Message;
            //    dlg.StealLocks = info.StealLocks;
            //    if (dlg.ShowDialog(Context.GetService<IAnkhDialogOwner>().DialogOwner) != DialogResult.OK)
            //        return null;

            //    info.CheckedItems = new List<SvnItem>( dlg.CheckedItems);
            //    info.Message = dlg.Message;
            //    info.StealLocks = dlg.StealLocks;
            //    return info;
            //}
        }

        /// <summary>
        /// Shows the log dialog.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public LogDialogInfo ShowLogDialog(LogDialogInfo info)
        {
            return null;
            //using (LogDialog dlg = new LogDialog())
            //{
            //    dlg.EnableRecursive = false;
            //    dlg.Items = info.VisibleItems;
            //    dlg.CheckedFilter = info.CheckedFilter;
            //    dlg.Options = PathSelectorOptions.DisplayRevisionRange;
            //    dlg.RevisionStart = info.RevisionStart;
            //    dlg.RevisionEnd = info.RevisionEnd;
            //    dlg.GetPathInfo += new EventHandler<ResolvingPathEventArgs>(GetPathInfo);
            //    dlg.StopOnCopy = info.StopOnCopy;

            //    if (dlg.ShowDialog(Context.GetService<IAnkhDialogOwner>().DialogOwner) != DialogResult.OK)
            //        return null;

            //    info.CheckedItems = new List<SvnItem>(dlg.CheckedItems);
            //    info.StopOnCopy = dlg.StopOnCopy;
            //    info.RevisionStart = dlg.RevisionStart;
            //    info.RevisionEnd = dlg.RevisionEnd;

            //    return info;
            //}
        }

        public SwitchDialogInfo ShowSwitchDialog(SwitchDialogInfo info)
        {
            using (SwitchDialog dialog = new SwitchDialog())
            {
                dialog.Items = info.VisibleItems;
                dialog.SingleSelection = true;
				dialog.CheckedFilter = delegate { return true; };
                dialog.Options = PathSelectorOptions.DisplaySingleRevision;
                dialog.Recursive = true;

                if (dialog.ShowDialog(Context.GetService<IAnkhDialogOwner>().DialogOwner) != DialogResult.OK)
                    return null;

                info.SwitchToUrl = dialog.ToUrl;
                info.Depth = dialog.Recursive ? SvnDepth.Infinity : SvnDepth.Empty;
                info.Path = dialog.SelectedPath;
                info.RevisionStart = dialog.RevisionStart;

                return info;
            }
        }

        public string ShowNewDirectoryDialog()
        {
            using (NewDirectoryDialog dlg = new NewDirectoryDialog())
            {
                if (dlg.ShowDialog(Context.GetService<IAnkhDialogOwner>().DialogOwner) != DialogResult.OK)
                    return null;

                return dlg.DirectoryName;
            }
        }

        public RepositoryRootInfo ShowAddRepositoryRootDialog()
        {
            using (AddRepositoryRootDialog dlg = new AddRepositoryRootDialog())
            {
                if (dlg.ShowDialog(Context.GetService<IAnkhDialogOwner>().DialogOwner) != DialogResult.OK)
                    return null;

                return new RepositoryRootInfo(dlg.Url, dlg.Revision);
            }
        }

        public string ShowAddWorkingCopyExplorerRootDialog()
        {
            using (AddWorkingCopyExplorerRootDialog dlg = new AddWorkingCopyExplorerRootDialog())
            {
                if (dlg.ShowDialog(Context.GetService<IAnkhDialogOwner>().DialogOwner) != DialogResult.OK)
                {
                    return null;
                }
                return dlg.NewRoot;
            }
        }

        #endregion

        private void CreateRepositoryExplorer()
        {
            // BH: Moved creating to the package to allow VS to manage all state associated with the window
            Debug.WriteLine("Previously precreated Repository Explorer here");
        }

        private void CreateCommitDialog()
        {
            Debug.WriteLine("Previously precreated Commit Window here");
        }

        private void CreateWorkingCopyExplorer()
        {
            // BH: Moved creating to the package to allow VS to manage all state associated with the window
            Debug.WriteLine("Previously precreated Working Copy Explorer here");
        }

        private void ProceedCommit(object sender, EventArgs e)
        {
            this.commitDialog.Proceed -= new EventHandler(this.ProceedCommit);
        }

        private RepositoryExplorerControl repositoryExplorerControl;
        private WorkingCopyExplorerControl workingCopyExplorerControl;
        private CommitDialog commitDialog;
    }
}
