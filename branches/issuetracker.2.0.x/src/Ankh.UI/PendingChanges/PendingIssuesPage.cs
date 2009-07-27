// $Id$
//
// Copyright 2008 The AnkhSVN Project
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            IAnkhIssueService issueService = Context.GetService<IAnkhIssueService>(typeof(IAnkhIssueService));
            if (issueService != null)
            {
                RefreshPageContents();
                issueService.IssueRepositoryChanged += new EventHandler(issueService_IssueRepositoryChanged);
            }
        }

        void issueService_IssueRepositoryChanged(object sender, EventArgs e)
        {
            RefreshPageContents();
        }

        public void RefreshPageContents()
        {
            this.Controls.Clear();
            IAnkhIssueService issueService = Context.GetService<IAnkhIssueService>(typeof(IAnkhIssueService));
            if (issueService != null)
            {
                IIssueRepository repository = issueService.CurrentIssueRepository;
                if (repository != null)
                {
                    Control control = Control.FromHandle(repository.Handle);
                    if (control != null)
                    {
                        control.Dock = DockStyle.Fill;
                        this.Controls.Add(control);
                        return;
                    }
                }
            }
            this.Controls.Add(label1);
        }
    }
}
