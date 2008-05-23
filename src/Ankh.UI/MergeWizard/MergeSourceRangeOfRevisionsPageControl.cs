using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WizardFramework;
using System.Threading;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeSourceRangeOfRevisionsPageControl : UserControl
    {
        WizardPage _wizardPage;
        delegate void SetMergeSourcesCallBack(List<string> mergeSources);

        public MergeSourceRangeOfRevisionsPageControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets/Sets the wizard page associated with this UserControl.
        /// </summary>
        public WizardPage WizardPage
        {
            get { return _wizardPage; }
            set { _wizardPage = value; }
        }

        /// <summary>
        /// Sets the merge sources for the mergeFromComboBox.
        /// </summary>
        private void SetMergeSources(List<string> mergeSources)
        {
            if (this.InvokeRequired)
            {
                SetMergeSourcesCallBack c = new SetMergeSourcesCallBack(SetMergeSources);

                this.Invoke(c, new object[] { mergeSources });
            }
            else
            {
                mergeFromComboBox.Text = "";
                mergeFromComboBox.DataSource = mergeSources;
                mergeFromComboBox.SelectedIndex = 0;
                mergeFromComboBox.Enabled = true;
                selectButton.Enabled = true;

                UIUtils.ResizeDropDownForLongestEntry(mergeFromComboBox);

                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Retrieves the merge sources and adds them to the <code>ComboBox</code>.
        /// </summary>
        private void RetrieveAndSetMergeSources()
        {
            if (WizardPage.Wizard is MergeWizard)
            {
                MergeWizard wizard = (MergeWizard)WizardPage.Wizard;
                List<string> mergeSources = wizard.MergeUtils.GetSuggestedMergeSources(wizard.MergeTarget);

                if (mergeSources.Count == 0)
                    mergeSources.Add(wizard.MergeTarget.Status.Uri.ToString());

                SetMergeSources(mergeSources);
            }
        }

        #region UI Events
        private void MergeSourceRangeOfRevisionsPageControl_Load(object sender, EventArgs e)
        {
            mergeFromComboBox.Text = Resources.LoadingMergeSources;
            mergeFromComboBox.Enabled = false;
            selectButton.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
         
            Thread t = new Thread(new ThreadStart(RetrieveAndSetMergeSources));

            t.Start();
        }
        #endregion
    }
}
