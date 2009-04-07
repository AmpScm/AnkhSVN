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
using System.Resources;
using System.Text;
using System.Windows.Forms;
using WizardFramework;
using Ankh.Scc;
using SharpSvn;

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

            Title = MergeStrings.MergeRevisionsSelectionPageTitle;
            this.Message = new WizardMessage(MergeStrings.MergeRevisionsSelectionPageMessage);

            PageControl.WizardPage = this;
            ((MergeRevisionsSelectionPageControl)PageControl).SelectionChanged += new EventHandler<EventArgs>(MergeRevisionsSelectionPage_SelectionChanged);
        }

        /// <summary>
        /// Returns an array of revisions, in numerical order, to be merged.
        /// </summary>
        public IEnumerable<SvnRevisionRange> MergeRevisions
        {
            get
            {
                ISvnLogItem start = null;
                ISvnLogItem end = null;
                int previousIndex = -1;
                List<ISvnLogItem> logitems = new List<ISvnLogItem>(((MergeRevisionsSelectionPageControl)PageControl).SelectedRevisions);
                logitems.Sort(delegate(ISvnLogItem a, ISvnLogItem b) { return a.Index.CompareTo(b.Index); });
                
                foreach (ISvnLogItem item in logitems)
                {
                    if (start == null)
                    {
                        start = item;
                        end = item;
                    }
                    else if (previousIndex + 1 == item.Index)
                    {
                        // range is still contiguous, move end ptr
                        end = item;
                    }
                    else
                    {
                        // The start of a new range because it's no longer contiguous
                        // return the previous range and start a new one
                        yield return new SvnRevisionRange(start.Revision - 1, end.Revision);

                        start = item;
                        end = item;
                    }

                    previousIndex = item.Index;
                }

                // The loop doesn't handle the last range
                if (start != null && end != null)
                {
                    yield return new SvnRevisionRange(start.Revision - 1, end.Revision);
                }
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

        protected override void OnPageChanged(WizardPageChangedEventArgs e)
        {
            base.OnPageChanged(e);
        }
    }
}
