using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Services;

namespace Ankh.UI.SvnLog
{
    public partial class EditLogMessageDialog : VSContainerForm
    {
        public EditLogMessageDialog()
        {
            InitializeComponent();
        }

        string _originalText;
        public string LogMessage
        {
            get { return logMessageEditor.Text; }
            set { logMessageEditor.Text = _originalText = value; }
        }
    }
}
