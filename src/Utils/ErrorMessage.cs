// $Id$
using System;
using System.Diagnostics;
using System.Web;
using System.Windows.Forms;
using System.Reflection;
namespace Utils
{
    /// <summary>
    /// Performs error handling and reporting.
    /// </summary>
    public class ErrorMessage
    {
        private ErrorMessage()
        {
            // empty
        }

        /// <summary>
        /// Concatenates the error messages and exception types from (potentially)
        /// nested exceptions.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetMessage( Exception ex )
        {
            string msg = "";
            while( ex != null )
            {
                msg += ex.GetType().FullName + ": " + Environment.NewLine;
                msg += ex.Message + Environment.NewLine;
                msg += ex.StackTrace + Environment.NewLine;

                ex = ex.InnerException;
            }

            return msg;
        }

        /// <summary>
        /// Sends an error message by opening the user's mail client.
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="subject"></param>
        /// <param name="ex"></param>
        /// <param name="assembly">The assembly where the error originated. This will 
        /// be used to extract version information.</param>
        public static void SendByMail( string recipient, string subject, Exception ex, 
            Assembly assembly )
        {
            string msg = GetMessage( ex );
            msg += Environment.NewLine + Environment.NewLine + "Version: " + 
                assembly.GetName().Version.ToString();

            string command = string.Format( "mailto:{0}?subject={1}&body={2}",
                recipient, HttpUtility.UrlEncode(subject), 
                msg );

            Process p = new Process();
            p.StartInfo.FileName = command;
            p.StartInfo.UseShellExecute = true;

            p.Start();
        }

        /// <summary>
        /// Sends the error to a web page.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ex"></param>
        /// <param name="assembly">The assembly where the error originated. This will 
        /// be used to extract version information.</param>
        public static void SendByWeb( string url, Exception ex, Assembly assembly )
        {
            try
            {
                string msg = GetMessage( ex );
                string command = string.Format( "{0}?message={1}&version={2}", 
                    url, HttpUtility.UrlEncode( msg ), 
                    HttpUtility.UrlEncode( assembly.GetName().Version.ToString() ) );

                Process p = new Process();
                p.StartInfo.FileName = command;
                p.StartInfo.UseShellExecute = true;

                p.Start();
            }
            catch( Exception newex )
            {
                MessageBox.Show( GetMessage( newex ) );
            }
        }

        /// <summary>
        /// Asks if the user wants to send an error report by mail.
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="subject"></param>
        /// <param name="ex"></param>
        /// <param name="assembly">The assembly where the error originated. This will 
        /// be used to extract version information.</param>
        public static void QuerySendByMail( string recipient, string subject, Exception ex,
            Assembly assembly )
        {
            if ( MessageBox.Show( "An error has occurred. Do you wish to send an error report?",
                "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error ) == DialogResult.Yes )
            {
                SendByMail( recipient, subject, ex, assembly );
            }

        }

        /// <summary>
        /// Asks if the user wants to send an error report over the web.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ex"></param>
        /// <param name="assembly">The assembly where the error originated. This will 
        /// be used to extract version information.</param>
        public static void QuerySendByWeb( string url, Exception ex, Assembly assembly )
        {
            string message = GetMessage( ex );
            if ( MessageBox.Show( "An error has occurred. Do you wish to send an error report?" + 
                Environment.NewLine + 
                "(This will open your default web browser)" + Environment.NewLine + Environment.NewLine +
                message,
                "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error ) == DialogResult.Yes )
            {
                SendByWeb( url, ex, assembly );
            }

        }
    }
}
