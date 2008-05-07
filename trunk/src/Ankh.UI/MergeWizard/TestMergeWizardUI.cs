using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.MergeWizard
{
    static class TestMergeWizardUI
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            WizardFramework.Wizard wizard = new MergeWizard();
            WizardFramework.WizardDialog wd = new WizardFramework.WizardDialog(wizard);

            Application.Run(wd);
        }
    }
}
