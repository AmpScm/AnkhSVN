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
using System.Windows.Forms;
using Ankh.UI.SvnLog;
using Ankh.Scc;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeSourceTwoDifferentTreesPageControl : BasePageControl
    {
        public MergeSourceTwoDifferentTreesPageControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the IsPageComplete based on the UI state.
        /// </summary>
        private void TogglePageComplete()
        {
            int tmpInt;
            Uri tmpUri;

            // Working scenarios:
            //    1) From and To HEAD revision RadioButtons are checked.
            //    2) From and To revision TextBoxes have valid revision numbers.
            //    3) From HEAD revision RadioButton is checked and To TextBox has a valid revision number.
            //    4) To HEAD revision RadioButton is checked and From TextBox has a valid revision number.
            //
            // After this, both the From and To URLs have to be valid.

            // Validate the From Url
            if (!Uri.TryCreate(fromURLTextBox.Text, UriKind.Absolute, out tmpUri))
            {
                WizardPage.Message = MergeUtils.INVALID_FROM_URL;
                WizardPage.IsPageComplete = false;
                fromRevisionSelectButton.Enabled = false;

                return;
            }
            
            fromRevisionSelectButton.Enabled = true;

            // Conditionally validate the To Url
            if (!useFromURLCheckBox.Checked && !Uri.TryCreate(toURLTextBox.Text, UriKind.Absolute, out tmpUri))
            {
                WizardPage.Message = MergeUtils.INVALID_TO_URL;
                WizardPage.IsPageComplete = false;
                toRevisionSelectButton.Enabled = false;

                return;
            }
            
            toRevisionSelectButton.Enabled = true;

            // Do not validate the revisions if To and From are using HEAD.
            if (fromHEADRevisionRadioButton.Checked && toHEADRevisionRadioButton.Checked)
            {
                WizardPage.Message = null;
                WizardPage.IsPageComplete = true;

                return;
            }

            // Conditionally validate the From Revision number
            if (fromRevisionRadioButton.Checked && (!Int32.TryParse(fromRevisionTextBox.Text, out tmpInt) || tmpInt < 0))
            {
                WizardPage.Message = MergeUtils.INVALID_FROM_REVISION;
                WizardPage.IsPageComplete = false;

                return;
            }

            // Conditionally validate the To Revision number
            if (toRevisionRadioButton.Checked && (!Int32.TryParse(toRevisionTextBox.Text, out tmpInt) || tmpInt < 0))
            {
                WizardPage.Message = MergeUtils.INVALID_TO_REVISION;
                WizardPage.IsPageComplete = false;

                return;
            }

            WizardPage.Message = null;
            WizardPage.IsPageComplete = true;

            return;
        }

        /// <summary>
        /// Displays the Subversion Log Viewer dialog.
        /// </summary>
        private void DisplayLogViewerAndRetrieveRevisions(object sender)
        {
            string target;
            if (sender == fromRevisionSelectButton || (sender == toRevisionSelectButton && useFromURLCheckBox.Checked))
            {
                target = fromURLTextBox.Text;   
            }
            else if (sender == toRevisionSelectButton)
            {
                target = toURLTextBox.Text;
            }
            else
            {
                return;
            }

            using (LogViewerDialog dialog = new LogViewerDialog(new SvnOrigin(WizardPage.Context, new Uri(target), null)))
            {
                dialog.LogControl.StrictNodeHistory = true;

                if (dialog.ShowDialog(WizardPage.Context) == DialogResult.OK)
                {
                    IEnumerable<ISvnLogItem> selected = dialog.SelectedItems;
                    long low = -1;
                    long high = -1;

                    foreach (ISvnLogItem item in selected)
                    {
                        // Should happen on first iteration
                        if (low == -1 && high == -1)
                        {
                            low = item.Revision;
                            high = item.Revision;

                            continue;
                        }

                        if (item.Revision < low)
                            low = item.Revision;
                        else if (item.Revision > high)
                            high = item.Revision;
                    }

                    fromRevisionTextBox.Text = low.ToString();

                    if (useFromURLCheckBox.Checked && high != -1 && high != low)
                        toRevisionTextBox.Text = high.ToString();
                }
            }
        }

        #region UI Events
        private void useFromURLCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                toURLSelectButton.Enabled = false;
                toURLSelectButton.Visible = false;

                toURLTextBox.Enabled = false;
                toURLTextBox.Visible = false;
                toURLTextBox.Text = "";

                if (WizardPage.Message == MergeUtils.INVALID_TO_URL)
                {
                    WizardPage.Message = null;
                }

                ((MergeSourceTwoDifferentTreesPage)WizardPage).HasSecondMergeSourceUrl = false;
            }
            else
            {
                toURLSelectButton.Enabled = true;
                toURLSelectButton.Visible = true;

                toURLTextBox.Enabled = true;
                toURLTextBox.Visible = true;
                toURLTextBox.Text = fromURLTextBox.Text;
                toURLTextBox.SelectAll();

                ((MergeSourceTwoDifferentTreesPage)WizardPage).HasSecondMergeSourceUrl = true;
            }
        }

        private void fromHeadRevisionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            fromRevisionTextBox.Enabled = false;
            fromRevisionSelectButton.Enabled = false;

            TogglePageComplete();

            ((MergeSourceTwoDifferentTreesPage)WizardPage).MergeFromRevision = -1;
        }

        private void fromRevisionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            fromRevisionTextBox.Enabled = true;
            fromRevisionTextBox.SelectAll();
            fromRevisionSelectButton.Enabled = true;

            TogglePageComplete();
        }

        private void toHEADRevisionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            toRevisionTextBox.Enabled = false;
            toRevisionSelectButton.Enabled = false;

            TogglePageComplete();

            ((MergeSourceTwoDifferentTreesPage)WizardPage).MergeToRevision = -1;
        }

        private void toRevisionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            toRevisionTextBox.Enabled = true;
            toRevisionTextBox.SelectAll();
            toRevisionSelectButton.Enabled = true;

            TogglePageComplete();
        }

        private void fromRevisionTextBox_TextChanged(object sender, EventArgs e)
        {
            TogglePageComplete();
            
            long rev;
            if (long.TryParse(fromRevisionTextBox.Text, out rev))
                ((MergeSourceTwoDifferentTreesPage)WizardPage).MergeFromRevision = rev;
            else((MergeSourceTwoDifferentTreesPage)WizardPage).MergeFromRevision = -1;
        }

        private void toRevisionTextBox_TextChanged(object sender, EventArgs e)
        {
            TogglePageComplete();
            
            long rev;
            if (long.TryParse(toRevisionTextBox.Text, out rev))
                ((MergeSourceTwoDifferentTreesPage)WizardPage).MergeToRevision = rev;
            else ((MergeSourceTwoDifferentTreesPage)WizardPage).MergeToRevision = -1;
        }

        private void fromURLTextBox_TextChanged(object sender, EventArgs e)
        {
            TogglePageComplete();

            ((MergeSourceTwoDifferentTreesPage)WizardPage).MergeSourceOne = fromURLTextBox.Text;
        }

        private void toURLTextBox_TextChanged(object sender, EventArgs e)
        {
            TogglePageComplete();

            ((MergeSourceTwoDifferentTreesPage)WizardPage).MergeSourceTwo = toURLTextBox.Text;
        }

        private void MergeSourceTwoDifferentTreesPageControl_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                fromURLTextBox.Text = ((MergeWizard)WizardPage.Wizard).MergeTarget.Status.Uri.ToString();
                fromURLTextBox.SelectAll();

                TogglePageComplete();
            }
        }

        private void fromRevisionSelectButton_Click(object sender, EventArgs e)
        {
            DisplayLogViewerAndRetrieveRevisions(sender);
        }

        private void toRevisionSelectButton_Click(object sender, EventArgs e)
        {
            DisplayLogViewerAndRetrieveRevisions(sender);
        }

        private void fromURLSelectButton_Click(object sender, EventArgs e)
        {
            Uri uri = UIUtils.DisplayBrowseDialogAndGetResult(WizardPage,
                ((MergeWizard)WizardPage.Wizard).MergeTarget,
                fromURLTextBox.Text);

            if (uri != null)
                fromURLTextBox.Text = uri.ToString();
        }

        private void toURLSelectButton_Click(object sender, EventArgs e)
        {
            Uri uri = UIUtils.DisplayBrowseDialogAndGetResult(WizardPage,
                ((MergeWizard)WizardPage.Wizard).MergeTarget,
                toURLTextBox.Text);

            if (uri != null)
                toURLTextBox.Text = uri.ToString();
        }
        #endregion
    }
}
