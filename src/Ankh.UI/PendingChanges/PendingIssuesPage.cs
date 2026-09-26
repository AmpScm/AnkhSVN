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
using System.Drawing;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.UI.IssueTracker;

namespace Ankh.UI.PendingChanges
{
    partial class PendingIssuesPage : PendingChangesPage
    {
        public PendingIssuesPage()
        {
            InitializeComponent();
            Disposed += PendingIssuesPage_Disposed;
        }

        protected override Type PageType
        {
            get
            {
                return typeof(PendingIssuesPage);
            }
        }

        IAnkhIssueService _issueService;

        IAnkhIssueService IssueService
        {
            get { return _issueService ?? (_issueService = ((Context != null) ? Context.GetService<IAnkhIssueService>() : null)); }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RefreshPageContents();

            if (IssueService != null)
                IssueService.IssueRepositoryChanged += issueService_IssueRepositoryChanged;
        }

        void PendingIssuesPage_Disposed(object sender, EventArgs e)
        {
            if (_issueService != null)
                _issueService.IssueRepositoryChanged -= issueService_IssueRepositoryChanged;
        }

        public override void OnThemeChanged(EventArgs e)
        {
            base.OnThemeChanged(e);

            if (VSVersion.VS2012OrLater)
                pleaseConfigureLabel.BorderStyle = BorderStyle.None;

            // Hosted issue-provider controls are inserted dynamically, so they
            // must be re-themed when Visual Studio changes theme at runtime.
            foreach (Control control in Controls)
                IssueTrackerThemeLogic.ThemeEmbeddedControl(
                    Context,
                    control,
                    false);
        }

        void issueService_IssueRepositoryChanged(object sender, EventArgs e)
        {
            RefreshPageContents();
        }

        public void RefreshPageContents()
        {
            Controls.Clear();

            IAnkhIssueService service = IssueService;
            if (service != null)
            {
                IssueRepository repository = service.CurrentIssueRepository;
                IWin32Window window = null;

                if (repository != null
                    && (window = repository.Window) != null)
                {
                    Control control = Control.FromHandle(window.Handle);
                    if (control != null)
                    {
                        control.Dock = DockStyle.Fill;
                        Controls.Add(control);

                        IssueTrackerThemeLogic.ThemeEmbeddedControl(
                            Context,
                            control,
                            false);
                        return;
                    }
                }

                int connectorCount = service.Connectors == null
                    ? 0
                    : service.Connectors.Count;
                IssueRepositorySettings settings = service.CurrentIssueRepositorySettings;
                bool hasConfiguredRepository =
                    settings != null
                    && !string.IsNullOrEmpty(settings.ConnectorName);

                ShowEmptyState(IssueTrackerAvailabilityLogic.GetEmptyState(
                    connectorCount,
                    hasConfiguredRepository));
            }
            else
            {
                ShowEmptyState(IssueTrackerEmptyState.NoConnectors);
            }

            Controls.Add(pleaseConfigureLabel);
        }

        void ShowEmptyState(IssueTrackerEmptyState state)
        {
            string text = IssueTrackerAvailabilityLogic.GetMessage(state);
            pleaseConfigureLabel.Text = text;

            int linkStart = text.LastIndexOf(
                IssueTrackerAvailabilityLogic.HelpLinkText,
                StringComparison.Ordinal);
            pleaseConfigureLabel.LinkArea = linkStart >= 0
                ? new LinkArea(
                    linkStart,
                    IssueTrackerAvailabilityLogic.HelpLinkText.Length)
                : new LinkArea(0, 0);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            pleaseConfigureLabel.Font = new Font(Font, FontStyle.Bold);
        }

        private void pleaseConfigureLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IAnkhHelpService help =
                Context == null ? null : Context.GetService<IAnkhHelpService>();

            if (help != null)
                help.RunHelp(this);
        }
    }
}
