using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.Controls
{
    public partial class StatusPanelTestForm : VSDialogForm
    {
        public StatusPanelTestForm()
        {
            InitializeComponent();
        }

        public static void TestForm()
        {
            using (Form f = new StatusPanelTestForm())
            {
                f.BackColor = Color.FromArgb(0xFF, 0xFF, 0xFF);
                f.ShowDialog();
            }
        }
    }
}
