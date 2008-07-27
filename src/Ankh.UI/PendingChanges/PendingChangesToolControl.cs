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

namespace Ankh.UI.PendingChanges
{
    public partial class PendingChangesToolControl : AnkhToolWindowControl
    {
        PendingCommitsPage _commitsPage;
        PendingIssuesPage _issuesPage;
        RecentChangesPage _changesPage;
        PendingConflictsPage _conflictsPage;
        PendingChangesPage _currentPage;
        
        IAnkhUISite _site;
        public PendingChangesToolControl()
        {
            InitializeComponent();

            _commitsPage = new PendingCommitsPage();
            _issuesPage = new PendingIssuesPage();
            _changesPage = new RecentChangesPage();
            _conflictsPage = new PendingConflictsPage();
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

                    UpdateColors();
                    UpdateCaption();
                }
            }
        }

        private void UpdateColors()
        {
            if (UISite == null)
                return;

            // We should use the VS colors instead of the ones provided by the OS
            IAnkhVSColor colorSvc = UISite.GetService<IAnkhVSColor>();
            
            Color color;
            if (colorSvc.TryGetColor(__VSSYSCOLOREX.VSCOLOR_TOOLWINDOW_BACKGROUND, out color))
                BackColor = color;

            if(colorSvc.TryGetColor(__VSSYSCOLOREX.VSCOLOR_COMMANDBAR_GRADIENT_MIDDLE, out color))
                pendingChangesTabs.BackColor = color;

            if(colorSvc.TryGetColor(__VSSYSCOLOREX.VSCOLOR_COMMANDBAR_HOVEROVERSELECTED, out color))
                pendingChangesTabs.ForeColor = color;
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

                if (UISite != null)
                {
                    page.UISite = UISite;
                }
            }

            _currentPage = page;

            fileChangesButton.Checked = (page == _commitsPage);
            issuesButton.Checked = (page == _issuesPage);
            recentChangesButton.Checked = (page == _changesPage);
            conflictsButton.Checked = (page == _conflictsPage);
            page.Select();

            if (UISite != null)
            {
                IAnkhCommandService cmd = UISite.GetService<IAnkhCommandService>();

                if (cmd != null)
                    cmd.UpdateCommandUI(false);

                UpdateCaption();
            }
        }

        void UpdateCaption()
        {
            if (UISite != null)
            {
                if (_currentPage == null || string.IsNullOrEmpty(_currentPage.Text))
                    UISite.Title = UISite.OriginalTitle;
                else
                    UISite.Title = UISite.OriginalTitle + " - " + _currentPage.Text;
            }
        }

        private void fileChangesButton_Click(object sender, EventArgs e)
        {
            ShowPanel(_commitsPage);
        }

        private void issuesButton_Click(object sender, EventArgs e)
        {
            ShowPanel(_issuesPage);
        }

        private void recentChangesButton_Click(object sender, EventArgs e)
        {
            ShowPanel(_changesPage);
        }

        private void conflictsButton_Click(object sender, EventArgs e)
        {
            ShowPanel(_conflictsPage);
        }
    }
}
