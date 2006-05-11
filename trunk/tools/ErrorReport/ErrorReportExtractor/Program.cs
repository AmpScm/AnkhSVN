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

                ConnectionScope.SetConnectionString( ErrorReportExtractor.Properties.Settings.Default.ErrorReportsConnectionString );

                OutlookContainer mail = new OutlookContainer();
                SqlServerStorage storage = new SqlServerStorage();

                mail.SetProgressCallback( callback );
                storage.SetProgressCallback( callback );

                storage.StorePotentialReplies( mail.GetPotentialReplies( @"Final year project\ankhsvn\Error reports", 3000 ) );
                //storage.UpdateErrorReports( storage.GetAllReports() );
                //IEnumerable<IErrorReport> items = mail.GetItems( @"Final year project\ankhsvn\Error reports", new DateTime( 2006, 4, 18 ), 3150 );
                //storage.Store( items );
                callback.Info( "Finished." );
                Console.ReadLine();
            }
            catch ( System.Exception ex )
            {
                callback.Exception( ex );
                Debug.WriteLine( ex.ToString() );
                Console.ReadLine();
                Environment.Exit( 1 );
            }
            
        }
    }
}
