namespace Ankh.UI.IssueTracker
{
    partial class ConnectorSelectionPageControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.connectorTreeView = new System.Windows.Forms.TreeView();
            this.removeCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // connectorTreeView
            // 
            this.connectorTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.connectorTreeView.HideSelection = false;
            this.connectorTreeView.Location = new System.Drawing.Point(0, 0);
            this.connectorTreeView.Name = "connectorTreeView";
            this.connectorTreeView.ShowPlusMinus = false;
            this.connectorTreeView.ShowRootLines = false;
            this.connectorTreeView.Size = new System.Drawing.Size(315, 210);
            this.connectorTreeView.TabIndex = 0;
            this.connectorTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.connectorTreeView_AfterSelect);
            // 
            // removeCheckBox
            // 
            this.removeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.removeCheckBox.AutoSize = true;
            this.removeCheckBox.Location = new System.Drawing.Point(3, 223);
            this.removeCheckBox.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.removeCheckBox.Name = "removeCheckBox";
            this.removeCheckBox.Size = new System.Drawing.Size(191, 17);
            this.removeCheckBox.TabIndex = 1;
            this.removeCheckBox.Text = "Remove Issue Tracker Association";
            this.removeCheckBox.UseVisualStyleBackColor = true;
            this.removeCheckBox.CheckedChanged += new System.EventHandler(this.removeCheckBox_CheckedChanged);
            // 
            // ConnectorSelectionPageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.connectorTreeView);
            this.Controls.Add(this.removeCheckBox);
            this.Name = "ConnectorSelectionPageControl";
            this.Size = new System.Drawing.Size(318, 247);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView connectorTreeView;
        private System.Windows.Forms.CheckBox removeCheckBox;
    }
}
