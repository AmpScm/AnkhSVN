using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.UI.SccManagement
{
    public partial class CreateDirectoryDialog : VSContainerForm
    {
        public CreateDirectoryDialog()
        {
            InitializeComponent();
        }

        public string NewDirectoryName
        {
            get { return directoryNameBox.Text; }
            set { directoryNameBox.Text = value; }
        }

        public bool NewDirectoryReadonly
        {
            get { return !directoryNameBox.Enabled; }
            set { directoryNameBox.Enabled = !value; }
        }

        public string LogMessage
        {
            get { return logMessage.Text; }
        }       
    }
}
