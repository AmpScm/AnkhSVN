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
        bool _initialized;
        public EditLogMessageDialog()
        {
            InitializeComponent();
        }

        public string LogMessage
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);
            if (!_initialized && Context != null)
            {
                logMessageEditor1.Init(Context);
                _initialized = true;
            }
        }
    }
}
