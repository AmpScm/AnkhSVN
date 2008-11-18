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
using Microsoft.VisualStudio;
using Ankh.VS;
using Ankh.Scc.UI;
using Ankh.Ids;

namespace Ankh.UI.PendingChanges
{
    public partial class PendingChangesToolControl : AnkhToolWindowControl
    {
        readonly List<PendingChangesPage> _pages;
        readonly PendingCommitsPage _commitsPage;
        readonly PendingIssuesPage _issuesPage;
        readonly RecentChangesPage _changesPage;
        readonly PendingConflictsPage _conflictsPage;
        PendingChangesPage _currentPage;

        public PendingChangesToolControl()
        {
            InitializeComponent();

            _commitsPage = new PendingCommitsPage();
            _issuesPage = new PendingIssuesPage();
            _changesPage = new RecentChangesPage();
            _conflictsPage = new PendingConflictsPage();

            _pages = new List<PendingChangesPage>();
            _pages.Add(_commitsPage);
            _pages.Add(_issuesPage);
            _pages.Add(_changesPage);
            _pages.Add(_conflictsPage);
        }

        protected override void OnLoad(EventArgs e)
        {
            foreach (PendingChangesPage p in _pages)
            {
                p.Context = Context;
                p.ToolControl = this;

                if (!panel1.Controls.Contains(p))
                {
                    p.Enabled = p.Visible = false;
                    p.Dock = DockStyle.Fill;
                    panel1.Controls.Add(p);
                }
            }

            base.OnLoad(e);

            ShowPanel(_commitsPage, false);
        }

        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            ToolWindowHost.CommandContext = AnkhId.PendingChangeContextGuid;
            //ToolWindowSite.KeyboardContext = AnkhId.PendingChangeContextGuid;
            UpdateColors();
            UpdateCaption();
        }

        private void UpdateColors()
        {
            if (Context == null)
                return;

            // We should use the VS colors instead of the ones provided by the OS
            IAnkhVSColor colorSvc = Context.GetService<IAnkhVSColor>();

            Color color;
            if (colorSvc.TryGetColor(__VSSYSCOLOREX.VSCOLOR_TOOLWINDOW_BACKGROUND, out color))
                BackColor = color;

            if (colorSvc.TryGetColor(__VSSYSCOLOREX.VSCOLOR_COMMANDBAR_GRADIENT_MIDDLE, out color))
            {
                pendingChangesTabs.BackColor = color;
                pendingChangesTabs.OverflowButton.BackColor = color;                
            }

            if (colorSvc.TryGetColor(__VSSYSCOLOREX.VSCOLOR_COMMANDBAR_HOVEROVERSELECTED, out color))
            {
                pendingChangesTabs.ForeColor = color;
                pendingChangesTabs.OverflowButton.ForeColor = color;
            }
        }

        void ShowPanel(PendingChangesPage page, bool select)
        {
            if (page == null)
                throw new ArgumentNullException("page");
            else if (page == _currentPage)
                return;

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
            }

            _currentPage = page;

            fileChangesButton.Checked = (page == _commitsPage);
            issuesButton.Checked = (page == _issuesPage);
            recentChangesButton.Checked = (page == _changesPage);
            conflictsButton.Checked = (page == _conflictsPage);

            if(select)
                page.Select();

            if (Context != null)
            {
                IAnkhCommandService cmd = Context.GetService<IAnkhCommandService>();

                if (cmd != null)
                    cmd.UpdateCommandUI(false);

                UpdateCaption();
            }
        }

        void UpdateCaption()
        {
            if (ToolWindowHost != null)
            {
                if (_currentPage == null || string.IsNullOrEmpty(_currentPage.Text))
                    ToolWindowHost.Title = ToolWindowHost.OriginalTitle;
                else
                    ToolWindowHost.Title = ToolWindowHost.OriginalTitle + " - " + _currentPage.Text;
            }
        }

        private void fileChangesButton_Click(object sender, EventArgs e)
        {
            ShowPanel(_commitsPage, true);
        }

        private void issuesButton_Click(object sender, EventArgs e)
        {
            ShowPanel(_issuesPage, true);
        }

        private void recentChangesButton_Click(object sender, EventArgs e)
        {
            ShowPanel(_changesPage, true);
        }

        private void conflictsButton_Click(object sender, EventArgs e)
        {
            ShowPanel(_conflictsPage, true);
        }
    }
}
