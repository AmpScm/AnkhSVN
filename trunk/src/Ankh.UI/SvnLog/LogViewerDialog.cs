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

namespace Ankh.UI
{
    public partial class LogViewerDialog : Form
    {
        private string _logTarget;
        private IAnkhServiceProvider _context;

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
        /// The context.
        /// </summary>
        public IAnkhServiceProvider Context
        {
            [DebuggerStepThrough]
            get { return _context; }
            set { _context = value; }
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
            logViewerControl.Site = Site;
            
            if (LogTarget != null)
                logViewerControl.StartLocalLog(Context, new string[] { LogTarget });
        }
        #endregion


        void value_SelectionChanged(object sender, IList<ISvnLogItem> e)
        {
        }
    }
}
