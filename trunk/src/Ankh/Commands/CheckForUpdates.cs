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

        public override void OnExecute(CommandEventArgs e)
        {
            if (e.Argument != null)
                ShowUpdate(e);

            Version v = typeof(CheckForUpdates).Assembly.GetName().Version;

            StringBuilder sb = new StringBuilder();
            sb.Append("http://svc.ankhsvn.net/svc/update-info/");
            sb.Append(v.ToString(2));
            sb.Append(".xml");
            sb.Append("?av=");
            sb.Append(v.ToString());
            sb.Append("&vs=");
            sb.Append(Uri.EscapeDataString(e.GetService<_DTE>(typeof(SDTE)).Version));

            int x = 0;
            // Create some hashcode that is probably constant and unique but unidentifyable
            // behind a NAT interface
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
            }

            wr.BeginGetResponse(new AsyncCallback(OnResponse), wr);
        }

        private void ShowUpdate(CommandEventArgs e)
        {
            AnkhMessageBox mb = new AnkhMessageBox(e.Context);

            mb.Show("An update is available");
            //throw new NotImplementedException();
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
            string description = NodeText(doc, "/u/i/d");
            string url = NodeText(doc, "/u/i/u");
            string urltext = NodeText(doc, "/u/i/u");

            string version = NodeText(doc, "/u/i/v");

            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
            {
                IAnkhCommandService cs = (IAnkhCommandService)_site.GetService(typeof(IAnkhCommandService));

                cs.PostExecCommand(AnkhCommand.CheckForUpdates,
                    new string[] { title, description, url, urltext, version });

            }
        }

        private string NodeText(XmlDocument doc, string xpath)
        {
            XmlNode node = doc.SelectSingleNode(xpath);

            if (node != null)
                return node.Value;

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
