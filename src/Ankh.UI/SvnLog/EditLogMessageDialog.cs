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
            ContainerMode = VSContainerMode.UseTextEditorScope | VSContainerMode.TranslateKeys;
        }

        string _originalText;
        public string LogMessage
        {
            get { return logMessageEditor.Text; }
            set { logMessageEditor.Text = _originalText = value; }
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);

            Initialize();
        }

        bool _initialized, _hooked;
        void Initialize()
        {
            if (!_initialized && Context != null)
            {
                logMessageEditor.Init(Context, true);
                _initialized = true;
            }

            if(!_hooked && _initialized && Context != null && IsHandleCreated)
            {
                AddCommandTarget(logMessageEditor.CommandTarget);
                AddWindowPane(logMessageEditor.WindowPane);
                _hooked = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Initialize();
            LogMessage = _originalText;
        }
    }
}
