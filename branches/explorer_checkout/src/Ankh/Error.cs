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
            else if ( ex is ResourceOutOfDateException )
            {
                MessageBox.Show(
                    "One or more of your local resources are out of date. " + 
                    "You need to run Update before you can proceed with the operation",
                    "Resource(s) out of date", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning );
            }
            else if ( ex is IllegalTargetException )
            {
                MessageBox.Show(  
                    "One or more of the resources selected are not valid targets for this operation" + 
                    Environment.NewLine + 
                    "(Are you trying to commit a child of a newly added, but not committed resource?)",
                    "Illegal target for this operation",
                    MessageBoxButtons.OK,
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
