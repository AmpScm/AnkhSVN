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
            this.SuspendLayout();
            // 
            // connectorTreeView
            // 
            this.connectorTreeView.HideSelection = false;
            this.connectorTreeView.Location = new System.Drawing.Point(3, 3);
            this.connectorTreeView.Name = "connectorTreeView";
            this.connectorTreeView.ShowPlusMinus = false;
            this.connectorTreeView.ShowRootLines = false;
            this.connectorTreeView.Size = new System.Drawing.Size(295, 232);
            this.connectorTreeView.TabIndex = 0;
            this.connectorTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.connectorTreeView_AfterSelect);
            // 
            // ConnectorSelectionPageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.connectorTreeView);
            this.Name = "ConnectorSelectionPageControl";
            this.Size = new System.Drawing.Size(301, 238);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView connectorTreeView;
    }
}
