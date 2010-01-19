// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.Win32;

using Ankh.UI;
using Ankh.VS;
using System.Text.RegularExpressions;

/*****************************************************************
 * This command performs update checks by calling our webservice
 *****************************************************************
 * A sample request would be:
 * http://svc.ankhsvn.net/svc/dev/update-info/2.0.xml?av=2.0.1234.5678&vs=9.0&os=6.0.6001.65536&iv=168&xx=2062238964
 *     
 * 2.0.xml is the major version; allowing updates per major version to be a static file
 *  av: Ankh version
 *  vs: Visual Studion version
 *  os: OS version
 *  iv: Update interval in hours (For usage statistics)
 *  xx: Hashcode per machine (For usage statistics)
 *  dn: Comma separated list of installed .Net versions (For usage statistics)
 *  pc: Number of processors available (For usage statistics)
 * 
 * Some sample valid responses are:
 *   * A 0 byte file or a file containing just whitespace. There is no update available
 *   * The following response 
 * <u>
 * <g>1</g>
 * <i>
 *   <t>Update available</t>
 *   <h>AnkhSVN Update Available</h>
 *   <d>A new version of AnkhSVN 2.0 might be available.
 * This is a test of the update service. The testmode will be deactivated after 2008-07-08.
 *
 * Thanks for your support.
 *   </d>
 *   <u>http://ankhsvn.net/daily/</u>
 *   <l>Download a new daily version</l>
 *   <v>2.0.9999.9999</v>
 * </i>
 *  </u>
 * 
 * g: 1             Informational version. Used for the tick 'never show this again'
 * i:               Update item block
 *  t:              Title of the dialog (appended to 'AnkhSVN -')
 *  h:              The header text (centered above the description); defaults to the 't' value
 *  d:              The description body
 *  u:              The title of the link
 *  l:              The link value
 *  v:              The new version (optional). If set the message is only shown on 
 *                  versions older than the specified version
 *  n:              The version shown as new version
 */

namespace Ankh.Commands
{
    [Command(AnkhCommand.CheckForUpdates, AlwaysAvailable = true)]
    sealed class CheckForUpdates : CommandBase, IComponent
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            // Always available
        }

        static Version _currentVersion;
        static Version GetCurrentVersion(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (_currentVersion != null)
                return _currentVersion;

            IAnkhPackage pkg = context.GetService<IAnkhPackage>();

            if (pkg != null)
                return _currentVersion = pkg.PackageVersion;
            return _currentVersion = typeof(CheckForUpdates).Assembly.GetName().Version;
        }

        static Version GetUIVersion(AnkhContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            IAnkhPackage pkg = context.GetService<IAnkhPackage>();

            if (pkg != null)
                return pkg.UIVersion;

            return GetCurrentVersion(context);
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

            Version version = GetCurrentVersion(e.Context);
            Version vsVersion = e.GetService<IAnkhSolutionSettings>().VisualStudioVersion;
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

                    if (type.Contains("Ethernet") || type.Contains("Wireless"))
                        x ^= ni.GetPhysicalAddress().GetHashCode();
                }
            }
            catch { }

            sb.AppendFormat(CultureInfo.InvariantCulture, "&xx={0}&pc={1}", x, Environment.ProcessorCount);

            try
            {
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP"))
                {
                    if (rk != null)
                    {
                        sb.Append("&dn=");
                        Regex re = new Regex("^[vV]([0-9]+\\.[0-9]+)(\\.[0-9]+)*", RegexOptions.Singleline);
                        bool first = true;
                        HybridCollection<string> vers = new HybridCollection<string>();

                        foreach (string s in rk.GetSubKeyNames())
                        {
                            Match m = re.Match(s);

                            if (m.Success)
                            {
                                string v = m.Groups[1].Value;

                                if (vers.Contains(v))
                                    continue;

                                vers.Add(v);

                                if (first)
                                    first = false;
                                else
                                    sb.Append(',');

                                sb.Append(v);
                            }
                        }
                    }
                }
            }
            catch
            { }

            Uri updateUri = new Uri(sb.ToString());
            WebRequest wr = WebRequest.Create(updateUri);

            HttpWebRequest hwr = wr as HttpWebRequest;

            if (hwr != null)
            {
                hwr.AllowAutoRedirect = true;
                hwr.AllowWriteStreamBuffering = true;
                hwr.UserAgent = string.Format("AnkhSVN/{0} VisualStudio/{1} Windows/{2}", version, vsVersion, osVersion);
            }

            wr.BeginGetResponse(OnResponse, wr);
        }

        static bool? _isDevVersion;
        static private bool IsDevVersion()
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

        private static void ShowUpdate(CommandEventArgs e)
        {
            string[] args;

            try
            {
                args = (string[])e.Argument;
            }
            catch { return; }

            if (args == null || args.Length < 8)
                return;

            string title = args[0], header = args[1], description = args[2], url = args[3],
                urltext = args[4], version = args[5], newVersion = args[6], tag = args[7];

            using (UI.SccManagement.UpdateAvailableDialog uad = new UI.SccManagement.UpdateAvailableDialog())
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
                        uad.newVerLabel.Text = newVersion;
                        uad.curVerLabel.Text = GetUIVersion(e.Context).ToString();
                        uad.versionPanel.Enabled = uad.versionPanel.Visible = true;
                    }

                    if (string.IsNullOrEmpty(tag))
                        uad.sameCheck.Enabled = uad.sameCheck.Visible = false;
                }
                catch
                {
                    return; // Don't throw a visible exception from a background check!
                }

                uad.ShowDialog(e.Context);

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

        private void OnResponse(IAsyncResult ar)
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

                if (body[0] != '<' || body[body.Length - 1] != '>')
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
                string newVersion = NodeText(doc, "/u/i/n") ?? version;

                tag = NodeText(doc, "/u/g");

                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
                {
                    if (!string.IsNullOrEmpty(version))
                    {
                        Version v = new Version(version);

                        if (v <= GetCurrentVersion(Context))
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
                        new string[] { title, header, description, url, urltext, version, newVersion, tag });
                }
            }
            finally
            {
                using (RegistryKey rk = config.OpenUserInstanceKey("UpdateCheck"))
                {
                    object fails = rk.GetValue("Fails", 0);
                    rk.DeleteValue("LastCheck", false);
                    rk.DeleteValue("LastVersion", false);
                    rk.DeleteValue("FailedChecks", false);
                    rk.SetValue("LastCheck", DateTime.UtcNow.Ticks);
                    rk.SetValue("LastVersion", GetCurrentVersion(Context).ToString());
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

                if (value is int)
                {
                    interval = (int)value;

                    if (interval <= 0)
                        return;
                }

                TimeSpan ts = TimeSpan.FromHours(interval);

                value = rk.GetValue("LastVersion");

                if (IsDevVersion() || (value is string && (string)value == GetCurrentVersion(context).ToString()))
                {
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
            }

            context.GetService<IAnkhScheduler>().Schedule(new TimeSpan(0, 0, 20), AnkhCommand.CheckForUpdates);
        }

        private static string NodeText(XmlDocument doc, string xpath)
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

        private IAnkhServiceProvider Context
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
