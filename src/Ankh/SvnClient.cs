// $Id$
using System;

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
    internal class SvnClient : Client
    {
        public SvnClient( AnkhContext ankhContext, string configDir ) :  base( configDir )
        {
            this.Init( ankhContext );
        }

        public SvnClient( AnkhContext ankhContext ) 
        {
            this.Init( ankhContext );
        }

        
        /// <summary>
        /// Invokes the LogMessage dialog.
        /// </summary>
        /// <param name="commitItems"></param>
        /// <returns></returns>
        internal IList ShowLogMessageDialog(IList items)
        {
            string templateText = this.GetTemplate();
            LogMessageTemplate template = new LogMessageTemplate( templateText );

            using( CommitDialog dialog = new CommitDialog() )
            {
                foreach( SvnItem item in items )
                {
                    CommitAction action = CommitAction.None;
                    Status status = 
                        this.ankhContext.SolutionExplorer.StatusCache[item.Path].Status;
                    switch( status.TextStatus )
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

                // we must give it diffs if it wants em
                dialog.DiffWanted += new DiffWantedDelegate( this.DiffWanted );
                if ( dialog.ShowDialog( this.ankhContext.HostWindow ) == DialogResult.OK )
                {
                    this.logMessage = dialog.LogMessage;
                    return (SvnItem[])dialog.GetSelectedTags( 
                        typeof(SvnItem) );
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
        
        protected override void OnLogMessage(LogMessageEventArgs args)
        {
            base.OnLogMessage( args );
            if ( args.Message == null )
                args.Message = this.logMessage;
        }

        protected override void OnNotification(NotificationEventArgs notification)
        {  
            base.OnNotification( notification );

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
        }

        protected override void OnCancel(CancelEventArgs args)
        {
            base.OnCancel( args );
            System.Diagnostics.Debug.WriteLine( "Cancel called" );
        }

        
        private string GetTemplate()
        {
            return this.ankhContext.Config.LogMessageTemplate != null ? 
                this.ankhContext.Config.LogMessageTemplate : "";
        }

        /// <summary>
        /// The commit dialog wants a diff. Give it one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DiffWanted( object sender, DiffWantedEventArgs args )
        {  
            // run the diff itself
            using ( MemoryStream diff = new MemoryStream() )
            {
                this.ankhContext.Client.Diff( new string[]{}, args.Path, Revision.Base, 
                    args.Path, Revision.Working, false, true, false, diff, Stream.Null );
                args.Diff = Encoding.Default.GetString( diff.ToArray() );                
            }

            // and provide the source file a verbo as well
            using( StreamReader reader = new StreamReader( args.Path, Encoding.Default ) )
                args.Source = reader.ReadToEnd();

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
   
        private void Init(AnkhContext ankhContext)
        {
            this.ankhContext = ankhContext;
            this.AuthBaton.Add( AuthenticationProvider.GetUsernameProvider() );
            this.AuthBaton.Add( AuthenticationProvider.GetSimpleProvider() );           
            this.AuthBaton.Add( AuthenticationProvider.GetSimplePromptProvider(
                new SimplePromptDelegate( this.PasswordPrompt ), 3 ) );
            this.AuthBaton.Add( AuthenticationProvider.GetSslServerTrustFileProvider() );
            this.AuthBaton.Add( AuthenticationProvider.GetSslServerTrustPromptProvider(
                new SslServerTrustPromptDelegate( this.SslServerTrustPrompt ) ) );
            this.AuthBaton.Add( 
                AuthenticationProvider.GetSslClientCertPasswordFileProvider() );
            this.AuthBaton.Add( 
                AuthenticationProvider.GetSslClientCertPasswordPromptProvider(
                new SslClientCertPasswordPromptDelegate( 
                this.ClientCertificatePasswordPrompt ), 3 ) );
            this.AuthBaton.Add( 
                AuthenticationProvider.GetSslClientCertFileProvider() );
            this.AuthBaton.Add( 
                AuthenticationProvider.GetSslClientCertPromptProvider( 
                new SslClientCertPromptDelegate( this.ClientCertificatePrompt ), 3 ) );
        }

        /// <summary>
        /// Pupulates actionStatus Hashtable.
        /// </summary>
        static SvnClient()
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
