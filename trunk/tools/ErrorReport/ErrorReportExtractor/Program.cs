using System;
using System.Collections.Generic;
using System.Text;
using Outlook;
using Microsoft.SqlServer.Management.Smo;
using System.Configuration;
using System.Diagnostics;

namespace ErrorReportExtractor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            IProgressCallback callback = new ProgressCallback();
            try
            {
                callback.VerboseMode = true;

                MailContainer mail = new MailContainer(@"Final year project\ankhsvn\Error reports", callback);
                Storage storage = new Storage(callback);
                storage.StorePotentialReplies(mail.GetPotentialReplies());
                //IEnumerable<IErrorReport> items = mail.GetAllItems(null);
                //storage.Store(items);
            }
            catch (System.Exception ex)
            {
                callback.Exception(ex);
                Debug.WriteLine( ex.ToString() );
                Environment.Exit(1);
            }
            
        }
    }
}
