using System;
using System.Windows.Forms;

namespace Ankh.UI.Controls
{
    /// <summary>
    /// Provides a link to sign-up for CollabNet's CloudForge services
    /// </summary>
    public partial class CloudForgeControl : UserControl
    {
        private static readonly string CF_SIGNUP_URL = "http://svc.ankhsvn.net/svc/go/?t=CloudForgeSignup";

        public CloudForgeControl()
        {
            InitializeComponent();
        }

        private void cloudForgePictureBox_Click(object sender, EventArgs e)
        {
            Control control = this;
            IAnkhServiceProvider sp = control as IAnkhServiceProvider;
            while (control != null && sp == null)
            {
                control = control.Parent;
                sp = control as IAnkhServiceProvider;
            }
            if (sp != null)
            {
                Ankh.VS.IAnkhWebBrowser wb = sp.GetService<Ankh.VS.IAnkhWebBrowser>();
                if (wb != null)
                {
                    Ankh.VS.AnkhBrowserArgs args = new Ankh.VS.AnkhBrowserArgs();
                    args.External = true;
                    Uri cfSignUpUrl = new Uri(CF_SIGNUP_URL);
                    wb.Navigate(cfSignUpUrl, args);
                }
            }
        }
    }
}
