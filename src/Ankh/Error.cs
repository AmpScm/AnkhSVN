using System;
using NSvn.Core;
using System.Windows.Forms;
using Utils;
using System.Reflection;
using System.Diagnostics;
using Ankh.UI;



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

        private static void DoHandle( SvnClientException ex )
        {
            if ( ex.ErrorCode == LockedFileErrorCode )
            {
                MessageBox.Show(
                    ex.Message + NL + NL +
                    "Avoid versioning files that can be locked by VS.NET. " + 
                    "These include *.ncb, *.projdata etc." + NL +
                    "See the AnkhSVN FAQ for more details.",
                    "File exclusively locked",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error );
            }
            else
            {
                ShowErrorDialog(ex, false, false);
            }
        }

        

        private static void DoHandle( Exception ex )
        {
#if REPORTERROR
            ShowErrorDialog(ex, true, true);
#else
                MessageBox.Show( ex.Message, "Unexpected error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error );
#endif              

        }

        private static void ShowErrorDialog(Exception ex, bool showStackTrace, bool internalError )
        {
            string stackTrace = GetNestedStackTraces( ex );
            string message = GetNestedMessages( ex );
            using( ErrorDialog dlg = new ErrorDialog() )
            {
                dlg.ErrorMessage = message;
                dlg.ShowStackTrace = showStackTrace;
                dlg.StackTrace = stackTrace;
                dlg.InternalError = internalError;
                if ( dlg.ShowDialog() == DialogResult.Retry )
                {
                    Utils.ErrorMessage.SendByWeb( "http://arild.no-ip.com/error/report.aspx",
                        ex, typeof(Connect).Assembly );
                }
            }
        }

        private static string GetNestedStackTraces( Exception ex )
        {
            if ( ex == null )
                return String.Empty;
            else
                return ex.StackTrace + NL + NL + GetNestedStackTraces( ex.InnerException );
        }

        private static string GetNestedMessages( Exception ex )
        {
            if ( ex == null )
                return String.Empty;
            else
                return ex.Message + NL + NL + GetNestedMessages( ex.InnerException );
        }

        private static readonly string NL = Environment.NewLine;
        private const int LockedFileErrorCode = 720032;
    }
}
