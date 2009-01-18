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
    }
}
