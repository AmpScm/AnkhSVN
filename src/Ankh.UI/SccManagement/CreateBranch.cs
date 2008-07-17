using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.SccManagement
{
    public partial class CreateBranch : VSContainerForm
    {
        public CreateBranch()
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
            get { return toUrlBox.Text; }
            set { toUrlBox.Text = value; }
        }

        public bool NewDirectoryReadonly
        {
            get { return !toUrlBox.Enabled; }
            set { toUrlBox.Enabled = !value; }
        }

        public string LogMessage
        {
            get { return logMessage.Text; }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void versionBox_TextChanged(object sender, EventArgs e)
        {
            specificVersionRadio.Checked = true;
        }
    }
}
