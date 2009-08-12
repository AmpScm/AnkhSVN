// $Id$
//
// Copyright 2009 The AnkhSVN Project
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

using WizardFramework;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.UI.IssueTracker
{
    public partial class IssueTrackerConfigWizardDialog : WizardDialog
    {
        IAnkhServiceProvider _context;
        IssueRepository _newRepository;

        public IssueTrackerConfigWizardDialog(IAnkhServiceProvider context)
        {
            InitializeComponent();
            _context = context;
            _newRepository = null;
            Wizard = new IssueTrackerConfigWizard(_context);
        }

        public IssueRepository NewIssueRepository
        {
            get { return _newRepository; }
            internal set { _newRepository = value; }
        }

        public IssueRepositorySettings CurrentIssueRepositorySettings
        {
            get
            {
                IAnkhIssueService iService = _context == null ? null : _context.GetService<IAnkhIssueService>();
                return iService == null ? null : iService.CurrentIssueRepositorySettings;
            }
        }
    }
}
