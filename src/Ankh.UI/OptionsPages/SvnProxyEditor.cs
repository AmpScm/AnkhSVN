using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

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

            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Tigris.org\\Subversion\\Servers\\Global"))
            {
                string host = rk.GetValue("http-proxy-host") as string ?? "";
                string port = rk.GetValue("http-proxy-port") as string ?? "8080";
                string username = rk.GetValue("http-proxy-username") as string ?? "";
                string password = rk.GetValue("http-proxy-password") as string ?? "";
                string exception = rk.GetValue("http-proxy-exceptions") as string ?? "";

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
                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Tigris.org\\Subversion\\Servers\\Global", true))
                {
                    rk.SetValue("http-proxy-host", hostBox.Text.Trim());
                    rk.SetValue("http-proxy-port", portBox.Text.Trim());
                    rk.SetValue("http-proxy-username", usernameBox.Text.Trim());
                    rk.SetValue("http-proxy-password", passwordBox.Text);
                    rk.SetValue("http-proxy-exceptions", NormalizeExceptionText(exceptionsBox.Text, false));
                }
            }
            else if (_wasEnabled)
            {
                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Tigris.org\\Subversion\\Servers\\Global", true))
                {
                    rk.DeleteValue("http-proxy-host");
                    rk.DeleteValue("http-proxy-port");
                    rk.DeleteValue("http-proxy-username");
                    rk.DeleteValue("http-proxy-password");
                    rk.DeleteValue("http-proxy-exceptions");
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
                foreach (string x in exceptionText.Split(',', ';', ' ', '\r', '\n'))
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
