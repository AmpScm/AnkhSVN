using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ErrorReportExtractor;

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

            Factory factory = new Factory();
            
            Application.Run( new MainForm(factory) );
        }
    }
}