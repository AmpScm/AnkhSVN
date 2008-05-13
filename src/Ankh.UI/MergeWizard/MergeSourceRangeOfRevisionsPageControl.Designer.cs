namespace Ankh.UI.MergeWizard
{
    partial class MergeSourceRangeOfRevisionsPageControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeSourceRangeOfRevisionsPageControl));
            this.mergeFromLabel = new System.Windows.Forms.Label();
            this.selectButton = new System.Windows.Forms.Button();
            this.mergeFromTextBox = new System.Windows.Forms.TextBox();
            this.revisionsGroupBox = new System.Windows.Forms.GroupBox();
            this.allRevisionsRadioButton = new System.Windows.Forms.RadioButton();
            this.selectRevisionsRadioButton = new System.Windows.Forms.RadioButton();
            this.revisionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // mergeFromLabel
            // 
            resources.ApplyResources(this.mergeFromLabel, "mergeFromLabel");
            this.mergeFromLabel.Name = "mergeFromLabel";
            // 
            // selectButton
            // 
            resources.ApplyResources(this.selectButton, "selectButton");
            this.selectButton.Name = "selectButton";
            this.selectButton.UseVisualStyleBackColor = true;
            // 
            // mergeFromTextBox
            // 
            resources.ApplyResources(this.mergeFromTextBox, "mergeFromTextBox");
            this.mergeFromTextBox.Name = "mergeFromTextBox";
            // 
            // revisionsGroupBox
            // 
            resources.ApplyResources(this.revisionsGroupBox, "revisionsGroupBox");
            this.revisionsGroupBox.Controls.Add(this.selectRevisionsRadioButton);
            this.revisionsGroupBox.Controls.Add(this.allRevisionsRadioButton);
            this.revisionsGroupBox.Name = "revisionsGroupBox";
            this.revisionsGroupBox.TabStop = false;
            // 
            // allRevisionsRadioButton
            // 
            resources.ApplyResources(this.allRevisionsRadioButton, "allRevisionsRadioButton");
            this.allRevisionsRadioButton.Checked = true;
            this.allRevisionsRadioButton.Name = "allRevisionsRadioButton";
            this.allRevisionsRadioButton.TabStop = true;
            this.allRevisionsRadioButton.UseVisualStyleBackColor = true;
            // 
            // selectRevisionsRadioButton
            // 
            resources.ApplyResources(this.selectRevisionsRadioButton, "selectRevisionsRadioButton");
            this.selectRevisionsRadioButton.Name = "selectRevisionsRadioButton";
            this.selectRevisionsRadioButton.UseVisualStyleBackColor = true;
            // 
            // MergeSourceRangeOfRevisionsPageControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.revisionsGroupBox);
            this.Controls.Add(this.mergeFromTextBox);
            this.Controls.Add(this.selectButton);
            this.Controls.Add(this.mergeFromLabel);
            this.Name = "MergeSourceRangeOfRevisionsPageControl";
            this.revisionsGroupBox.ResumeLayout(false);
            this.revisionsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mergeFromLabel;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.TextBox mergeFromTextBox;
        private System.Windows.Forms.GroupBox revisionsGroupBox;
        private System.Windows.Forms.RadioButton allRevisionsRadioButton;
        private System.Windows.Forms.RadioButton selectRevisionsRadioButton;
    }
}
