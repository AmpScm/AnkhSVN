using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Resources;

using SharpSvn;
using WizardFramework;
using System.Threading;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// This is a base class that all merge shources should extend for the base
    /// of the wizard page UI.  This base class handles the merge sources population
    /// and the enablement to go to the next page and/or finish the wizard.
    /// </summary>
    public partial class MergeSourceBasePageControl : UserControl
    {
        MergeSourceBasePage _wizardPage;
        delegate void SetMergeSourcesCallBack(List<string> mergeSources);

        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeSourceBasePageControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enables/Disables the Select button.
        /// </summary>
        public void EnableSelectButton(bool enabled)
        {
            selectButton.Enabled = enabled;
            selectButton.Visible = enabled;
        }

        #region Base Functionality
        /// <summary>
        /// Returns the merge type for the associated wizard page.
        /// </summary>
        public MergeWizard.MergeType MergeType
        {
            get { return WizardPage.MergeType; }
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
        public MergeSourceBasePage WizardPage
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

                if (mergeSources.Count != 0)
                {
                    mergeFromComboBox.DataSource = mergeSources;

                    mergeFromComboBox.SelectedItem = ((MergeWizard)WizardPage.Wizard).MergeTarget.Status.Uri.ToString();

                    UIUtils.ResizeDropDownForLongestEntry(mergeFromComboBox);

                    WizardPage.IsPageComplete = true;
                }
                else if (MergeType == MergeWizard.MergeType.ManuallyRemove)
                {
                    WizardPage.Message = new WizardMessage(Resources.NoRevisionsToUnblock, WizardMessage.ERROR);

                    ((WizardDialog)WizardPage.Form).UpdateMessage();

                    WizardPage.IsPageComplete = false;
                }

                ((WizardDialog)WizardPage.Form).EnablePageAndButtons(true);

                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Retrieves the merge sources and adds them to the <code>ComboBox</code>.
        /// </summary>
        private void RetrieveAndSetMergeSources()
        {
            if (WizardPage != null && WizardPage.Wizard is MergeWizard)
            {
                MergeWizard wizard = (MergeWizard)WizardPage.Wizard;
                List<string> mergeSources = wizard.MergeUtils.GetSuggestedMergeSources(wizard.MergeTarget, MergeType);

                if (mergeSources.Count == 0 && MergeType != MergeWizard.MergeType.ManuallyRemove)
                {
                    using (SvnClient client = wizard.MergeUtils.GetClient())
                    {
                        mergeSources.Add(wizard.MergeTarget.Status.Uri.ToString());
                    }
                }

                SetMergeSources(mergeSources);
            }
        }
        #endregion

        #region UI Events
        /// <summary>
        /// Loads the necessary suggested merge sources as part of the OnLoad event.
        /// </summary>
        private void MergeSourceBasePageControl_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                mergeFromComboBox.Text = Resources.LoadingMergeSources;

                ((WizardDialog)WizardPage.Form).EnablePageAndButtons(false);

                Cursor.Current = Cursors.WaitCursor;

                Thread t = new Thread(new ThreadStart(RetrieveAndSetMergeSources));

                t.Start();
            }
        }

        /// <summary>
        /// Checks for text in the ComboBox to make sure something is there.
        /// </summary>
        private void mergeFromComboBox_TextChanged(object sender, EventArgs e)
        {
            if (!DesignMode)
                ((WizardDialog)WizardPage.Form).UpdateButtons();
        }
        #endregion
    }
}
