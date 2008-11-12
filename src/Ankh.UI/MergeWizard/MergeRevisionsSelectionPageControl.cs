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

namespace Ankh.UI.MergeWizard
{
    public partial class MergeRevisionsSelectionPageControl : BasePageControl
    {
        public MergeRevisionsSelectionPageControl()
        {
            InitializeComponent();
            logToolControl1.SelectionChanged += new SelectionChangedEventHandler<Ankh.Scc.ISvnLogItem>(logToolControl1_SelectionChanged);

            logToolControl1.StrictNodeHistory = true;
        }

        public IList<Ankh.Scc.ISvnLogItem> SelectedRevisions
        {
            get
            {
                return logToolControl1.SelectedItems;
            }
        }

        public event EventHandler<EventArgs> SelectionChanged;

        void logToolControl1_SelectionChanged(object sender, IList<Ankh.Scc.ISvnLogItem> e)
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
            get { return ((MergeWizard)WizardPage.Wizard).MergeSource; }
        }

        public SvnOrigin MergeTarget
        {
            get { return new SvnOrigin(((MergeWizard)WizardPage.Wizard).MergeTarget); }
        }

        protected void PopulateUI()
        {
            logToolControl1.Site = Site;
            switch (((MergeWizard)WizardPage.Wizard).LogMode)
            {
                case LogMode.MergesEligible:
                    logToolControl1.IncludeMerged = false;
                    logToolControl1.StartMergesEligible(WizardPage.Context, MergeTarget, MergeSource.Target);
                    break;
                case LogMode.MergesMerged:
                    logToolControl1.IncludeMerged = true;
                    logToolControl1.StartMergesMerged(WizardPage.Context, MergeTarget, MergeSource.Target);
                    break;
                case LogMode.Log:
                    logToolControl1.StartLog(WizardPage.Context, new SvnOrigin[] { new SvnOrigin(WizardPage.Context, MergeSource.Target, MergeTarget.RepositoryRoot)}, null, null);
                    break;
            }
        }

        private void WizardDialog_PageChangeEvent(object sender, WizardPageChangeEventArgs e)
        {
            if (e.CurrentPage == WizardPage)
            {
                PopulateUI();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ((MergeWizard)WizardPage.Wizard).WizardDialog.PageChangeEvent += new EventHandler<WizardPageChangeEventArgs>(WizardDialog_PageChangeEvent);
        }

        private void logToolControl1_BatchFinished(object sender, BatchFinishedEventArgs e)
        {
            if (e.TotalCount == 0)
            {
                WizardPage.Message = new WizardMessage(Resources.NoLogItems, WizardMessage.MessageType.ERROR);
            }
            else
            {
                WizardPage.Message = new WizardMessage("", WizardMessage.MessageType.NONE);
            }
        }
    }
}
