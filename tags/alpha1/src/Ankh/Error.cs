using System;
using NSvn.Core;
using System.Windows.Forms;
using Utils;




namespace Ankh
{
	/// <summary>
	/// Encapsulates error handling functionality.
	/// </summary>
	public class Error
	{
		private Error()
		{
			// nothing to see here
		}


        /// <summary>
        /// Handles an exception.
        /// </summary>
        /// <param name="ex"></param>
        public static void Handle( Exception ex )
        {
            if ( ex is WorkingCopyLockedException )
            {
                MessageBox.Show( "Your working copy appear to be locked. " + NL + 
                    "Run svn cleanup to amend the situation.", 
                    "Working copy locked", MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning );
            }
            else if ( ex is AuthorizationFailedException )
            {
                MessageBox.Show( 
                    "You failed to authorize against the remote repository. ",
                    "Authorization failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning );
            }
            else
            {
#if REPORTERROR
                Utils.ErrorMessage.QuerySendByWeb( "http://arild.no-ip.com/error/report.aspx", ex,
                    typeof(Connect).Assembly );
#else
                MessageBox.Show( ex.Message, "Unexpected error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error );
#endif
               
            }
        }
        private static readonly string NL = Environment.NewLine;
    }
}
