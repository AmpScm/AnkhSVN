using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
{
    public partial class EditReadonlyFileDialog : VSDialogForm
    {
        public EditReadonlyFileDialog()
        {
            InitializeComponent();
            pictureBox1.Image = SystemIcons.Exclamation.ToBitmap();
        }

        public EditReadonlyFileDialog(SvnItem item)
            :this()
        {
            if(item == null)
                throw new ArgumentNullException("item");

            label1.Text = string.Format(label1.Text, item.Name);
        }
    }
}
