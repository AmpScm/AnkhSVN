using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using Ankh.Scc;
using System.Diagnostics;
using Ankh.UI.SvnLog;
using System.IO;

namespace Ankh.UI
{
    public partial class LogViewerDialog : VSContainerForm
    {
        private string _logTarget;

        public LogViewerDialog()
        {
            InitializeComponent();
            logViewerControl.SelectionChanged += new SelectionChangedEventHandler<ISvnLogItem>(value_SelectionChanged); ;
        }

        public LogViewerDialog(string target, IAnkhServiceProvider context)
            : this()
        {
            LogTarget = target;
            Context = context;
        }

        /// <summary>
        /// Gets an instance of the <code>LogControl</code>.
        /// </summary>
        internal LogControl LogControl
        {
            get { return logViewerControl; }
        }

        /// <summary>
        /// The target of the log.
        /// </summary>
        public string LogTarget
        {
            [DebuggerStepThrough]
            get { return _logTarget; }
            set { _logTarget = value; }
        }

        public IEnumerable<ISvnLogItem> SelectedItems
        {
            get { return logViewerControl.SelectedItems; }
        }

        #region UI Events
        private void LogViewerDialog_Load(object sender, EventArgs e)
        {
            Uri uri;

            logViewerControl.Site = Site;

            if (LogTarget != null && File.Exists(LogTarget) || Directory.Exists(LogTarget))
            {
                logViewerControl.StartLog(Context, new SvnOrigin[] { new SvnOrigin(Context.GetService<IFileStatusCache>()[LogTarget]) }, null, null);
            }
            else
            {

                if (!Uri.TryCreate(LogTarget, UriKind.Absolute, out uri))
                {
                    MessageBox.Show("Invalid URL", "'" + LogTarget + "' is not a valid url.",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                if (LogTarget != null)
                    logViewerControl.StartLog(Context, new SvnOrigin[] { new SvnOrigin(Context, uri, null) }, null, null);
            }
                //logViewerControl.StartLocalLog(Context, new string[] { LogTarget });
        }
        #endregion


        void value_SelectionChanged(object sender, IList<ISvnLogItem> e)
        {
        }
    }
}
