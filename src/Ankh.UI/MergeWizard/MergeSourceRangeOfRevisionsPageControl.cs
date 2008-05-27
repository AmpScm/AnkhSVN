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
    public partial class MergeSourceRangeOfRevisionsPageControl : MergeSourceBasePageControl
    {
        public MergeSourceRangeOfRevisionsPageControl()
        {
            InitializeComponent();
        }

        #region UI Events
        /// <summary>
        /// Enable the "Next" button since the revision(s) must be selected on the next page.
        /// </summary>
        private void selectRevisionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                ((MergeSourceRangeOfRevisionsPage)WizardPage).NextPageRequired = true;
            }
        }

        /// <summary>
        /// Disable the "Next" button since the all applicable revisions will be merged.
        /// </summary>
        private void allRevisionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                ((MergeSourceRangeOfRevisionsPage)WizardPage).NextPageRequired = false;
            }
        }
        #endregion
    }
}
