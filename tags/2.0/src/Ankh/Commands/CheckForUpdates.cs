﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using EnvDTE;

using Ankh.Ids;
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

            int interval = 24 * 7; // 1 week
            IAnkhConfigurationService config = e.GetService<IAnkhConfigurationService>();
            using (RegistryKey rk = config.OpenUserInstanceKey("UpdateCheck"))
            {                
                object value = rk.GetValue("Interval");

                if (value is int)
                {
                    interval = (int)value;

                    if (interval <= 0)
                        return;
                }
            }

            Version version = CurrentVersion;
            Version vsVersion = new Version(e.GetService<_DTE>(typeof(SDTE)).Version);
            Version osVersion = Environment.OSVersion.Version;

            StringBuilder sb = new StringBuilder();
            sb.Append("http://svc.ankhsvn.net/svc/");
            if (IsDevVersion())
                sb.Append("dev/");
            sb.Append("update-info/");
            sb.Append(version.ToString(2));
            sb.Append(".xml");
            sb.Append("?av=");
            sb.Append(version);
            sb.Append("&vs=");
            sb.Append(vsVersion);
            sb.Append("&os=");
            sb.Append(osVersion);

            if (IsDevVersion())
                sb.Append("&dev=1");

            sb.AppendFormat(CultureInfo.InvariantCulture, "&iv={0}", interval);
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

        bool? _isDevVersion;
        private bool IsDevVersion()
        {
            if (_isDevVersion.HasValue)
                return _isDevVersion.Value;

            _isDevVersion = true;
            foreach (AssemblyConfigurationAttribute a in typeof(CheckForUpdates).Assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false))
            {
                if (!string.IsNullOrEmpty(a.Configuration))
                    _isDevVersion = false;
            }

            return _isDevVersion.Value;
        }

        private void ShowUpdate(CommandEventArgs e)
        {
            string[] args;

            try
            {
                args = (string[])e.Argument;
            }
            catch { return; }
            
            if (args == null || args.Length < 7)
                return;

            string title = args[0], header = args[1], description = args[2], url= args[3], 
                urltext=args[4], version = args[5], tag = args[6];

            using (Ankh.UI.SccManagement.UpdateAvailableDialog uad = new Ankh.UI.SccManagement.UpdateAvailableDialog())
            {
                try
                {
                    uad.Text = string.Format(uad.Text, title);
                    uad.headLabel.Text = header;
                    uad.bodyLabel.Text = description;
                    uad.linkLabel.Text = urltext;
                    uad.linkLabel.Links.Add(0, urltext.Length).LinkData = url;

                    if (!string.IsNullOrEmpty(version))
                    {
                        uad.newVerLabel.Text = version;
                        uad.curVerLabel.Text = CurrentVersion.ToString();
                        uad.versionPanel.Enabled = uad.versionPanel.Visible = true;
                    }

                    if(string.IsNullOrEmpty(tag))
                        uad.sameCheck.Enabled = uad.sameCheck.Visible = false;
                }
                catch
                {
                    return; // Don't throw a visible exception from a background check!
                }

                System.Windows.Forms.Design.IUIService ui = e.GetService<System.Windows.Forms.Design.IUIService>();

                if (ui != null)
                    ui.ShowDialog(uad);
                else
                    uad.ShowDialog();

                if (uad.sameCheck.Checked)
                {
                    IAnkhConfigurationService config = e.GetService<IAnkhConfigurationService>();
                    using (RegistryKey rk = config.OpenUserInstanceKey("UpdateCheck"))
                    {
                        rk.SetValue("SkipTag", tag);
                    }
                }
            }
        }

        public void OnResponse(IAsyncResult ar)
        {
            IAnkhConfigurationService config = Context.GetService<IAnkhConfigurationService>();
            bool failed = true;
            string tag = null;
            try
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

                    if (hwr != null)
                    {
                        if (hwr.StatusCode == HttpStatusCode.NotFound)
                        {
                            failed = false;
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

                if (string.IsNullOrEmpty(body))
                {
                    failed = false;
                    return;
                }
                
                if(body[0] != '<' || body[body.Length - 1] != '>')
                    return; // No valid xml or empty

                failed = false;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(body);

                string title = NodeText(doc, "/u/i/t");
                string header = NodeText(doc, "/u/i/h") ?? title;
                string description = NodeText(doc, "/u/i/d");
                string url = NodeText(doc, "/u/i/u");
                string urltext = NodeText(doc, "/u/i/l");

                string version = NodeText(doc, "/u/i/v");

                tag = NodeText(doc, "/u/g");

                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
                {
                    if (!string.IsNullOrEmpty(version))
                    {
                        Version v = new Version(version);

                        if (v <= CurrentVersion)
                            return;
                    }

                    if (!string.IsNullOrEmpty(tag))
                    {
                        using (RegistryKey rk = config.OpenUserInstanceKey("UpdateCheck"))
                        {
                            string pTag = rk.GetValue("SkipTag") as string;

                            if (pTag == tag)
                                return;
                        }
                    }

                    IAnkhCommandService cs = Context.GetService<IAnkhCommandService>();

                    cs.PostExecCommand(AnkhCommand.CheckForUpdates,
                        new string[] { title, header, description, url, urltext, version, tag });
                }
            }
            finally
            {                
                using (RegistryKey rk = config.OpenUserInstanceKey("UpdateCheck"))
                {
                    object fails = rk.GetValue("Fails", 0);
                    rk.DeleteValue("LastCheck", false);
                    rk.DeleteValue("FailedChecks", false);
                    rk.SetValue("LastCheck", DateTime.UtcNow.Ticks);
                    if (tag != null)
                        rk.SetValue("LastTag", tag);
                    else
                        rk.DeleteValue("LastTag", false);

                    if (failed)
                    {
                        int f = 0;
                        if (fails is int)
                            f = (int)fails + 1;

                        rk.SetValue("FailedChecks", f);
                    }
                }
            }
        }

        static bool _checkedOnce;
        public static void MaybePerformUpdateCheck(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (_checkedOnce)
                return;

            _checkedOnce = true;

            IAnkhConfigurationService config = context.GetService<IAnkhConfigurationService>();
            using (RegistryKey rk = config.OpenUserInstanceKey("UpdateCheck"))
            {
                int interval = 24 * 7; // 1 week
                object value = rk.GetValue("Interval");

                if(value is int)
                {
                    interval = (int)value;

                    if(interval <= 0)
                        return;
                }

                TimeSpan ts = TimeSpan.FromHours(interval);

                
                value = rk.GetValue("LastCheck");
                long lv;
                if (value is string && long.TryParse((string)value, out lv))
                {
                    DateTime lc = new DateTime(lv, DateTimeKind.Utc);

                    if ((lc + ts) > DateTime.UtcNow)
                        return;

                    // TODO: Check the number of fails to increase the check interval
                }
            }

            context.GetService<IAnkhCommandService>().PostExecCommand(AnkhCommand.CheckForUpdates, null, CommandPrompt.Never);
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

        protected IAnkhServiceProvider Context
        {
            get { return (IAnkhServiceProvider)_site.GetService(typeof(IAnkhServiceProvider)); }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
