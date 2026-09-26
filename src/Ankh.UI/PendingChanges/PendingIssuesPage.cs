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
using Ankh.Commands;
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
                        Control host = CreateTrackerHost(
                            control,
                            repository);
                        Controls.Add(host);

                        IssueTrackerThemeLogic.ThemeEmbeddedControl(
                            Context,
                            host,
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

                IssueTrackerEmptyState state =
                    IssueTrackerAvailabilityLogic.GetEmptyState(
                        connectorCount,
                        hasConfiguredRepository);

                Control emptyHost = CreateEmptyTrackerHost(
                    state,
                    connectorCount > 0);
                Controls.Add(emptyHost);
                IssueTrackerThemeLogic.ThemeEmbeddedControl(
                    Context,
                    emptyHost,
                    false);
                return;
            }

            Control noConnectorHost = CreateEmptyTrackerHost(
                IssueTrackerEmptyState.NoConnectors,
                false);
            Controls.Add(noConnectorHost);
            IssueTrackerThemeLogic.ThemeEmbeddedControl(
                Context,
                noConnectorHost,
                false);
        }

        internal TableLayoutPanel CreateTrackerHost(
            Control providerControl,
            IssueRepository repository)
        {
            if (providerControl == null)
                throw new ArgumentNullException("providerControl");

            FlowLayoutPanel management =
                CreateTrackerManagementBar(repository);

            TableLayoutPanel host = new TableLayoutPanel
            {
                Name = "issueTrackerHost",
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            host.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));
            host.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));
            host.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            management.Dock = DockStyle.Fill;
            providerControl.Dock = DockStyle.Fill;

            host.Controls.Add(management, 0, 0);
            host.Controls.Add(providerControl, 0, 1);
            return host;
        }

        internal TableLayoutPanel CreateEmptyTrackerHost(
            IssueTrackerEmptyState state,
            bool canConfigure)
        {
            ShowEmptyState(state);

            TableLayoutPanel host = new TableLayoutPanel
            {
                Name = "emptyIssueTrackerHost",
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            host.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));
            host.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));
            host.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            pleaseConfigureLabel.Dock = DockStyle.Fill;
            host.Controls.Add(pleaseConfigureLabel, 0, 0);

            FlowLayoutPanel actions = new FlowLayoutPanel
            {
                Name = "emptyIssueTrackerActions",
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(6, 4, 6, 6)
            };

            Button add = new Button
            {
                Name = "addIssueTrackerButton",
                Text = "Add Tracker...",
                AutoSize = true,
                Enabled = canConfigure
            };
            add.Click += changeIssueTrackerButton_Click;
            actions.Controls.Add(add);

            host.Controls.Add(actions, 0, 1);
            return host;
        }

        internal FlowLayoutPanel CreateTrackerManagementBar(
            IssueRepository repository)
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Name = "issueTrackerManagementBar",
                Dock = DockStyle.Top,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(6, 4, 6, 4)
            };

            Label current = new Label
            {
                Name = "currentIssueTrackerLabel",
                AutoSize = true,
                Text = "Tracker: "
                    + (repository == null
                        ? "(none)"
                        : repository.ConnectorName),
                Margin = new Padding(3, 7, 12, 3)
            };

            Button change = new Button
            {
                Name = "changeIssueTrackerButton",
                Text = "Change Tracker...",
                AutoSize = true
            };
            change.Click += changeIssueTrackerButton_Click;

            Button remove = new Button
            {
                Name = "removeIssueTrackerButton",
                Text = "Remove Tracker",
                AutoSize = true,
                Enabled = repository != null
            };
            remove.Click += removeIssueTrackerButton_Click;

            panel.Controls.Add(current);
            panel.Controls.Add(change);
            panel.Controls.Add(remove);
            return panel;
        }

        void changeIssueTrackerButton_Click(object sender, EventArgs e)
        {
            if (Context == null)
                return;

            IAnkhCommandService commands =
                Context.GetService<IAnkhCommandService>();
            if (commands != null)
            {
                commands.ExecCommand(
                    AnkhCommand.SolutionIssueTrackerSetup,
                    true);
            }
        }

        void removeIssueTrackerButton_Click(object sender, EventArgs e)
        {
            if (Context == null)
                return;

            IAnkhIssueService service =
                Context.GetService<IAnkhIssueService>();
            IssueRepository repository =
                service == null ? null : service.CurrentIssueRepository;

            string trackerName = repository == null
                ? "the current issue tracker"
                : repository.ConnectorName;

            DialogResult result = MessageBox.Show(
                this,
                "Remove the AnkhSVN association with "
                    + trackerName
                    + "?\r\n\r\n"
                    + "This will not delete Local SVN Issues data and will "
                    + "not erase standard bugtraq:* properties.",
                "Remove Issue Tracker",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
                IssueTrackerAssociationManager.Apply(Context, null);
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
