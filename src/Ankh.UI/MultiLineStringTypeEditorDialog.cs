using System;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for use in a type editor for a string. Presents a dialog for editing the string.
    /// </summary>
    public partial class MultiLineStringTypeEditorDialog : Form, IStringEditorDialog
    {
        public MultiLineStringTypeEditorDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The value entered in the dialog.
        /// </summary>
        public string Value
        {
            get
            {
                return String.Join( Environment.NewLine, this.textBox.Lines );
            }
            set
            {
                this.textBox.Text = value;
            }
        }

        /// <summary>
        /// Required by IStringEditorDialog.
        /// </summary>
        public Form Dialog
        {
            get { return this; }
        }        
    }
}
