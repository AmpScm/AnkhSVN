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

namespace Ankh.UI.MergeWizard
{
    public partial class MergeRevisionsSelectionPageControl : BasePageControl
    {
        public MergeRevisionsSelectionPageControl()
        {
            InitializeComponent();
            logToolControl1.SelectionChanged += new SelectionChangedEventHandler<Ankh.Scc.ISvnLogItem>(logToolControl1_SelectionChanged);
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
        public string MergeSource
        {
            get { return ((MergeWizard)WizardPage.Wizard).MergeSource; }
        }

        public string MergeTarget
        {
            get { return ((MergeWizard)WizardPage.Wizard).MergeTarget.FullPath; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            logToolControl1.Site = Site;
            switch(((MergeWizard)WizardPage.Wizard).LogMode)
            {
                case LogMode.MergesEligible:
                    logToolControl1.StartMergesEligible(WizardPage.Context, MergeTarget, new Uri(MergeSource));
                    break;
                case LogMode.MergesMerged:
                    logToolControl1.StartMergesMerged(WizardPage.Context, MergeTarget, new Uri(MergeSource));
                    break;
                case LogMode.Remote:
                    logToolControl1.StartRemoteLog(WizardPage.Context, new Uri(MergeSource));
                    break;
            }
        }
    }
}
