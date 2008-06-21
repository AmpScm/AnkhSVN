using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using Ankh.Scc;

namespace Ankh.UI
{
    public partial class LogViewerDialog : Form, ICurrentItemDestination<ISvnLogItem>
    {
        private string _logTarget;
        private IAnkhServiceProvider _context;

        public LogViewerDialog()
        {
            InitializeComponent();
            ItemSource = logViewerControl;
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

        public IEnumerable<ISvnLogItem> SelectedItems
        {
            get
            {
                if (ItemSource == null)
                    return new ISvnLogItem[0];

                return ItemSource.SelectedItems;
            }
        }

        #region UI Events
        private void LogViewerDialog_Load(object sender, EventArgs e)
        {
            logViewerControl.Site = Site;
            
            if (LogTarget != null)
                logViewerControl.Start(Context, new string[] { LogTarget });
        }
        #endregion

        #region ICurrentItemDestination<ISvnLogItem> Members

        ICurrentItemSource<ISvnLogItem> _itemSource;
        public ICurrentItemSource<ISvnLogItem> ItemSource
        {
            get
            {
                return _itemSource;
            }
            set
            {
                _itemSource = value;
                value.SelectionChanged += new SelectionChangedEventHandler<ISvnLogItem>(value_SelectionChanged);
            }
        }
        #endregion

        void value_SelectionChanged(object sender, IList<ISvnLogItem> e)
        {
        }
    }
}
