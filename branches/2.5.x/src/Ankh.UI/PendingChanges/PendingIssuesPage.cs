// $Id$
//
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

namespace Ankh.UI.PendingChanges
{
    partial class PendingIssuesPage : PendingChangesPage
    {
        public PendingIssuesPage()
        {
            InitializeComponent();
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

            if (IssueService != null)
            {
                RefreshPageContents();
                IssueService.IssueRepositoryChanged += new EventHandler(issueService_IssueRepositoryChanged);
            }
        }

        public override void OnThemeChanged(EventArgs e)
        {
            base.OnThemeChanged(e);

            if (VSVersion.VS2012OrLater)
                pleaseConfigureLabel.BorderStyle = BorderStyle.None;
        }

        void issueService_IssueRepositoryChanged(object sender, EventArgs e)
        {
            RefreshPageContents();
        }

        public void RefreshPageContents()
        {
            Controls.Clear();

            if (IssueService != null)
            {
                IssueRepository repository = IssueService.CurrentIssueRepository;
                IWin32Window window = null;

                if (repository != null
                    && (window = repository.Window) != null)
                {
                    Control control = Control.FromHandle(window.Handle);
                    if (control != null)
                    {
                        control.Dock = DockStyle.Fill;
                        Controls.Add(control);

                        if (VSVersion.SupportsTheming && Context != null)
                        {
                            IWinFormsThemingService wts = Context.GetService<IWinFormsThemingService>();

                            if (wts != null)
                                wts.ThemeRecursive(control);
                        }
                        return;
                    }
                }
            }
            Controls.Add(pleaseConfigureLabel);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            pleaseConfigureLabel.Font = new Font(Font, FontStyle.Bold);
        }

        private void pleaseConfigureLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (IssueService != null)
                IssueService.ShowConnectHelp();
        }
    }
}
