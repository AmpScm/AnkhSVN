// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// Summary description for LogMessageControl.
    /// </summary>
    public partial class LogMessageControl : System.Windows.Forms.UserControl
    {
        public LogMessageControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call
        }

        public string LogMessage
        {
            get
            {
                return this.logTextBox.Text;
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}



