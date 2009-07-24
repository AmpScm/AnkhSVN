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
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // connectorTreeView
            // 
            this.connectorTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectorTreeView.HideSelection = false;
            this.connectorTreeView.Location = new System.Drawing.Point(0, 0);
            this.connectorTreeView.Name = "connectorTreeView";
            this.connectorTreeView.ShowPlusMinus = false;
            this.connectorTreeView.ShowRootLines = false;
            this.connectorTreeView.Size = new System.Drawing.Size(301, 221);
            this.connectorTreeView.TabIndex = 0;
            this.connectorTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.connectorTreeView_AfterSelect);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.checkBox1.Location = new System.Drawing.Point(0, 221);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(301, 17);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Remove Issue Tracker Association";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ConnectorSelectionPageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.connectorTreeView);
            this.Controls.Add(this.checkBox1);
            this.Name = "ConnectorSelectionPageControl";
            this.Size = new System.Drawing.Size(301, 238);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView connectorTreeView;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
