using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.RepositoryOpen
{
    public partial class RepositoryOpenDialog : Form
    {
        public RepositoryOpenDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GC.KeepAlive(textBox1.Text);
            //ParentForm.DialogResult = this.button1.DialogResult;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
