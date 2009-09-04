using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Ankh.UI.VSSelectionControls;
using System.Windows.Forms;
using Ankh.Scc;

namespace Ankh.UI.IssueTracker
{
    class IssuesView: ListViewWithSelection<IssuesViewItem>
    {
        public IssuesView()
        {
            base.MultiSelect = false;
            Init();
        }

        public IssuesView(IContainer container)
            : this()
        {
            container.Add(this);
        }

        void Init()
        {
            SmartColumn issueId = new SmartColumn(this, "&Issue Id", 100);

            AllColumns.Add(issueId);

            Columns.AddRange(
                new ColumnHeader[]
                {
                    issueId
                });

            SortColumns.Add(issueId);
            FinalSortColumn = issueId;            
        }

        protected override void OnRetrieveSelection(ListViewWithSelection<IssuesViewItem>.RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new IssuesItem(e.Item);
            base.OnRetrieveSelection(e);
        }

        protected override void OnResolveItem(ListViewWithSelection<IssuesViewItem>.ResolveItemEventArgs e)
        {
            e.Item = ((IssuesItem)e.SelectionItem).ListViewItem;
            base.OnResolveItem(e);
        }

        public IEnumerable<string> SelectedIssues
        {
            get
            {
                foreach (ListViewItem lvi in SelectedItems)
                {
                    IssuesViewItem ivi = lvi as IssuesViewItem;
                    yield return ivi.IssueId;
                }
            }
        }

        public string SelectedIssue
        {
            get
            {
                return EnumTools.GetFirst<string>(SelectedIssues);
            }
        }
    }

    sealed class IssuesViewItem : SmartListViewItem
    {
        readonly string _issueId;

        public IssuesViewItem(IssuesView view, string issueId)
            : base(view)
        {
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

    sealed class IssuesItem : AnkhPropertyGridItem
    {
        readonly IssuesViewItem _livi;

        public IssuesItem(IssuesViewItem livi)
        {
            if (livi == null)
                throw new ArgumentNullException("livi");

            _livi = livi;
            _index = livi.Index;
        }

        internal IssuesViewItem ListViewItem
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
