using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.OptionsPages
{
    public partial class EnvironmentSettingsControl : AnkhOptionsPageControl
    {
        public EnvironmentSettingsControl()
        {
            InitializeComponent();
        }

        private void authenticationEdit_Click(object sender, EventArgs e)
        {
            using (SvnAuthenticationCacheEditor editor = new SvnAuthenticationCacheEditor())
            {
                editor.ShowDialog(Context);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void proxyEdit_Click(object sender, EventArgs e)
        {
            using (SvnProxyEditor editor = new SvnProxyEditor())
            {
                editor.ShowDialog(Context);
            }
        }
    }
}
