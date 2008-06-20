using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
{
    public partial class LogViewerDialog : Form
    {
        private string _logTarget;
        private IAnkhServiceProvider _context;

        public LogViewerDialog()
        {
            InitializeComponent();
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
            get { return _context; }
            set { _context = value; }
        }

        /// <summary>
        /// The target of the log.
        /// </summary>
        public string LogTarget
        {
            get { return _logTarget; }
            set { _logTarget = value; }
        }

        #region UI Events
        private void LogViewerDialog_Load(object sender, EventArgs e)
        {
            logViewerControl.Site = Site;
            
            if (LogTarget != null)
                logViewerControl.Start(Context, new string[] { LogTarget });
        }
        #endregion
    }
}
