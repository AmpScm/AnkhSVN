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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WizardFramework;
using SharpSvn;
using Ankh.UI.SvnLog;
using Ankh.Scc;
using Ankh.Scc.UI;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeRevisionsSelectionPage : BasePage, ILogControl
    {
        [Obsolete()]
        public MergeRevisionsSelectionPage()
        {
            InitializeComponent();

            logToolControl1.SelectionChanged += new EventHandler<CurrentItemEventArgs<ISvnLogItem>>(logToolControl1_SelectionChanged);

            logToolControl1.StrictNodeHistory = true;
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
