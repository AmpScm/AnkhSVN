using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using SharpSvn;
using Ankh.UI.VSSelectionControls;
using System.Globalization;
using System.Drawing;
using Ankh.Scc;
using SharpSvn.Implementation;
using System.Collections.ObjectModel;

namespace Ankh.UI.SvnLog
{
    class LogRevisionItem : SmartListViewItem
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnLoggingEventArgs _args;
        public LogRevisionItem(LogRevisionView listView, IAnkhServiceProvider context, SvnLoggingEventArgs e)
            : base(listView)
        {
            _args = e;
            _context = context;
            RefreshText();
            UpdateColors();
        }

        void RefreshText()
        {
            SetValues(
                Revision.ToString(CultureInfo.CurrentCulture),
                Author,
                Date.ToString(CultureInfo.CurrentCulture),
                "", // Issue
                LogMessage);
        }

        void UpdateColors()
        {
            if (SystemInformation.HighContrast)
                return;

            if (_args.ChangedPaths == null)
                return;

            foreach (SvnChangeItem ci in _args.ChangedPaths)
            {
                if (ci.CopyFromRevision >= 0)
                    ForeColor = Color.DarkBlue;
            }
        }

        internal DateTime Date
        {
            get { return _args.Time.ToLocalTime(); }
        }

        internal string Author
        {
            get { return _args.Author; }
        }

        string _logMessage;
        internal string LogMessage
        {
            get
            {
                if (_logMessage == null && _args.LogMessage != null)
                {
                    // Don't show carriage return linefeed in the listview
                    string[] lines = _args.LogMessage.Split('\r', '\n');
                    foreach (string line in lines)
                    {
                        if (line.Trim().Length > 0)
                        {
                            _logMessage = line;
                            break;
                        }
                    }
                }
                return _logMessage;
            }
        }

        internal long Revision
        {
            get { return _args.Revision; }
        }

        internal KeyedCollection<string, SvnChangeItem> ChangedPaths
        {
            get { return _args.ChangedPaths; }
        }

        internal SvnLoggingEventArgs RawData
        {
            get { return _args; }
        }
    }

    sealed class LogItem : AnkhPropertyGridItem, ISvnLogItem
    {
        readonly LogRevisionItem _lvi;
        public Uri _repositoryRoot;

        public LogItem(LogRevisionItem lvi, Uri repositoryRoot)
        {
            if (lvi == null)
                throw new ArgumentNullException("lvi");

            _lvi = lvi;
            _repositoryRoot = repositoryRoot;
        }

        internal LogRevisionItem ListViewItem
        {
            get { return _lvi; }
        }

        /// <summary>
        /// Gets the repository root.
        /// </summary>
        /// <value>The repository root.</value>
        [Browsable(false)]
        public Uri RepositoryRoot
        {
            get { return _repositoryRoot; }
        }

        [Category("Subversion")]
        [DisplayName("Commit date")]
        public DateTime CommitDate
        {
            get { return _lvi.Date.ToLocalTime(); }
        }

        [Category("Subversion")]
        public string Author
        {
            get { return _lvi.Author; }
        }

        [Category("Subversion")]
        [DisplayName("Log message")]
        public string LogMessage
        {
            get { return _lvi.RawData.LogMessage; }
        }

        [Category("Subversion")]
        public long Revision
        {
            get { return _lvi.Revision; }
        }

        [Browsable(false)]
        public SvnChangeItemCollection ChangedPaths
        {
            get { return _lvi.RawData.ChangedPaths; }
        }

        /// <summary>
        /// Gets the light/second name shown in the gridview header
        /// </summary>
        /// <value></value>
        protected override string ClassName
        {
            get { return "Revision Information"; }
        }

        /// <summary>
        /// Gets the bold/first name shown in the gridview header
        /// </summary>
        /// <value></value>
        protected override string ComponentName
        {
            get { return string.Format("r{0}", Revision); }
        }
    }
}
