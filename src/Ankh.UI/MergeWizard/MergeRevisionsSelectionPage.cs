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
using System.Resources;
using System.Text;
using System.Windows.Forms;
using WizardFramework;
using Ankh.Scc;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of <code>WizardPage</code> to enable the user to select
    /// the merge revision(s).
    /// </summary>
    class MergeRevisionsSelectionPage : BasePage
    {
        public const string PAGE_NAME = "Merge Revisions Selection Page";

        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeRevisionsSelectionPage(MergeWizard wizard)
            : base(wizard, new MergeRevisionsSelectionPageControl(), PAGE_NAME)
        {
            IsPageComplete = false;

            Title = Resources.MergeRevisionsSelectionPageTitle;
            this.Message = new WizardMessage(Resources.MergeRevisionsSelectionPageMessage);

            PageControl.WizardPage = this;
            ((MergeRevisionsSelectionPageControl)PageControl).SelectionChanged += new EventHandler<EventArgs>(MergeRevisionsSelectionPage_SelectionChanged);
        }

        /// <summary>
        /// Returns an array of revisions, in numerical order, to be merged.
        /// </summary>
        public long[] MergeRevisions
        {
            get
            {
                List<long> revs = new List<long>();

                foreach (ISvnLogItem item in ((MergeRevisionsSelectionPageControl)PageControl).SelectedRevisions)
                {
                    revs.Add(item.Revision);
                }

                revs.Sort();

                return revs.ToArray();
            }
        }

        void MergeRevisionsSelectionPage_SelectionChanged(object sender, EventArgs e)
        {
            IsPageComplete = ((MergeRevisionsSelectionPageControl)PageControl).SelectedRevisions.Count > 0;

            if (IsPageComplete)
                ((MergeWizard)Wizard).MergeRevisions = MergeRevisions;
            else
                ((MergeWizard)Wizard).MergeRevisions = null;
        }

        protected override void OnPageChanged(WizardPageChangeEventArgs e)
        {
            base.OnPageChanged(e);
        }
    }
}
