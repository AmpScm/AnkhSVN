using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.PathSelector
{
    partial class VersionSelector
    {

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.typeCombo = new System.Windows.Forms.ComboBox();
            this.typeLabel = new System.Windows.Forms.Label();
            this.versionTypePanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // typeCombo
            // 
            this.typeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeCombo.FormattingEnabled = true;
            this.typeCombo.Location = new System.Drawing.Point(41, 2);
            this.typeCombo.Name = "typeCombo";
            this.typeCombo.Size = new System.Drawing.Size(121, 21);
            this.typeCombo.TabIndex = 1;
            this.typeCombo.SelectedValueChanged += new System.EventHandler(this.typeCombo_SelectedValueChanged);
            // 
            // typeLabel
            // 
            this.typeLabel.AutoSize = true;
            this.typeLabel.Location = new System.Drawing.Point(1, 5);
            this.typeLabel.Name = "typeLabel";
            this.typeLabel.Size = new System.Drawing.Size(34, 13);
            this.typeLabel.TabIndex = 2;
            this.typeLabel.Text = "&Type:";
            // 
            // versionTypePanel
            // 
            this.versionTypePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.versionTypePanel.Location = new System.Drawing.Point(168, 0);
            this.versionTypePanel.Name = "versionTypePanel";
            this.versionTypePanel.Size = new System.Drawing.Size(252, 25);
            this.versionTypePanel.TabIndex = 3;
            // 
            // VersionSelector
            // 
            this.Controls.Add(this.versionTypePanel);
            this.Controls.Add(this.typeLabel);
            this.Controls.Add(this.typeCombo);
            this.Name = "VersionSelector";
            this.Size = new System.Drawing.Size(420, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.ComboBox typeCombo;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.Panel versionTypePanel;
    }
}
