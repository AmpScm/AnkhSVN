using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.UI;

namespace Ankh.Scc.SccUI
{
    public partial class EditReadOnlyFileDialog : VSDialogForm
    {
        public EditReadOnlyFileDialog()
        {
            InitializeComponent();
            pictureBox1.Image = SystemIcons.Exclamation.ToBitmap();
        }

        public EditReadOnlyFileDialog(SvnItem item)
            :this()
        {
            if(item == null)
                throw new ArgumentNullException("item");

            label1.Text = string.Format(label1.Text, item.Name);
        }
    }
}
