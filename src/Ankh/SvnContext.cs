using System;
using NSvn;
using NSvn.Common;
using NSvn.Core;
using Ankh.UI;
using System.Windows.Forms;
using System.Collections;

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
            CommitDialog dialog = new CommitDialog( commitItems );
            if ( dialog.ShowDialog() == DialogResult.OK )
                return dialog.LogMessage;
            else
                return null;
        }

        protected override void NotifyCallback(NSvn.Core.Notification notification)
        {        
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

        private AnkhContext ankhContext;
    }
}
