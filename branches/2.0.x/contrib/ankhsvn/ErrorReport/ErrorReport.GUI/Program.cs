using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ErrorReportExtractor;
using ErrorReport.GUI.Properties;

namespace ErrorReport.GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            ConnectionScope.SetConnectionString( Settings.Default.ConnectionString );
            ServiceProvider factory = SetUpServices();
            
            Application.Run( new MainForm(factory) );
        }

        private static ServiceProvider SetUpServices()
        {
            ServiceProvider provider = new ServiceProvider();
            provider.ProfferService<IMailer>( new ErrorReportExtractor.Mailer());
            provider.ProfferService<ITemplateManager>( new TemplateManager() );
            provider.ProfferService<IStorage>( new SqlServerStorage() );
            //provider.ProfferService<IReportContainer>( new OutlookContainer() );

            return provider;
        }
    }
}