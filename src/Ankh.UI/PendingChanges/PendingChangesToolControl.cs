// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
using Ankh.Selection;

namespace Ankh.UI.PendingChanges
{
    public partial class PendingChangesToolControl : AnkhToolWindowControl, IAnkhHasVsTextView
    {
        readonly List<PendingChangesPage> _pages;
        readonly PendingActivationPage _activatePage;
        readonly PendingCommitsPage _commitsPage;
        readonly PendingIssuesPage _issuesPage;
        readonly RecentChangesPage _changesPage;
        readonly PendingConflictsPage _conflictsPage;
        PendingChangesPage _currentPage;
        PendingChangesPage _lastPage;

        public PendingChangesToolControl()
        {
            InitializeComponent();

            _activatePage = new PendingActivationPage();
            _commitsPage = new PendingCommitsPage();
            _issuesPage = new PendingIssuesPage();
            _changesPage = new RecentChangesPage();
            _conflictsPage = new PendingConflictsPage();

            _pages = new List<PendingChangesPage>();
            _pages.Add(_activatePage);
            _pages.Add(_commitsPage);
            _pages.Add(_issuesPage);
            _pages.Add(_changesPage);
            _pages.Add(_conflictsPage);

            fileChangesButton.Image = VSVersion.VS2012OrLater ? PCResources.SourceFiles : PCResources.SourceFilesOld;
            issuesButton.Image = VSVersion.VS2012OrLater ? PCResources.Issues : PCResources.IssuesOld;
            recentChangesButton.Image = VSVersion.VS2012OrLater ? PCResources.RecentChanges : PCResources.RecentChangesOld;
            conflictsButton.Image = VSVersion.VS2012OrLater ? PCResources.Conflicts : PCResources.ConflictsOld;

            ApplyNavigationLayout();

            pendingChangesTabs.BackColorChanged += delegate { RefreshNavigationIcons(); };
            pendingChangesTabs.DpiChangedAfterParent += delegate
            {
                ApplyNavigationLayout();
                RefreshNavigationIcons();
            };
            Disposed += delegate { DisposeNavigationIcons(); };
        }

        protected override void OnLoad(EventArgs e)
        {
            foreach (PendingChangesPage p in _pages)
            {
                p.Context = Context;
                p.ToolControl = this;

                if (!contentPanel.Controls.Contains(p))
                {
                    p.Enabled = p.Visible = false;
                    p.Dock = DockStyle.Fill;
                    contentPanel.Controls.Add(p);
                }
            }

            base.OnLoad(e);

            AnkhServiceEvents ev = Context.GetService<AnkhServiceEvents>();

            ev.SccProviderActivated += new EventHandler(OnSccProviderActivated);
            ev.SccProviderDeactivated += new EventHandler(OnSccProviderDeactivated);

            IAnkhCommandStates states = Context.GetService<IAnkhCommandStates>();

            bool shouldActivate = false;

            if (states != null)
            {
                if (!states.UIShellAvailable)
                {
                    ev.UIShellActivate += new EventHandler(OnSccShellActivate);
                    shouldActivate = false;
                }
                else
                    shouldActivate = states.SccProviderActive;
            }

            _lastPage = _commitsPage;

            ShowPanel(shouldActivate ? _lastPage : _activatePage, false);
            pendingChangesTabs.Enabled = shouldActivate;
        }

        protected override void OnThemeChanged(EventArgs e)
        {
            base.OnThemeChanged(e);

            ToolStripRenderer renderer = null;
            System.Windows.Forms.Design.IUIService ds = Context.GetService<System.Windows.Forms.Design.IUIService>();
            if (ds != null)
            {
                renderer = ds.Styles["VsRenderer"] as ToolStripRenderer;
            }

            if (renderer != null)
                pendingChangesTabs.Renderer = renderer;

            foreach (PendingChangesPage p in _pages)
            {
                p.OnThemeChanged(e);
            }

            RefreshNavigationIcons();
            pendingChangesTabs.Invalidate();
        }

        void OnSccShellActivate(object sender, EventArgs e)
        {
            RefreshNavigationIcons();
            IAnkhCommandStates states = Context.GetService<IAnkhCommandStates>();

            if (states != null && states.SccProviderActive)
            {
                OnSccProviderActivated(sender, e);
            }

        }

        void OnSccProviderDeactivated(object sender, EventArgs e)
        {
            // Visual Studio can briefly raise provider deactivation while other
            // source-control state is being refreshed. Disabling the whole
            // navigation strip synchronously can strand Pending Changes on the
            // current page until the tool window is recreated.
            //
            // Defer one UI turn so command state has settled, then trust the
            // actual SccProviderActive value instead of the transient event.
            if (IsHandleCreated && !IsDisposed && !Disposing)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    ApplySccProviderState(false);
                });
            }
            else
            {
                ApplySccProviderState(false);
            }
        }

        void OnSccProviderActivated(object sender, EventArgs e)
        {
            ApplySccProviderState(true);
        }

        void ApplySccProviderState(bool eventSaysActive)
        {
            bool active = eventSaysActive;

            if (Context != null)
            {
                IAnkhCommandStates states =
                    Context.GetService<IAnkhCommandStates>();

                if (states != null && states.UIShellAvailable)
                    active = states.SccProviderActive;
            }

            ApplyNavigationProviderState(active);
        }

        internal void ApplyNavigationProviderState(bool active)
        {
            if (active)
            {
                pendingChangesTabs.Enabled = true;

                if (_currentPage == _activatePage && _lastPage != null)
                    ShowPanel(_lastPage, false);
            }
            else
            {
                _activatePage.ShowMessage = true;
                ShowPanel(_activatePage, false);
                pendingChangesTabs.Enabled = false;
            }
        }

        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            ToolWindowHost.CommandContext = AnkhId.PendingChangeContextGuid;
            //ToolWindowSite.KeyboardContext = AnkhId.PendingChangeContextGuid;
            UpdateCaption();
        }

        void ShowPanel(PendingChangesPage page, bool select)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (page == _currentPage)
            {
                page.Enabled = page.Visible = true;
                page.BringToFront();
                UpdateNavigationChecks();

                if (select)
                    page.Select();
                return;
            }

            bool foundPage = false;
            foreach (PendingChangesPage p in contentPanel.Controls)
            {
                if (p != page)
                {
                    p.Enabled = p.Visible = false;
                }
                else
                {
                    foundPage = true;
                    p.Enabled = p.Visible = true;

                    if (!p._alreadyThemed)
                    {
                        ThemePage(p);
                    }
                }
            }

            System.Diagnostics.Debug.Assert(foundPage);

            _currentPage = page;

            if (page != _activatePage)
                _lastPage = page;

            page.BringToFront();
            UpdateNavigationChecks();

            if (select)
                page.Select();

            if (Context != null)
            {
                IAnkhCommandService cmd = Context.GetService<IAnkhCommandService>();

                if (cmd != null)
                    cmd.UpdateCommandUI(false);

                UpdateCaption();
            }
        }

        void UpdateNavigationChecks()
        {
            fileChangesButton.Checked = (_lastPage == _commitsPage);
            issuesButton.Checked = (_lastPage == _issuesPage);
            recentChangesButton.Checked = (_lastPage == _changesPage);
            conflictsButton.Checked = (_lastPage == _conflictsPage);
        }

        private void ThemePage(PendingChangesPage page)
        {
            page._alreadyThemed = true;

            if (VSVersion.VS2012OrLater && Context != null)
            {
                IWinFormsThemingService wts = Context.GetService<IWinFormsThemingService>();

                if (wts != null)
                    wts.ThemeRecursive(page, false);
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

        static bool _vertical;
        protected override void OnFrameSize(FrameEventArgs e)
        {
            Size sz = e.Location.Size;

            if (sz.Height > 50 && sz.Width > 50)
            {
                if (!_vertical && sz.Height > sz.Width * 5 / 4)
                {
                    _vertical = true;
                    ChangeOrientation();
                }
                else if (_vertical && sz.Width > sz.Height * 5 / 4)
                {
                    _vertical = false;
                    ChangeOrientation();
                }
            }

            base.OnFrameSize(e);
        }

        internal static bool ShownVertically
        {
            get { return _vertical; }
        }

        private void ChangeOrientation()
        {
            if (_vertical)
                pendingChangesTabs.Dock = DockStyle.Bottom;
            else
                pendingChangesTabs.Dock = DockStyle.Left;

            ApplyNavigationLayout();

            if (Context != null)
            {
                IVsUIShell ui = Context.GetService<IVsUIShell>(typeof(SVsUIShell));
                if (ui != null)
                    ui.UpdateCommandUI(0);
            }
        }

        void ApplyNavigationLayout()
        {
            // Keep the navigation rail visually aligned with modern Visual
            // Studio tool-window chrome. The strip must be wide enough for the
            // rendered glyph plus both the item and outer insets; otherwise the
            // checked-state border clips the right edge at common DPI scales.
            int dpi = pendingChangesTabs.DeviceDpi;
            int outerInset = ScaleNavigationMetric(3, dpi);
            int itemInset = ScaleNavigationMetric(2, dpi);
            int itemGap = ScaleNavigationMetric(2, dpi);
            int stripThickness = GetNavigationStripThickness(dpi);

            pendingChangesTabs.AutoSize = false;
            pendingChangesTabs.Padding = new Padding(outerInset);

            bool horizontal = pendingChangesTabs.Dock == DockStyle.Bottom;
            if (horizontal)
                pendingChangesTabs.Height = stripThickness;
            else
                pendingChangesTabs.Width = stripThickness;

            Padding itemMargin = horizontal
                ? new Padding(itemGap, 0, itemGap, 0)
                : new Padding(0, itemGap, 0, itemGap);

            foreach (ToolStripItem item in pendingChangesTabs.Items)
            {
                item.Padding = new Padding(itemInset);
                item.Margin = itemMargin;
            }
        }

        internal static int GetNavigationStripThickness(int dpi)
        {
            return ScaleNavigationMetric(36, dpi);
        }

        static int ScaleNavigationMetric(int logicalPixels, int dpi)
        {
            if (dpi <= 0)
                dpi = 96;

            return Math.Max(1, (logicalPixels * dpi + 48) / 96);
        }

        #region IAnkhHasVsTextView Members
        Microsoft.VisualStudio.TextManager.Interop.IVsTextView IAnkhHasVsTextView.TextView
        {
            get { return _commitsPage.TextView; }
        }

        Microsoft.VisualStudio.TextManager.Interop.IVsFindTarget IAnkhHasVsTextView.FindTarget
        {
            get { return _commitsPage.FindTarget; }
        }

        #endregion
    }
}
