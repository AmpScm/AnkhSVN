using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.SccManagement
{
    public partial class CreateDirectory : VSContainerForm
    {
        public CreateDirectory()
        {
            InitializeComponent();
        }

        public string NewDirectoryName
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public bool NewDirectoryReadonly
        {
            get { return !textBox1.Enabled; }
            set { textBox1.Enabled = !value; }
        }

        public string LogMessage
        {
            get { return logMessage.Text; }
        }
    }
}
