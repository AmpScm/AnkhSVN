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

                //MailContainer mail = new MailContainer();
                Storage storage = new Storage();
                
                //mail.SetProgressCallback( callback );
                storage.SetProgressCallback( callback );

                //storage.StorePotentialReplies(mail.GetPotentialReplies(@"Final year project\ankhsvn\Error reports"));
                storage.UpdateErrorReports( storage.GetAllReports() );
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
