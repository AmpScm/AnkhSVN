using System;
using NSvn;
using NSvn.Common;
using NSvn.Core;
using Ankh.UI;
using System.Windows.Forms;

namespace Ankh
{
	/// <summary>
	/// Summary description for SvnContext.
	/// </summary>
	internal class SvnContext : NSvnContext
	{
        public SvnContext()
        {
            this.AddAuthenticationProvider( new DialogProvider() );
        }
		/// <summary>
		/// Invokes the LogMessage dialog.
		/// </summary>
		/// <param name="commitItems"></param>
		/// <returns></returns>
	    protected override string LogMessageCallback(NSvn.Core.CommitItem[] commitItems)
        {
            CommitDialog dialog = new CommitDialog();
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
            private LoginDialog loginDialog = new LoginDialog();

            #region Implementation of IAuthenticationProvider
            public NSvn.Common.ICredential NextCredentials()
            {
                if ( loginDialog.ShowDialog() == DialogResult.OK )
                    return new SimpleCredential( loginDialog.Username, 
                        loginDialog.Password );
                else
                    return null;
            }
            public NSvn.Common.ICredential FirstCredentials()
            {
                if ( loginDialog.ShowDialog() == DialogResult.OK )
                    return new SimpleCredential( loginDialog.Username, 
                        loginDialog.Password );
                else
                    return null;
            }
            public string Kind
            {
                get
                {
                    return SimpleCredential.AuthKind;
                }
            }
            #endregion
        }
        #endregion
    }
}
