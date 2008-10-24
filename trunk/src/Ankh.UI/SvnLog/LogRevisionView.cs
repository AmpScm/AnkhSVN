using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using SharpSvn;
using SharpSvn.Implementation;


using Ankh.Scc;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using System.Drawing;

namespace Ankh.UI.SvnLog
{
    class LogRevisionView : ListViewWithSelection<LogListViewItem>
    {
        public LogRevisionView()
        {
			Sorting = SortOrder.None;
            Init();
        }
        public LogRevisionView(IContainer container)
			: this()
        {
            container.Add(this);
        }

        LogDataSource _dataSource;
        public LogDataSource LogSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        void Init()
        {
            SmartColumn revision = new SmartColumn(this, "&Revision", 64, HorizontalAlignment.Right);
            SmartColumn author = new SmartColumn(this, "&Author", 73);
            SmartColumn date = new SmartColumn(this, "&Date", 118);
            SmartColumn issue = new SmartColumn(this, "&Issue", 60);
            SmartColumn message = new SmartColumn(this, "&Message", 300);

            revision.Sortable = author.Sortable = date.Sortable = issue.Sortable = message.Sortable = false;


            AllColumns.Add(revision);
            AllColumns.Add(author);

            AllColumns.Add(date);
            AllColumns.Add(issue);
            AllColumns.Add(message);

            // The listview can't align the first column right. We switch their display position
            // to work around this            
            Columns.AddRange(
                new ColumnHeader[]
                {
                    author,
                    revision,
                    date,
                    message
                });

            author.DisplayIndex = 1;
            revision.DisplayIndex = 0;
        }

		protected override void OnRetrieveSelection(ListViewWithSelection<LogListViewItem>.RetrieveSelectionEventArgs e)
		{
			e.SelectionItem = new LogItem((LogListViewItem)e.Item, LogSource.RepositoryRoot);
			base.OnRetrieveSelection(e);
		}

		protected override void OnResolveItem(ListViewWithSelection<LogListViewItem>.ResolveItemEventArgs e)
		{
			e.Item = ((LogItem)e.SelectionItem).ListViewItem;
			base.OnResolveItem(e);
		}

    }

    class LogListViewItem : SmartListViewItem
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnLoggingEventArgs _args;
        public LogListViewItem(LogRevisionView listView, IAnkhServiceProvider context, SvnLoggingEventArgs e)
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

		internal SvnChangeItemCollection ChangedPaths
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
		readonly LogListViewItem _lvi;
        public Uri _repositoryRoot;

		public LogItem(LogListViewItem lvi, Uri repositoryRoot)
		{
			if (lvi == null)
				throw new ArgumentNullException("lvi");

			_lvi = lvi;
            _repositoryRoot = repositoryRoot;
		}

		internal LogListViewItem ListViewItem
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
