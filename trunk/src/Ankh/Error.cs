using System;
using NSvn.Core;
using System.Windows.Forms;
using Utils;
using System.Reflection;
using System.Diagnostics;



namespace Ankh
{
    /// <summary>
    /// Encapsulates error handling functionality.
    /// </summary>
    internal class Error
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
            try
            {
                Type t = typeof(Error);
                t.InvokeMember( "DoHandle", BindingFlags.InvokeMethod |BindingFlags.Static |
                    BindingFlags.NonPublic, null, 
                    null, new object[]{ ex } );
            }
            catch( Exception x )
            {
                Debug.WriteLine( x );
            }
        }

        private static void DoHandle( ProgressRunner.ProgressRunnerException ex )
        {
            // we're only interested in the inner exception - we know where the 
            // outer one comes from
            Handle( ex.InnerException );
        }


        private static void DoHandle( WorkingCopyLockedException ex )
        {
            MessageBox.Show( "Your working copy appear to be locked. " + NL + 
                "Run Cleanup to amend the situation.", 
                "Working copy locked", MessageBoxButtons.OK, 
                MessageBoxIcon.Warning );
        }

        private static void DoHandle( AuthorizationFailedException ex )
        {
            MessageBox.Show( 
                "You failed to authorize against the remote repository. ",
                "Authorization failed", MessageBoxButtons.OK,
                MessageBoxIcon.Warning );
        }
        
        private static void DoHandle( ResourceOutOfDateException ex )
        {
            MessageBox.Show(
                "One or more of your local resources are out of date. " + 
                "You need to run Update before you can proceed with the operation",
                "Resource(s) out of date", MessageBoxButtons.OK,
                MessageBoxIcon.Warning );
        }

        private static void DoHandle( IllegalTargetException ex )
        {
            MessageBox.Show(  
                "One or more of the resources selected are not valid targets for this operation" + 
                Environment.NewLine + 
                "(Are you trying to commit a child of a newly added, but not committed resource?)",
                "Illegal target for this operation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning );
        }

        private static void DoHandle( Exception ex )
        {
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
