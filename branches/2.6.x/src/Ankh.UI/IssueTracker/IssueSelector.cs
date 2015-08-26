using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.VS;

namespace Ankh.UI.IssueTracker
{
    public partial class IssueSelector : VSDialogForm
    {
        IEnumerable<TextMarker> _issueWalker;

        public IssueSelector()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (issuesView1 != null)
            {
                issuesView1.SelectionPublishServiceProvider = Context;
                issuesView1.ColumnWidthChanged += new ColumnWidthChangedEventHandler(issuesView1_ColumnWidthChanged);
                IDictionary<string, int> widths = ConfigurationService.GetColumnWidths(GetType());
                issuesView1.SetColumnWidths(widths);
            }
        }

        protected void issuesView1_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            IDictionary<string, int> widths = issuesView1.GetColumnWidths();
            ConfigurationService.SaveColumnsWidths(GetType(), widths);
        }

        public void LoadIssues(IEnumerable<TextMarker> issueWalker)
        {
            if (issueWalker == null)
                throw new ArgumentNullException("issueWalker");

            _issueWalker = issueWalker;
            Reload();
        }

        private void Reload()
        {
            issuesView1.ClearItems();
            ICollection<string> issues = new List<string>();
            foreach (TextMarker im in _issueWalker)
            {
                // skip duplicate items
                if (!issues.Contains(im.Value))
                {
                    issuesView1.Items.Add(new IssuesViewItem(issuesView1, im.Value));
                    issues.Add(im.Value);
                }
            }
        }

        /// <summary>
        /// Returns the issue id if there is only one issue
        /// </summary>
        /// <param name="issueId"></param>
        public bool IsSingleIssue(out string issueId)
        {
            issueId = null;
            if (issuesView1.Items.Count == 1)
            {
                IssuesViewItem ivi = issuesView1.Items[0] as IssuesViewItem;
                if (ivi != null)
                    issueId = ivi.IssueId;
            }
            return !string.IsNullOrEmpty(issueId);
        }

        public string SelectedIssue
        {
            get
            {
                return issuesView1.SelectedIssue;
            }
        }

        void issuesView1_ItemSelectionChanged(object sender, System.Windows.Forms.ListViewItemSelectionChangedEventArgs e)
        {
            okButton.Enabled = e.IsSelected;
        }
    }
}
