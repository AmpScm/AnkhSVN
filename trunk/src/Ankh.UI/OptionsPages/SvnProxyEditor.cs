using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using SharpSvn.Implementation;

namespace Ankh.UI.OptionsPages
{
    public partial class SvnProxyEditor : VSDialogForm
    {
        public SvnProxyEditor()
        {
            InitializeComponent();
        }

        bool _wasEnabled;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            using (RegistryKey rk = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Tigris.org\\Subversion\\Servers\\Global"))
            {
                string host = rk.GetValue(SvnConfigNames.HttpProxyHost) as string ?? "";
                string port = rk.GetValue(SvnConfigNames.HttpProxyPort) as string ?? "8080";
                string username = rk.GetValue(SvnConfigNames.HttpProxyUserName) as string ?? "";
                string password = rk.GetValue(SvnConfigNames.HttpProxyPassword) as string ?? "";
                string exception = rk.GetValue(SvnConfigNames.HttpProxyExceptions) as string ?? "";

                hostBox.Text = host;

                ushort p;
                if (ushort.TryParse(port.Trim(), out p))
                    portBox.Text = p.ToString();

                usernameBox.Text = username;
                passwordBox.Text = password;
                exceptionsBox.Text = NormalizeExceptionText(exception, true);

                proxyEnabled.Checked = _wasEnabled = !string.IsNullOrEmpty(host);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (proxyEnabled.Checked)
            {
                using (RegistryKey rk = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Tigris.org\\Subversion\\Servers\\Global"))
                {
                    rk.SetValue(SvnConfigNames.HttpProxyHost, hostBox.Text.Trim());
                    rk.SetValue(SvnConfigNames.HttpProxyPort, portBox.Text.Trim());
                    rk.SetValue(SvnConfigNames.HttpProxyUserName, usernameBox.Text.Trim());
                    rk.SetValue(SvnConfigNames.HttpProxyPassword, passwordBox.Text);
                    rk.SetValue(SvnConfigNames.HttpProxyExceptions, NormalizeExceptionText(exceptionsBox.Text, false));
                }
            }
            else if (_wasEnabled)
            {
                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Tigris.org\\Subversion\\Servers\\Global", true))
                {
                    rk.DeleteValue(SvnConfigNames.HttpProxyHost);
                    rk.DeleteValue(SvnConfigNames.HttpProxyPort);
                    rk.DeleteValue(SvnConfigNames.HttpProxyUserName);
                    rk.DeleteValue(SvnConfigNames.HttpProxyPassword);
                    rk.DeleteValue(SvnConfigNames.HttpProxyExceptions);
                }
            }
            Context.GetService<ISvnClientPool>().FlushAllClients();
        }

        private string NormalizeExceptionText(string exceptionText, bool forDialog)
        {
            StringBuilder result = new StringBuilder();

            if (!string.IsNullOrEmpty(exceptionText))
            {
                bool next = false;
                foreach (string x in exceptionText.Split(',', ';', ' ', '\t', '\r', '\n'))
                {
                    string item = x.Trim();
                    if (!string.IsNullOrEmpty(x))
                    {
                        if (next)
                            result.Append(forDialog ? Environment.NewLine : ", ");
                        else
                            next = true;

                        result.Append(x);
                    }
                }
            }

            return result.ToString();
        }

        private void proxyEnabled_CheckedChanged(object sender, EventArgs e)
        {
            proxyGroup.Enabled = proxyEnabled.Checked;
        }
    }
}
