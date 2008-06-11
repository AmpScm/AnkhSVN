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
            ContainerMode = VSContainerMode.UseTextEditorScope;
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

            if (!_initialized && Context != null)
            {
                logMessageEditor.Init(Context, true);
                AddCommandTarget(logMessageEditor.CommandTarget);
                AddWindowPane(logMessageEditor.WindowPane);
                _initialized = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
  
            LogMessage = _originalText;
        }
    }
}
