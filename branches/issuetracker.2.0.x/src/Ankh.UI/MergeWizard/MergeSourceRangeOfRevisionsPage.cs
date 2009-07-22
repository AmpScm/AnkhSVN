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
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of a wizard page for retrieving the merge source
    /// information for your typical merge scenario.
    /// </summary>
    class MergeSourceRangeOfRevisionsPage : MergeSourceBasePage
    {
        public const string PAGE_NAME = "Merge Source Range of Revisions";

        /// <summary>
        /// Constructor
        /// </summary>
        public MergeSourceRangeOfRevisionsPage(MergeWizard wizard) 
            : base(wizard, new MergeSourceRangeOfRevisionsPageControl(), PAGE_NAME)
        {
            Title = Resources.MergeSourceHeaderTitle;
            Description = Resources.MergeSourceRangeOfRevisionsPageHeaderMessage;
            Control.Load += new EventHandler(Control_Load);
        }

        void Control_Load(object sender, EventArgs e)
        {
            NextPageRequired = true;
        }

        /// <summary>
        /// Gets/Sets whether or not the next page is required.
        /// </summary>
        public bool NextPageRequired
        {
            get { return _needsNextPage; }
            set
            {
                _needsNextPage = value;

                if (Form != null)
                    ((WizardDialog)Form).UpdateButtons();
            }
        }

        /// <see cref="Ankh.UI.MergeWizard.MergeSourceBasePage" />
        internal override MergeWizard.MergeType MergeType
        {
            get { return MergeWizard.MergeType.RangeOfRevisions; }
        }

        protected override void OnPageChanging(WizardPageChangingEventArgs e)
        {
            base.OnPageChanging(e);

            ((MergeWizard)Wizard).LogMode = Ankh.UI.SvnLog.LogMode.MergesEligible;
        }

        private bool _needsNextPage = false;
    }
}
