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
        public SvnContext( AnkhContext ankhContext ) : base( @"T:\foo123config" )
        {

            //Clears the pane when opening new solutions.
            this.ankhContext = ankhContext;
            this.AddAuthenticationProvider( AuthenticationProvider.GetSimpleProvider() );           
            this.AddAuthenticationProvider( AuthenticationProvider.GetSimplePromptProvider(
                new SimplePromptDelegate( this.PasswordPrompt ), 3 ) );
            this.AddAuthenticationProvider( AuthenticationProvider.GetSslServerTrustFileProvider() );
            this.AddAuthenticationProvider( AuthenticationProvider.GetSslServerTrustPromptProvider(
                new SslServerTrustPromptDelegate( this.SslServerTrustPrompt ) ) );
            this.AddAuthenticationProvider( 
                AuthenticationProvider.GetSslClientCertPasswordFileProvider() );
            this.AddAuthenticationProvider( 
                AuthenticationProvider.GetSslClientCertPasswordPromptProvider(
                    new SslClientCertPasswordPromptDelegate( 
                        this.ClientCertificatePasswordPrompt ) ) );


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
        
        protected override string LogMessageCallback(NSvn.Core.CommitItem[] commitItems)
        {
            return this.logMessage;
        }

        protected override void NotifyCallback(NSvn.Core.Notification notification)
        {
            if ( actionStatus[notification.Action] != null)
            {
                this.ankhContext.OutputPane.Write("{0} - {2}: {1}{3}"
                    ,actionStatus[notification.Action].ToString() 
                    ,notification.Path, notification.NodeKind.ToString()
                    ,Environment.NewLine );
            }
            if (notification.Action == NotifyAction.UpdateCompleted)
                this.ankhContext.OutputPane.WriteLine("\nUpdated to revision {0}.", notification.RevisionNumber);
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

        /// <summary>
        /// Prompt the user for a username and password.
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        private SimpleCredential PasswordPrompt( String realm, String username )
        {
            using( LoginDialog dialog = new LoginDialog() )
            {
                if ( realm != null )
                    dialog.Realm = realm;

                if ( username != null )
                    dialog.Username = username;

                if ( dialog.ShowDialog( this.ankhContext.HostWindow ) != DialogResult.OK )
                    return null;

                return new SimpleCredential( dialog.Username, dialog.Password );
            }
        }

        /// <summary>
        /// Prompt the user whether to accept a certain server certificate.
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="failures"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private SslServerTrustCredential SslServerTrustPrompt( string realm,
            SslFailures failures, SslServerCertificateInfo info )
        {
            using( SslServerTrustDialog dialog = new SslServerTrustDialog() )
            {
                dialog.Failures = failures;
                dialog.CertificateInfo = info;
                DialogResult result = dialog.ShowDialog();

                // Cancel means reject.
                if ( result == DialogResult.Cancel )
                    return null;

                SslServerTrustCredential cred = new SslServerTrustCredential();
                cred.AcceptedFailures = failures;
                
                // OK means trust permanently
                if ( result == DialogResult.OK )
                    cred.TrustPermanently = true;

                // anything else means trust temporarily
                return cred;
            }
        }

        /// <summary>
        /// Prompt the user for a passphrase for a client cert.
        /// </summary>
        /// <returns></returns>
        private SslClientCertificatePasswordCredential ClientCertificatePasswordPrompt()
        {
            using( ClientCertPassphraseDialog dialog = new ClientCertPassphraseDialog() )
            {
                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    SslClientCertificatePasswordCredential cred = new 
                        SslClientCertificatePasswordCredential();
                    cred.Password = dialog.Passphrase;
                    return cred;
                }
                else
                    return null;
            }
        }
        

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
        private string logMessage = null;
    }
}
