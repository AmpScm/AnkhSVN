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

using SharpSvn;

using Ankh.Scc;
using Ankh.Scc.UI;
using Ankh.UI.SvnLog;
using Ankh.UI.WizardFramework;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeRevisionsSelectionPage : BaseWizardPage, ILogControl
    {
		public const string PAGE_NAME = "Merge Revisions Selection Page";

		/// <summary>
		/// Constructor.
		/// </summary>
		public MergeRevisionsSelectionPage()
		{
			Name = PAGE_NAME;
			IsPageComplete = false;

			Text = MergeStrings.MergeRevisionsSelectionPageTitle;
			this.Message = new WizardMessage(MergeStrings.MergeRevisionsSelectionPageMessage);

			SelectionChanged += new EventHandler<EventArgs>(MergeRevisionsSelectionPage_SelectionChanged);
			InitializeComponent();

			logToolControl1.SelectionChanged += new EventHandler<CurrentItemEventArgs<ISvnLogItem>>(logToolControl1_SelectionChanged);

			logToolControl1.StrictNodeHistory = true;
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
				List<ISvnLogItem> logitems = new List<ISvnLogItem>(SelectedRevisions);
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
			IsPageComplete = SelectedRevisions.Count > 0;

			if (IsPageComplete)
				((MergeWizard)Wizard).MergeRevisions = MergeRevisions;
			else
				((MergeWizard)Wizard).MergeRevisions = null;
		}

		protected override void OnPageChanged(WizardPageChangedEventArgs e)
		{
			base.OnPageChanged(e);
		}

        public IList<Ankh.Scc.ISvnLogItem> SelectedRevisions
        {
            get
            {
                return logToolControl1.SelectedItems;
            }
        }

        protected override void OnContextChanged(EventArgs e)
        {
            logToolControl1.Context = Context;
        }

        public event EventHandler<EventArgs> SelectionChanged;

        void logToolControl1_SelectionChanged(object sender, CurrentItemEventArgs<ISvnLogItem> e)
        {
            OnSelectionChanged(EventArgs.Empty);
        }

        void OnSelectionChanged(EventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }

        /// <summary>
        /// Gets or sets the merge source.
        /// </summary>
        /// <value>The merge source.</value>
        public SvnOrigin MergeSource
        {
            get { return ((MergeWizard)Wizard).MergeSource; }
        }

        public SvnOrigin MergeTarget
        {
            get { return new SvnOrigin(((MergeWizard)Wizard).MergeTarget); }
        }

        protected void PopulateUI()
        {
            switch (((MergeWizard)Wizard).LogMode)
            {
                case LogMode.MergesEligible:
                    logToolControl1.IncludeMergedRevisions = false;
                    logToolControl1.StartMergesEligible(Context, MergeTarget, MergeSource.Target);
                    break;
                case LogMode.MergesMerged:
                    logToolControl1.IncludeMergedRevisions = true;
                    logToolControl1.StartMergesMerged(Context, MergeTarget, MergeSource.Target);
                    break;
                case LogMode.Log:
                    logToolControl1.StartLog(new SvnOrigin[] { new SvnOrigin(Context, MergeSource.Target, MergeTarget.RepositoryRoot) }, null, null);
                    break;
            }
        }

        private void WizardDialog_PageChangeEvent(object sender, WizardPageChangedEventArgs e)
        {
            if (e.CurrentPage == this)
            {
                PopulateUI();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ((MergeWizard)Wizard).WizardDialog.PageChanged += new EventHandler<WizardPageChangedEventArgs>(WizardDialog_PageChangeEvent);
        }

        private void logToolControl1_BatchFinished(object sender, BatchFinishedEventArgs e)
        {
            if (e.TotalCount == 0)
            {
                Message = new WizardMessage(MergeStrings.NoLogItems, WizardMessage.MessageType.Error);
            }
            else
            {
                Message = new WizardMessage("", WizardMessage.MessageType.None);
            }
        }

        #region ILogControl Members

        bool ILogControl.ShowChangedPaths
        {
            get { return logToolControl1.ShowChangedPaths; }
            set { logToolControl1.ShowChangedPaths = value; }
        }

        bool ILogControl.ShowLogMessage
        {
            get { return logToolControl1.ShowLogMessage; }
            set { logToolControl1.ShowLogMessage = value; }
        }

        bool ILogControl.StrictNodeHistory
        {
            get { return logToolControl1.StrictNodeHistory; }
            set { logToolControl1.StrictNodeHistory = value; }
        }

        bool ILogControl.IncludeMergedRevisions
        {
            get { return logToolControl1.IncludeMergedRevisions; }            
            set { logToolControl1.IncludeMergedRevisions = value; }
        }

        void ILogControl.FetchAll()
        {
            logToolControl1.FetchAll();
        }

        void ILogControl.Restart()
        {
            logToolControl1.Restart();
        }

        IList<SvnOrigin> ILogControl.Origins
        {
            get { return new SvnOrigin[] { MergeSource }; }
        }

        #endregion
    }
}
