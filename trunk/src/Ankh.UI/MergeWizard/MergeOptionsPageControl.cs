using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using System.Diagnostics;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeOptionsPageControl : BasePageControl
    {
        private Dictionary<SvnDepth, string> mergeDepths;

        public MergeOptionsPageControl()
        {
            InitializeComponent();
        }

        #region UI Events
        private void MergeOptionsPageControl_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                // Moved from Constructor to _Load for timing reasons.
                mergeDepths = ((MergeWizard)WizardPage.Wizard).MergeUtils.MergeDepths;

                // Decided against using BindingSource due to rendering time
                // and the requirement of threading to keep the UI from 
                // "freezing" when initially displayed.
                foreach (KeyValuePair<SvnDepth, string> kvp in mergeDepths)
                {
                    depthComboBox.Items.Add(kvp.Value);
                }

                UIUtils.ResizeDropDownForLongestEntry(depthComboBox);

                depthComboBox.SelectedIndex = 0;
            }
        }
        #endregion
    }
}
