using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.PendingChanges
{
    public partial class ConfigureRecentChangesPageDialog : VSContainerForm
    {
        int _currentSetting;

        public ConfigureRecentChangesPageDialog()
        {
            InitializeComponent();
        }

        private void enableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.intervalUpDown.Enabled = enableCheckBox.Checked;
            okButton.Enabled = IsComplete;
        }

        private void intervalUpDown_ValueChanged(object sender, EventArgs e)
        {
            okButton.Enabled = IsComplete;
        }

        public int RefreshInterval
        {
            get
            {
                return intervalUpDown.Enabled ?
                    (int)intervalUpDown.Value
                    : 0;
            }
            set
            {
                _currentSetting = value;
                enableCheckBox.Checked = _currentSetting > 0;
                intervalUpDown.Value = _currentSetting;
            }
        }

        bool IsComplete
        {
            get
            {
                return RefreshInterval != _currentSetting;
            }
        }

    }
}
