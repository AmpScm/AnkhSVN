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

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            Initialize();
        }

        bool _initialized, _hooked;
        void Initialize()
        {
            if (!_initialized && Context != null)
            {
                logMessage.Init(Context, true);
                _initialized = true;
            }

            if (!_hooked && _initialized && Context != null && IsHandleCreated)
            {
                AddCommandTarget(logMessage.CommandTarget);
                AddWindowPane(logMessage.WindowPane);
                _hooked = true;
            }
        }

        public string NewDirectoryName
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public string LogMessage
        {
            get { return logMessage.Text; }
        }
    }
}
