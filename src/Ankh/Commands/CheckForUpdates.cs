using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using System.Net;
using System.Reflection;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using System.Net.NetworkInformation;
using System.Xml;
using System.IO;
using System.ComponentModel;
using Ankh.UI;

namespace Ankh.Commands
{
    [Command(AnkhCommand.CheckForUpdates, AlwaysAvailable = true)]
    class CheckForUpdates : CommandBase, IComponent
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            // Always available
        }

        static Version CurrentVersion
        {
            get { return typeof(CheckForUpdates).Assembly.GetName().Version; }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            if (e.Argument != null)
            {
                ShowUpdate(e);
                return;
            }

            Version version = CurrentVersion;
            Version vsVersion = new Version(e.GetService<_DTE>(typeof(SDTE)).Version);
            Version osVersion = Environment.OSVersion.Version;

            StringBuilder sb = new StringBuilder();
            sb.Append("http://svc.ankhsvn.net/svc/update-info/");
            sb.Append(version.ToString(2));
            sb.Append(".xml");
            sb.Append("?av=");
            sb.Append(version);
            sb.Append("&vs=");
            sb.Append(vsVersion);
            sb.Append("&os=");
            sb.Append(osVersion);
            int x = 0;
            // Create some hashcode that is probably constant and unique for all users
            // using the same IP address, but not translatable to a single user
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    string type = ni.NetworkInterfaceType.ToString();

                    if(type.Contains("Ethernet") || type.Contains("Wireless"))
                        x ^= ni.GetPhysicalAddress().GetHashCode();
                }
            }
            catch { }

            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "&xx={0}", x);

            Uri updateUri = new Uri(sb.ToString());
            WebRequest wr = WebRequest.Create(updateUri);

            HttpWebRequest hwr = wr as HttpWebRequest;

            if (hwr != null)
            {
                hwr.AllowAutoRedirect = true;
                hwr.AllowWriteStreamBuffering = true;
                hwr.UserAgent = string.Format("AnkhSVN/{0} VisualStudio/{1} Windows/{2}", version, vsVersion, osVersion);
            }

            wr.BeginGetResponse(new AsyncCallback(OnResponse), wr);
        }

        private void ShowUpdate(CommandEventArgs e)
        {
            string[] args = (string[])e.Argument;
            string title = args[0], header = args[1], description = args[2], url= args[3], urltext=args[4], version = args[5];

            using (Ankh.UI.SccManagement.UpdateAvailableDialog uad = new Ankh.UI.SccManagement.UpdateAvailableDialog())
            {
                uad.Text = string.Format(uad.Text, title);
                uad.headLabel.Text = header;
                //uad.desc
                uad.linkLabel.Text = urltext;
                uad.linkLabel.Links.Add(0, urltext.Length).LinkData = url;

                if (!string.IsNullOrEmpty(version))
                {
                    uad.newVerLabel.Text = version;
                    uad.curVerLabel.Text = CurrentVersion.ToString();
                    uad.versionPanel.Enabled = uad.versionPanel.Visible = true;
                }

                System.Windows.Forms.Design.IUIService ui = e.GetService<System.Windows.Forms.Design.IUIService>();

                if (ui != null)
                    ui.ShowDialog(uad);
                else
                    uad.ShowDialog();
            }
        }

        public void OnResponse(IAsyncResult ar)
        {
            WebRequest rq = ((WebRequest)ar.AsyncState);
            WebResponse wr;
            try
            {
                wr = rq.EndGetResponse(ar);
            }
            catch (WebException e)
            {
                HttpWebResponse hwr = e.Response as HttpWebResponse;

                if(hwr != null)
                {
                    if(hwr.StatusCode == HttpStatusCode.NotFound)
                    {
                        return; // File not found.. Update info not yet or no longer available
                    }
                }

                return;
            }
            catch
            {
                return;
            }

            if (wr.ContentLength > 65536) // Not for us.. We expect a few hundred bytes max
                return;

            string body;
            using (Stream s = wr.GetResponseStream())
            using (StreamReader sr = new StreamReader(s))
            {
                body = sr.ReadToEnd().Trim();
            }

            if (string.IsNullOrEmpty(body) || body[0] != '<' || body[body.Length - 1] != '>')
                return; // No valid xml or empty

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(body);

            string title = NodeText(doc, "/u/i/t");
            string header = NodeText(doc, "/u/i/h") ?? title;
            string description = NodeText(doc, "/u/i/d");
            string url = NodeText(doc, "/u/i/u");
            string urltext = NodeText(doc, "/u/i/l");

            string version = NodeText(doc, "/u/i/v");            

            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
            {
                if (!string.IsNullOrEmpty(version))
                {
                    Version v = new Version(version);

                    if (v <= CurrentVersion)
                        return;
                }

                IAnkhCommandService cs = (IAnkhCommandService)_site.GetService(typeof(IAnkhCommandService));

                cs.PostExecCommand(AnkhCommand.CheckForUpdates,
                    new string[] { title, header, description, url, urltext, version });
            }
        }

        private string NodeText(XmlDocument doc, string xpath)
        {
            XmlNode node = doc.SelectSingleNode(xpath);

            if (node != null)
                return node.InnerText;

            return null;
        }

        #region IComponent Members

        event EventHandler IComponent.Disposed
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        ISite _site;
        ISite IComponent.Site
        {
            get { return _site; }
            set { _site = value; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
