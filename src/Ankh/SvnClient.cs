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
using System.Diagnostics;

namespace Ankh
{
    /// <summary>
    /// Summary description for SvnContext.
    /// </summary>
    public class SvnClient : Client
    {
        public SvnClient( IContext ankhContext, string configDir ) :  base( configDir )
        {
            this.Init( ankhContext );
        }

        public SvnClient( IContext ankhContext ) 
        {
            this.Init( ankhContext );
        }       

        private delegate void OnNotificationDelegate(NotificationEventArgs notification);

        protected override void OnNotification(NotificationEventArgs notification)
        {  
            if ( this.ankhContext.UIShell.SynchronizingObject.InvokeRequired )
            {
                Debug.WriteLine( "OnNotification: Invoking back to main GUI thread", 
                    "Ankh" );
                this.ankhContext.UIShell.SynchronizingObject.Invoke(
                    new OnNotificationDelegate(this.OnNotification),
                    new object[]{notification} );
                return;
            }

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
            System.Diagnostics.Debug.WriteLine( "Cancel called. Cancelled: " + args.Cancel, 
                "Ankh" );
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
                DialogResult result = dialog.ShowDialog( this.ankhContext.HostWindow );

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

                if ( dialog.ShowDialog( this.ankhContext.HostWindow ) == DialogResult.OK )
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

                if ( dialog.ShowDialog( this.ankhContext.HostWindow ) == DialogResult.OK )
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
   
        private void Init(IContext ankhContext)
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
            actionStatus[NotifyAction.Locked] =                 "Locked";
            actionStatus[NotifyAction.Unlocked] =               "Unlocked";
            actionStatus[NotifyAction.FailedLock] =             "Failed lock";
            actionStatus[NotifyAction.FailedUnlock] =           "Failed unlock";
        }
        
        static readonly Hashtable actionStatus = new Hashtable();
        private IContext ankhContext;
        private static IDictionary map = new Hashtable();
    }
}
