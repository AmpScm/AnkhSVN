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
                    ankhContext.OutputPane.StartActionText("Commit");
                    return dialog.LogMessage;
                }
                else
                    return null;
            }
        }

        protected override void NotifyCallback(NSvn.Core.Notification notification)
        {
            if (((string)actionStatus[notification.Action.ToString()]) != "ignoretext")
            {
                this.ankhContext.OutputPane.Write("{0} - {2}: {1}{3}"
                    ,actionStatus[notification.Action.ToString()] 
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
            actionStatus["Add"] =                   "ADDED";
            actionStatus["Copy"] =                  "COPIED";
            actionStatus["Delete"] =                "DELETED";
            actionStatus["Restore"] =               "RESTORED";
            actionStatus["Revert"] =                "REVERTED";
            actionStatus["FailedRevert"] =          "REVERT FAILED";
            actionStatus["Resolve"] =               "RESOLVED";
            actionStatus["Skip"] =                  "SKIPPED";
            actionStatus["UpdateDelete"] =          "UPDATE DELETED";
            actionStatus["UpdateAdd"] =             "UPDATE ADDED";
            actionStatus["UpdateUpdate"] =          "UPDATED";
            actionStatus["UpdateCompleted"] =       "ignoretext";
            actionStatus["UpdateExternal"] =        "UPDATED EXTERNAL";
            actionStatus["CommitModified"] =        "COMMIT MODIFIED";
            actionStatus["CommitAdded"] =           "COMMIT ADDED";
            actionStatus["CommitDeleted"] =         "COMMIT DELETED";
            actionStatus["CommitPostfixTxDelta"] =  "ignoretext";
        }
        
        static readonly Hashtable actionStatus = new Hashtable();
        private AnkhContext ankhContext;
        private static IDictionary map = new Hashtable();
    }
}
