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

using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.UI.IssueTracker
{
    public abstract class IssueTrackerWizard : Wizard
    {
        public IssueTrackerWizard(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public override bool PerformFinish()
        {
            IssueRepository newRepository;
            if (TryCreateIssueRepository(out newRepository))
            {
                Container.NewIssueRepository = newRepository;
            }
            Form.DialogResult = System.Windows.Forms.DialogResult.OK;
            return true;
        }

        protected virtual bool TryCreateIssueRepository(out IssueRepository repository)
        {
            repository = null;
            return true;
        }

        public new IssueTrackerConfigWizardDialog Container
        {
            get
            {
                return base.Container as IssueTrackerConfigWizardDialog;
            }
            set
            {
                base.Container = value;
            }
        }


    }
}
