using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ErrorReport.GUI
{
    public partial class SignatureDialog : Form
    {
        public SignatureDialog()
        {
            InitializeComponent();
        }

        public string Signature
        {
            get { return signatureTextBox.Text; }
            set { signatureTextBox.Text = value; }
        }
    }
}