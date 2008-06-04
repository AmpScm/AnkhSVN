using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Windows.Forms;
using System.Threading;
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// This is a base class that all merge shources should extend for the base
    /// of the wizard page UI.  This base class handles the merge sources population
    /// and the enablement to go to the next page and/or finish the wizard.
    /// </summary>
    public class MergeSourceBasePageControl<TControl> : MergeSourceBasePageControlImpl
        where TControl : MergeSourceBasePageControl<TControl>, new()
    {
        MergeSourceBasePage<TControl> _wizardPage;

        /// <summary>
        /// Gets/Sets the wizard page associated with this UserControl.
        /// </summary>
        public MergeSourceBasePage<TControl> WizardPage
        {
            get { return _wizardPage; }
            set { _wizardPage = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);



            if (!DesignMode)
            {
                mergeFromComboBox.TextChanged += new EventHandler(mergeFromComboBox_TextChanged);
                mergeFromComboBox.Text = Resources.LoadingMergeSources;

                ((WizardDialog)WizardPage.Form).EnablePageAndButtons(false);

                Cursor.Current = Cursors.WaitCursor;

                Thread t = new Thread(new ThreadStart(RetrieveAndSetMergeSources));

                t.Start();
            }
        }

        /// <summary>
        /// Returns whether or not the "Merge From" <code>TextBox</code>'s
        /// text is a valid Uri.
        /// </summary>
        public bool IsMergeURLValid
        {
            get
            {
                Uri tmpUri;

                // Do not show an error while the resources are retrieved.
                if (mergeFromComboBox.Text == Resources.LoadingMergeSources)
                    return true;

                if (mergeFromComboBox.Text == "" ||
                    !Uri.TryCreate(mergeFromComboBox.Text, UriKind.Absolute, out tmpUri))
                {
                    WizardPage.Message = MergeUtils.INVALID_FROM_URL;

                    ((WizardDialog)WizardPage.Form).UpdateMessage();

                    return false;
                }
                else
                {
                    if (WizardPage.Message == MergeUtils.INVALID_FROM_URL)
                    {
                        WizardPage.Message = null;

                        ((WizardDialog)WizardPage.Form).UpdateMessage();
                    }

                    return true;
                }
            }
        }

        /// <summary>
        /// Returns the merge type for the associated wizard page.
        /// </summary>
        public MergeWizard.MergeType MergeType
        {
            get { return WizardPage.MergeType; }
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

                    mergeFromComboBox.SelectedItem = WizardPage.Wizard.MergeTarget.Status.Uri.ToString();

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
                MergeWizard wizard = WizardPage.Wizard;
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

        #region UI Events
        /// <summary>
        /// Checks for text in the ComboBox to make sure something is there.
        /// </summary>
        private void mergeFromComboBox_TextChanged(object sender, EventArgs e)
        {
            if (!DesignMode)
                ((WizardDialog)WizardPage.Form).UpdateButtons();
        }
        #endregion
        delegate void SetMergeSourcesCallBack(List<string> mergeSources);
    }
}
