using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeSourceTwoDifferentTreesPageControl : UserControl
    {
        private WizardPage _wizardPage;

        public MergeSourceTwoDifferentTreesPageControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets/Sets the wizard page associated with this control.
        /// </summary>
        public WizardPage WizardPage
        {
            set { _wizardPage = value; }
            get { return _wizardPage; }
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
            }
            else
            {
                toURLSelectButton.Enabled = true;
                toURLSelectButton.Visible = true;

                toURLTextBox.Enabled = true;
                toURLTextBox.Visible = true;
                toURLTextBox.Text = fromURLTextBox.Text;
                toURLTextBox.SelectAll();
            }
        }

        private void MergeSourceTwoDifferentTreesPageControl_Load(object sender, EventArgs e)
        {
            fromURLTextBox.Text = ((MergeWizard)WizardPage.Wizard).MergeTarget.Status.Uri.ToString();
            fromURLTextBox.SelectAll();
        }
        #endregion
    }
}
