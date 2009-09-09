using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Win32;

namespace Ankh
{
	/// <summary>
	/// Summary description for UpdateChecker.
	/// </summary>
	public class UpdateChecker
	{
		private UpdateChecker()
		{
		}


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


		static Version _currentVersion;
		static Version GetCurrentVersion()
		{
			if (_currentVersion != null)
				return _currentVersion;

			return _currentVersion = typeof(UpdateChecker).Assembly.GetName().Version;
		}

		static RegistryKey OpenUpdateCheck()
		{
			return Registry.CurrentUser.CreateSubKey("SOFTWARE\\AnkhSVN\\UpdateCheck");
		}

		static Thread t;
		static void PerformCheck()
		{
			if(t != null)
				return;

			t = new Thread(new ThreadStart(PerformCheckInternal));
			t.Start();
		}

		static void PerformCheckInternal()
		{
			//Thread.Sleep(new TimeSpan(0,0,0,20));
			int interval = 24 * 7; // 1 week
			using (RegistryKey rk = OpenUpdateCheck())
			{
				object value = rk.GetValue("Interval");

				if (value is int)
				{
					interval = (int)value;

					if (interval <= 0)
						return;
				}
			}

			Version version = GetCurrentVersion();
			Version vsVersion = new Version(_context.DTE.Version);
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

			using (RegistryKey rk = OpenUpdateCheck())
			{
				object value = rk.GetValue("CheckKey");

				if (value is int)
				{
					x = (int)value;
				}

				if(x == 0)
				{
					Random r = new Random();
					x = r.Next();
					rk.SetValue("CheckKey", x);
				}
			}
		

			sb.AppendFormat(CultureInfo.InvariantCulture, "&xx={0}", x);

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

		static bool _isDevVersion;
		static bool _hasDevVersionValue;
		static private bool IsDevVersion()
		{
			if (_hasDevVersionValue)
				return _isDevVersion;

			_isDevVersion = true;
			foreach (AssemblyConfigurationAttribute a in typeof(UpdateChecker).Assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false))
			{
				if (a.Configuration != null && a.Configuration.Length > 0)
					_isDevVersion = false;
			}

			_hasDevVersionValue = true;

			return _isDevVersion;
		}
		delegate void ShowUpdateHandler(string title, string header, string description, string url, string urltext, string version, string newVersion, string tag);
		static void ShowUpdate(string title, string header, string description, string url, string urltext, string version, string newVersion, string tag)
		{
			using (Ankh.UI.UpdateAvailableDialog uad = new UI.UpdateAvailableDialog())
			{
				try
				{
					uad.Text = string.Format(uad.Text, title);
					uad.headLabel.Text = header;
					uad.bodyLabel.Text = description;
					uad.linkLabel.Text = urltext;
					uad.linkLabel.Links.Add(0, urltext.Length).LinkData = url;

					if (version != null && version.Length > 0)
					{
						uad.newVerLabel.Text = newVersion;
						uad.curVerLabel.Text = GetCurrentVersion().ToString();
						uad.versionPanel.Enabled = uad.versionPanel.Visible = true;
					}

					if (tag == null || tag.Length == 0)
						uad.sameCheck.Enabled = uad.sameCheck.Visible = false;
				}
				catch
				{
					return; // Don't throw a visible exception from a background check!
				}

				uad.ShowDialog(_context.HostWindow);

				if (uad.sameCheck.Checked)
				{
					using (RegistryKey rk = OpenUpdateCheck())
					{
						rk.SetValue("SkipTag", tag);
					}
				}
			}
		}

		static string UnaryOperator(string first, string second)
		{
			if(first != null)
				return first;
			return second;
		}

		static void OnResponse(IAsyncResult ar)
		{
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

				if (body == null || body.Length == 0)
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
				string header = UnaryOperator(NodeText(doc, "/u/i/h"), title);
				string description = NodeText(doc, "/u/i/d");
				string url = NodeText(doc, "/u/i/u");
				string urltext = NodeText(doc, "/u/i/l");

				string version = NodeText(doc, "/u/i/v");
				string newVersion = UnaryOperator(NodeText(doc, "/u/i/n"), version);

				tag = NodeText(doc, "/u/g");

				if (title != null && title.Length > 0 && description != null && description.Length > 0)
				{
					if (version != null && version.Length > 0)
					{
						Version v = new Version(version);

						if (v <= GetCurrentVersion())
							return;
					}

					if (tag != null && tag.Length > 0)
					{
						using (RegistryKey rk = OpenUpdateCheck())
						{
							string pTag = rk.GetValue("SkipTag") as string;

							if (pTag == tag)
								return;
						}
					}

					_context.UIShell.SynchronizingObject.Invoke(new ShowUpdateHandler(ShowUpdate),
						new string[] { title, header, description, url, urltext, version, newVersion, tag });
				}
			}
			finally
			{
				using (RegistryKey rk = OpenUpdateCheck())
				{
					object fails = rk.GetValue("Fails", 0);
					rk.DeleteValue("LastCheck", false);
					rk.DeleteValue("LastVersion", false);
					rk.DeleteValue("FailedChecks", false);
					rk.SetValue("LastCheck", DateTime.UtcNow.Ticks);
					rk.SetValue("LastVersion", GetCurrentVersion().ToString());
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
			t = null;
		}

		
		static IContext _context;
		static bool _checkedOnce;
		public static void MaybePerformUpdateCheck(IContext context)
		{
			if (_checkedOnce)
				return;

			_checkedOnce = true;

			_context = context;

			using (RegistryKey rk = OpenUpdateCheck())
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

				if (IsDevVersion() || (value is string && (string)value == GetCurrentVersion().ToString()))
				{
					value = rk.GetValue("LastCheck");
					if (value is string )
					{
						DateTime lc = new DateTime(long.Parse((string)value));

						if ((lc + ts) > DateTime.UtcNow)
							return;

						// TODO: Check the number of fails to increase the check interval
					}
				}
			}

			PerformCheck();
		}

		private static string NodeText(XmlDocument doc, string xpath)
		{
			XmlNode node = doc.SelectSingleNode(xpath);

			if (node != null)
				return node.InnerText;

			return null;
		}
	}
}