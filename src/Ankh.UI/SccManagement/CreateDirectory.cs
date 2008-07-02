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
            ContainerMode = VSContainerMode.UseTextEditorScope | VSContainerMode.TranslateKeys;
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);

            Initialize();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

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
