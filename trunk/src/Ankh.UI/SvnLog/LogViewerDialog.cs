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
        private SvnOrigin _logTarget;

        public LogViewerDialog()
        {
            InitializeComponent();
            logViewerControl.SelectionChanged += new SelectionChangedEventHandler<ISvnLogItem>(value_SelectionChanged); ;
        }

        public LogViewerDialog(SvnOrigin target, IAnkhServiceProvider context)
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
        public SvnOrigin LogTarget
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
            logViewerControl.Site = Site;

            if (LogTarget == null)
                throw new InvalidOperationException("Log target is null");

            logViewerControl.StartLog(Context, new SvnOrigin[] { LogTarget }, null, null);
        }
        #endregion


        void value_SelectionChanged(object sender, IList<ISvnLogItem> e)
        {
        }
    }
}
