using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using System.Configuration;
using System.Diagnostics;
using System.Transactions;

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

                using ( ConnectionScope cscope = new ConnectionScope() )
                using ( TransactionScope tscope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.Zero) )
                {

                    Pop3Importer importer = new Pop3Importer( "mail.broadpark.no", 110, "ankhsvn", "grynte" );
                    importer.SetProgressCallback( callback );
                    //importer.GetUidl();
                    SqlServerStorage storage = new SqlServerStorage();
                    storage.SetProgressCallback( callback );

                    IEnumerable<IErrorReport> reports = importer.GetItems();
                    storage.Store( reports );

                    IEnumerable<IMailItem> items = importer.GetPotentialReplies();
                    storage.StorePotentialReplies( items );

                    tscope.Complete(); // Nope
                }

                //OutlookContainer mail = new OutlookContainer();
                //SqlServerStorage storage = new SqlServerStorage();

                //mail.SetProgressCallback( callback );
                //storage.SetProgressCallback( callback );

                //int lastIndex;

                //storage.StorePotentialReplies( mail.GetPotentialReplies( @"Final year project\ankhsvn\Error reports", 3000, out lastIndex ) );
                ////storage.UpdateErrorReports( storage.GetAllReports() );
                ////IEnumerable<IErrorReport> items = mail.GetItems( @"Final year project\ankhsvn\Error reports", new DateTime( 2006, 4, 18 ), 3150 );
                ////storage.Store( items );
                //callback.Info( "Finished." );
                //callback.Info( "Last index retrieved was {0}.", lastIndex );
                Console.WriteLine("Done.");
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
