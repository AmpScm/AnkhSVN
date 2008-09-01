using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.ComponentModel;

namespace IssueZilla
{
    class CredentialService : ICredentials
    {

        public CredentialService( IWin32Window parent, ISynchronizeInvoke invoker )
        {
            this.parent = parent;
            this.invoker = invoker;
        }

        private delegate NetworkCredential GetCredentialInvokeCallback();

        public NetworkCredential GetCredential( Uri uri, string authType )
        {
            if ( this.invoker.InvokeRequired )
            {
                GetCredentialInvokeCallback method = 
                    delegate { return this.GetCredential( uri, authType ); };

                return this.invoker.Invoke( method, null ) as NetworkCredential;
            }

            using ( LoginDialog dialog = new LoginDialog(this.parent) )
            {
                if ( dialog.ShowDialog(this.parent) == DialogResult.OK )
                {
                    return new NetworkCredential( dialog.UserName, dialog.PassWord );
                }
                else
                {
                    return null;
                }
            }
        }

        private IWin32Window parent;
        private ISynchronizeInvoke invoker;
    }
}
