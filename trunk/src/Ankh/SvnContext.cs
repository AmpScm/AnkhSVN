// $Id$
using System;
using NSvn;
using NSvn.Common;
using NSvn.Core;
using Ankh.UI;
using System.Windows.Forms;
using System.Collections;
using EnvDTE;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace Ankh
{
    /// <summary>
    /// Summary description for SvnContext.
    /// </summary>
    internal class SvnContext : NSvnContext
    {
        public SvnContext( AnkhContext ankhContext )
        {

            //Clears the pane when opening new solutions.
            this.ankhContext = ankhContext;
            this.AddAuthenticationProvider( new DialogProvider( ankhContext.HostWindow ) );           
        }
        /// <summary>
        /// Invokes the LogMessage dialog.
        /// </summary>
        /// <param name="commitItems"></param>
        /// <returns></returns>
        
        public WorkingCopyResource[] ShowLogMessageDialog( WorkingCopyResource[] commitItems )
        {

            string templateText = this.GetTemplate();
            LogMessageTemplate template = new LogMessageTemplate( templateText );

            using( CommitDialog dialog = new CommitDialog() )
            {
                foreach( WorkingCopyResource item in commitItems )
                {
                    CommitAction action = CommitAction.None;
                    switch( item.Status.TextStatus )
                    {
                        case StatusKind.Added:
                            action = CommitAction.Added;
                            break;
                        case StatusKind.Deleted:
                            action = CommitAction.Deleted;
                            break;
                        case StatusKind.Modified:
                            action = CommitAction.Modified;
                            break;
                    }
                    if ( action != CommitAction.None )
                        dialog.AddCommitItem( action, item.Path, item );
                } // foreach

                dialog.LogMessageTemplate = template;

                // is there a previous log message?
                if ( this.logMessage != null )
                {
                    if ( MessageBox.Show( this.ankhContext.HostWindow, 
                        "The previous commit did not complete." + Environment.NewLine + 
                        "Do you want to reuse the log message?", 
                        "Previous log message", MessageBoxButtons.YesNo ) ==
                        DialogResult.Yes )

                        dialog.LogMessage = this.logMessage;
                }


                dialog.DiffWanted += new EventHandler( this.DiffWanted );
                if ( dialog.ShowDialog( this.ankhContext.HostWindow ) == DialogResult.OK )
                {
                    this.logMessage = dialog.LogMessage;
                    return (WorkingCopyResource[])dialog.GetSelectedTags( 
                        typeof(WorkingCopyResource) );
                }
                else
                {
                    this.logMessage = null;
                    return null;
                }
            }
        }

        /// <summary>
        /// To be called when a commit is finished, so the context can clean up.
        /// </summary>
        public void CommitCompleted()
        {
            this.logMessage = null;
        }
        
        protected override string LogMessageCallback(NSvn.Core.CommitItem[] commitItems)
        {
            return this.logMessage;
        }

        protected override void NotifyCallback(NSvn.Core.Notification notification)
        {
            if ( actionStatus[notification.Action] != null)
            {
                string nodeKind = "";
                if ( notification.NodeKind == NodeKind.File )
                    nodeKind = " file";
                else if ( notification.NodeKind == NodeKind.Directory )
                    nodeKind = " directory";

                this.ankhContext.OutputPane.WriteLine( "{0}{1}: {2}",
                    actionStatus[notification.Action],
                    nodeKind, 
                    notification.Path );
            }

            if (notification.Action == NotifyAction.CommitPostfixTxDelta)
                this.ankhContext.OutputPane.Write( '.' );

            if (notification.Action == NotifyAction.UpdateCompleted)
                this.ankhContext.OutputPane.WriteLine( "{0}Updated {1} to revision {2}.", 
                    Environment.NewLine, 
                    notification.Path, 
                    notification.RevisionNumber);

            // ensure the output pane gets updated 
            Application.DoEvents();
        }
        
        private string GetTemplate()
        {
            return @"# All lines starting with a # will be ignored
***# %path%";
        }

        private void DiffWanted( object sender, EventArgs args )
        {  
            DiffVisitor visitor = new DiffVisitor();

            this.ankhContext.SolutionExplorer.VisitSelectedItems( visitor, true );

            CommitDialog dialog = (CommitDialog)sender;
            dialog.Diff = visitor.Diff;    
        }

        #region DialogProvider
        private class DialogProvider : IAuthenticationProvider
        {
            public DialogProvider( IWin32Window hostWindow )
            {
                this.hostWindow = hostWindow;
            }
            

            #region Implementation of IAuthenticationProvider
            public NSvn.Common.ICredential NextCredentials( ICollection parameters )
            {              

                if ( loginDialog.ShowDialog( this.hostWindow ) == DialogResult.OK )
                    return this.lastCredential = new SimpleCredential( loginDialog.Username, 
                        loginDialog.Password );
                else
                    return this.lastCredential = null;
            }
            public NSvn.Common.ICredential FirstCredentials(  string realm, ICollection parameters )
            {
                if ( this.savedCredential != null )
                    return this.savedCredential;

                loginDialog.Realm = realm;

                if ( loginDialog.ShowDialog( this.hostWindow ) == DialogResult.OK )
                    return this.lastCredential = new SimpleCredential( loginDialog.Username, 
                        loginDialog.Password );
                else
                    return this.lastCredential = null;
            }

            public bool SaveCredentials( ICollection parameters )
            {
                this.savedCredential = this.lastCredential;
                return true;
            }
            public string Kind
            {
                get
                {
                    return SimpleCredential.AuthKind;
                }
            }
            #endregion

            private LoginDialog loginDialog = new LoginDialog();
            private ICredential lastCredential;
            private ICredential savedCredential;
            private IWin32Window hostWindow;
        }
        #endregion

        /// <summary>
        /// Pupulates actionStatus Hashtable.
        /// </summary>
        /// 
        static SvnContext()
        {
            actionStatus[NotifyAction.Add] =                    "Added";
            actionStatus[NotifyAction.Copy] =                   "Copied";
            actionStatus[NotifyAction.Delete] =                 "Deleted";
            actionStatus[NotifyAction.Restore] =                "Restored";
            actionStatus[NotifyAction.Revert] =                 "Reverted";
            actionStatus[NotifyAction.FailedRevert] =           "Revert failed";
            actionStatus[NotifyAction.Resolved] =                "Resolved";
            actionStatus[NotifyAction.Skip] =                   "Skipped";
            actionStatus[NotifyAction.UpdateDelete] =           "Deleted";
            actionStatus[NotifyAction.UpdateAdd] =              "Added";
            actionStatus[NotifyAction.UpdateUpdate] =           "Updated";
            actionStatus[NotifyAction.UpdateCompleted] =        null;
            actionStatus[NotifyAction.UpdateExternal] =         "Updated external";
            actionStatus[NotifyAction.CommitModified] =         "Modified";
            actionStatus[NotifyAction.CommitAdded] =            "Added";
            actionStatus[NotifyAction.CommitDeleted] =          "Deleted";
            actionStatus[NotifyAction.CommitReplaced] =         "Replaced";
            actionStatus[NotifyAction.CommitPostfixTxDelta] =   null;
        }
        
        static readonly Hashtable actionStatus = new Hashtable();
        private AnkhContext ankhContext;
        private static IDictionary map = new Hashtable();
        private string logMessage = null;
    }
}
