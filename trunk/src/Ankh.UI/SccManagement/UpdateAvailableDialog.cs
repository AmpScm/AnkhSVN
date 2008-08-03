using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.SccManagement
{
    public partial class UpdateAvailableDialog : Form
    {
        public UpdateAvailableDialog()
        {
            InitializeComponent();
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Uri uri; // Just some minor precautions
            if (Uri.TryCreate((string)e.Link.LinkData, UriKind.Absolute, out uri) && !uri.IsFile && !uri.IsUnc)
            {
                try
                {
                    System.Diagnostics.Process.Start((string)e.Link.LinkData);
                }
                catch
                { }
            }
        }
    }
}
