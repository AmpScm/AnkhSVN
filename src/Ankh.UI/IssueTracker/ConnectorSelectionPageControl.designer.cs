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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectorSelectionPageControl));
            this.connectorTreeView = new System.Windows.Forms.TreeView();
            this.removeCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // connectorTreeView
            // 
            resources.ApplyResources(this.connectorTreeView, "connectorTreeView");
            this.connectorTreeView.HideSelection = false;
            this.connectorTreeView.Name = "connectorTreeView";
            this.connectorTreeView.ShowPlusMinus = false;
            this.connectorTreeView.ShowRootLines = false;
            this.connectorTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.connectorTreeView_AfterSelect);
            // 
            // removeCheckBox
            // 
            resources.ApplyResources(this.removeCheckBox, "removeCheckBox");
            this.removeCheckBox.Name = "removeCheckBox";
            this.removeCheckBox.UseVisualStyleBackColor = true;
            this.removeCheckBox.CheckedChanged += new System.EventHandler(this.removeCheckBox_CheckedChanged);
            // 
            // ConnectorSelectionPageControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.connectorTreeView);
            this.Controls.Add(this.removeCheckBox);
            this.Name = "ConnectorSelectionPageControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView connectorTreeView;
        private System.Windows.Forms.CheckBox removeCheckBox;
    }
}
