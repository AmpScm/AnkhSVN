using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WizardFramework;
using System.Threading;
using SharpSvn;

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
        /// Returns whether or not the "Merge From" <code>ComboBox</code>
        /// has content in it.  (Validation will occur during the merge step.)
        /// </summary>
        public bool HasMergeSource
        {
            get
            {
                return mergeFromComboBox.Text != "";
            }
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

                mergeFromComboBox.SelectedItem = ((MergeWizard)WizardPage.Wizard).MergeTarget.Status.Uri.ToString();

                ((WizardDialog)WizardPage.Form).EnablePageAndButtons(true);

                UIUtils.ResizeDropDownForLongestEntry(mergeFromComboBox);

                Cursor.Current = Cursors.Default;

                WizardPage.IsPageComplete = true;
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
                List<string> mergeSources = wizard.MergeUtils.GetSuggestedMergeSources(wizard.MergeTarget, MergeWizard.MergeType.RangeOfRevisions);

                if (mergeSources.Count == 0)
                {
                    using (SvnClient client = wizard.MergeUtils.GetClient())
                    {
                        mergeSources.Add(wizard.MergeTarget.Status.Uri.ToString());
                    }
                }

                SetMergeSources(mergeSources);
            }
        }

        #region UI Events
        private void MergeSourceRangeOfRevisionsPageControl_Load(object sender, EventArgs e)
        {
            mergeFromComboBox.Text = Resources.LoadingMergeSources;
            ((WizardDialog)WizardPage.Form).EnablePageAndButtons(false);
            Cursor.Current = Cursors.WaitCursor;
         
            Thread t = new Thread(new ThreadStart(RetrieveAndSetMergeSources));

            t.Start();
        }

        private void selectRevisionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                ((MergeSourceRangeOfRevisionsPage)WizardPage).NextPageRequired = true;
            }
        }

        private void allRevisionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                ((MergeSourceRangeOfRevisionsPage)WizardPage).NextPageRequired = false;
            }
        }

        private void mergeFromComboBox_TextChanged(object sender, EventArgs e)
        {
            ((WizardDialog)WizardPage.Form).UpdateButtons();
        }

        private void selectButton_Click(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
