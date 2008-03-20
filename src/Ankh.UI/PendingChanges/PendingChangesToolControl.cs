using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Services;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Commands;

namespace Ankh.UI.PendingChanges
{
    public partial class PendingChangesToolControl : UserControl
    {
        PendingCommitsPage _commitsPage;
        PendingIssuesPage _issuesPage;
        RecentChangesPage _changesPage;
        IAnkhUISite _site;
        public PendingChangesToolControl()
        {
            InitializeComponent();

            _commitsPage = new PendingCommitsPage();
            _issuesPage = new PendingIssuesPage();
            _changesPage = new RecentChangesPage();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ShowPanel(_commitsPage);
        }

        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;

                IAnkhUISite site = value as IAnkhUISite;

                if (site != null)
                {
                    _site = site;
                    foreach (PendingChangesPage page in panel1.Controls)
                    {
                        page.UISite = site;
                    }
                }
            }
        }

        [CLSCompliant(false)]
        public IAnkhUISite UISite
        {
            get { return _site; }
        }

        void ShowPanel(PendingChangesPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            bool foundPage = false;
            foreach (PendingChangesPage p in panel1.Controls)
            {
                if (p != page)
                {
                    p.Enabled = p.Visible = false;
                }
                else
                {
                    foundPage = true;
                    p.Enabled = p.Visible = true;
                }
            }

            if (!foundPage)
            {
                panel1.Controls.Add(page);
                page.Dock = DockStyle.Fill;

                if (UISite != null)
                {
                    page.UISite = UISite;
                }
            }

            fileChangesButton.Checked = (page == _commitsPage);
            issuesButton.Checked = (page == _changesPage);
            recentChangesButton.Checked = (page == _issuesPage);

            if (UISite != null)
            {
                IAnkhCommandService cmd = UISite.GetService<IAnkhCommandService>();

                if (cmd != null)
                    cmd.UpdateCommandUI(false);
            }
        }

        private object IAnkhCommandHandler(Type type)
        {
            throw new NotImplementedException();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            ShowPanel(_commitsPage);            
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            ShowPanel(_changesPage);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            ShowPanel(_issuesPage);
        }
    }
}
