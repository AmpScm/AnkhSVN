using System;
using System.Collections.Generic;
using System.Text;
using Outlook;
using Microsoft.SqlServer.Management.Smo;
using System.Configuration;

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

                MailContainer mail = new MailContainer(ConfigurationManager.AppSettings["FolderPath"], callback);
                Storage storage = new Storage(callback);
                IEnumerable<IErrorReport> items = mail.GetAllItems(null);
                storage.Store(items);
            }
            catch (System.Exception ex)
            {
                callback.Exception(ex);
                Environment.Exit(1);
            }
            
        }
    }
}
