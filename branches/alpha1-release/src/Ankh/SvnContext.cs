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
            this.AddAuthenticationProvider( new DialogProvider() );           
        }
        /// <summary>
        /// Invokes the LogMessage dialog.
        /// </summary>
        /// <param name="commitItems"></param>
        /// <returns></returns>
        
        protected override string LogMessageCallback(NSvn.Core.CommitItem[] commitItems)
        {
            string templateText = this.GetTemplate();
            LogMessageTemplate template = new LogMessageTemplate( templateText );

            using( CommitDialog dialog = new CommitDialog( commitItems ) )
            {
                dialog.LogMessageTemplate = template;

                dialog.DiffWanted += new EventHandler( this.DiffWanted );
                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    ankhContext.OutputPane.StartActionText("Committing");
                    return dialog.LogMessage;
                }
                else
                    return null;
            }
        }

        protected override void NotifyCallback(NSvn.Core.Notification notification)
        {
            if (((string)actionStatus[notification.Action]) != null)
            {
                this.ankhContext.OutputPane.Write("{0} - {2}: {1}{3}"
                    ,actionStatus[notification.Action].ToString() 
                    ,notification.Path, notification.NodeKind.ToString()
                    ,Environment.NewLine );
            }
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
            

            #region Implementation of IAuthenticationProvider
            public NSvn.Common.ICredential NextCredentials( ICollection parameters )
            {               

                if ( loginDialog.ShowDialog() == DialogResult.OK )
                    return this.lastCredential = new SimpleCredential( loginDialog.Username, 
                        loginDialog.Password );
                else
                    return this.lastCredential = null;
            }
            public NSvn.Common.ICredential FirstCredentials( ICollection parameters )
            {
                if ( this.savedCredential != null )
                    return this.savedCredential;

                if ( loginDialog.ShowDialog() == DialogResult.OK )
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
            actionStatus[NotifyAction.Resolve] =                "Resolved";
            actionStatus[NotifyAction.Skip] =                   "Skipped";
            actionStatus[NotifyAction.UpdateDelete] =           "Update deleted";
            actionStatus[NotifyAction.UpdateAdd] =              "Update added";
            actionStatus[NotifyAction.UpdateUpdate] =           "Updated";
            actionStatus[NotifyAction.UpdateCompleted] =        null;
            actionStatus[NotifyAction.UpdateExternal] =         "Updated external";
            actionStatus[NotifyAction.CommitModified] =         "Commit modified";
            actionStatus[NotifyAction.CommitAdded] =            "Commit added";
            actionStatus[NotifyAction.CommitDeleted] =          "Commit deleted";
            actionStatus[NotifyAction.CommitReplaced] =         "Commit replaced";
            actionStatus[NotifyAction.CommitPostfixTxDelta] =   null;
        }
        
        static readonly Hashtable actionStatus = new Hashtable();
        private AnkhContext ankhContext;
        private static IDictionary map = new Hashtable();
    }
}
