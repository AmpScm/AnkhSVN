using Ankh.UI.VSSelectionControls;
using Ankh.Scc;
using System.ComponentModel;
using System.Windows.Forms;
using System;

namespace Ankh.UI.SvnLog
{
    class LogIssuesView : ListViewWithSelection<LogIssuesViewItem>
    {
        public LogIssuesView()
        {
            base.MultiSelect = false;
            Init();
        }

        public LogIssuesView(IContainer container)
            : this()
        {
            container.Add(this);
        }

        LogDataSource _dataSource;
        public LogDataSource DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        void Init()
        {
            SmartColumn issueId = new SmartColumn(this, "&Issue Id", 60);

            AllColumns.Add(issueId);

            Columns.AddRange(
                new ColumnHeader[]
                {
                    issueId
                });

            SortColumns.Add(issueId);
            FinalSortColumn = issueId;            
        }

        protected override void OnRetrieveSelection(ListViewWithSelection<LogIssuesViewItem>.RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new LogIssuesItem(e.Item);
            base.OnRetrieveSelection(e);
        }

        protected override void OnResolveItem(ListViewWithSelection<LogIssuesViewItem>.ResolveItemEventArgs e)
        {
            e.Item = ((LogIssuesItem)e.SelectionItem).ListViewItem;
            base.OnResolveItem(e);
        }
    }

    sealed class LogIssuesViewItem : SmartListViewItem
    {
        readonly string _issueId;
        readonly ISvnLogItem _logItem;

        public LogIssuesViewItem(LogIssuesView view, ISvnLogItem logItem, string issueId)
            : base(view)
        {
            _logItem = logItem;
            _issueId = issueId;
            RefreshText();
        }

        public string IssueId
        {
            get { return _issueId; }
        }

        void RefreshText()
        {
            SetValues(IssueId);
        }
    }

    sealed class LogIssuesItem : AnkhPropertyGridItem
    {
        readonly LogIssuesViewItem _livi;

        public LogIssuesItem(LogIssuesViewItem livi)
        {
            if (livi == null)
                throw new ArgumentNullException("livi");

            _livi = livi;
            _index = livi.Index;
        }

        internal LogIssuesViewItem ListViewItem
        {
            get { return _livi; }
        }

        readonly int _index;
        [Browsable(false)]
        public int Index
        {
            get { return _index; }
        }

        [Category("Subversion")]
        [DisplayName("Issue Id")]
        public string Issue
        {
            get { return _livi.IssueId; }
        }

        /// <summary>
        /// Gets the light/second name shown in the gridview header
        /// </summary>
        /// <value></value>
        protected override string ClassName
        {
            get { return "Issue Information"; }
        }

        /// <summary>
        /// Gets the bold/first name shown in the gridview header
        /// </summary>
        /// <value></value>
        protected override string ComponentName
        {
            get { return string.Format("Issue {0}", Issue); }
        }
    }
}
