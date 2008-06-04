namespace Ankh.UI.MergeWizard
{
    partial class MergeSourceBasePageControl<TControl>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(GetType());
            this.mergeFromComboBox = new System.Windows.Forms.ComboBox();
            this.selectButton = new System.Windows.Forms.Button();
            this.mergeFromLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mergeFromComboBox
            // 
            resources.ApplyResources(this.mergeFromComboBox, "mergeFromComboBox");
            this.mergeFromComboBox.FormattingEnabled = true;
            this.mergeFromComboBox.Name = "mergeFromComboBox";
            this.mergeFromComboBox.TextChanged += new System.EventHandler(this.mergeFromComboBox_TextChanged);
            // 
            // selectButton
            // 
            resources.ApplyResources(this.selectButton, "selectButton");
            this.selectButton.Name = "selectButton";
            this.selectButton.UseVisualStyleBackColor = true;
            // 
            // mergeFromLabel
            // 
            resources.ApplyResources(this.mergeFromLabel, "mergeFromLabel");
            this.mergeFromLabel.Name = "mergeFromLabel";
            // 
            // MergeSourceBasePageControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mergeFromComboBox);
            this.Controls.Add(this.selectButton);
            this.Controls.Add(this.mergeFromLabel);
            this.Name = "MergeSourceBasePageControl";
            this.Load += new System.EventHandler(this.MergeSourceBasePageControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox mergeFromComboBox;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.Label mergeFromLabel;
    }
}
