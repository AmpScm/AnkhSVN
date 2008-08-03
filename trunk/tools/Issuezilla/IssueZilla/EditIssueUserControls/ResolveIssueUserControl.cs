using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Fines.IssueZillaLib;

namespace IssueZilla.EditIssueUserControls
{
    public partial class ResolveIssueUserControl : UserControl
    {
        public ResolveIssueUserControl(IMetadataSource source)
        {
            InitializeComponent();

            this.resolutionComboBox.DataSource = source.Resolutions;
        }

        public string SelectedValue
        {
            get
            {
                return this.resolutionComboBox.SelectedValue as string;
            }
        }
    }
}
