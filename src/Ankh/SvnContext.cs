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
        public SvnContext( AnkhContext ankhContext, string configDir ) :  base( configDir )
        {
            this.Init( ankhContext );
        }

        public SvnContext( AnkhContext ankhContext ) 
        {
            this.Init( ankhContext );
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

        /// <summary>
        /// Prompt the user for a username and password.
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        private SimpleCredential PasswordPrompt( String realm, String username, bool maySave )
        {
            using( LoginDialog dialog = new LoginDialog() )
            {
                if ( realm != null )
                    dialog.Realm = realm;

                if ( username != null )
                    dialog.Username = username;

                dialog.MaySave = maySave;

                if ( dialog.ShowDialog( this.ankhContext.HostWindow ) != DialogResult.OK )
                    return null;

                return new SimpleCredential( dialog.Username, dialog.Password, dialog.ShallSave );
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
            SslFailures failures, SslServerCertificateInfo info, bool maySave )
        {
            using( SslServerTrustDialog dialog = new SslServerTrustDialog() )
            {
                dialog.Failures = failures;
                dialog.CertificateInfo = info;
                dialog.MaySave = maySave;
                DialogResult result = dialog.ShowDialog();

                // Cancel means reject.
                if ( result == DialogResult.Cancel )
                    return null;

                SslServerTrustCredential cred = new SslServerTrustCredential();
                cred.AcceptedFailures = failures;
                
                if ( dialog.ShallSave ) 
                    cred.MaySave = true;       
         
                return cred;
            }
        }

        /// <summary>
        /// Prompt the user for a passphrase for a client cert.
        /// </summary>
        /// <returns></returns>
        private SslClientCertificatePasswordCredential ClientCertificatePasswordPrompt( 
            string realm, bool maySave )
        {
            using( ClientCertPassphraseDialog dialog = new ClientCertPassphraseDialog() )
            {
                dialog.MaySave = maySave;
                dialog.Realm = realm;

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
        /// Prompts the user for a client certificate.
        /// </summary>
        /// <returns></returns>
        private SslClientCertificateCredential ClientCertificatePrompt( string realm, bool maySave )
        {
            using( ClientCertDialog dialog = new ClientCertDialog() )
            {
                dialog.Realm = realm;
                dialog.MaySave = maySave;

                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    SslClientCertificateCredential cred = new 
                        SslClientCertificateCredential();
                    cred.CertificateFile = dialog.CertificateFile;
                    cred.MaySave = dialog.ShallSave;
                    return cred;
                }
                else
                    return null;
            }
        }     
   

        private CancelOperation CancelCallback()
        {
            System.Diagnostics.Debug.WriteLine( "Cancel called" );
            Application.DoEvents();

            return CancelOperation.DontCancel;
        }

        private void Init(AnkhContext ankhContext)
        {
            this.ankhContext = ankhContext;
            this.AddAuthenticationProvider( AuthenticationProvider.GetUsernameProvider() );
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
                this.ClientCertificatePasswordPrompt ), 3 ) );
            this.AddAuthenticationProvider( 
                AuthenticationProvider.GetSslClientCertFileProvider() );
            this.AddAuthenticationProvider( 
                AuthenticationProvider.GetSslClientCertPromptProvider( 
                new SslClientCertPromptDelegate( this.ClientCertificatePrompt ), 3 ) );

            this.ClientContext.CancelCallback = new CancelCallback( this.CancelCallback );

        }

        /// <summary>
        /// Pupulates actionStatus Hashtable.
        /// </summary>
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
