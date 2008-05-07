namespace Ankh.UI.MergeWizard
{
    partial class MergeTypePageControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeTypePageControl));
            this.mergeTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.manuallyRemoveRadioButton = new System.Windows.Forms.RadioButton();
            this.manuallyRecordRadioButton = new System.Windows.Forms.RadioButton();
            this.twoDifferentTreesRadioButton = new System.Windows.Forms.RadioButton();
            this.changesetBasedRadioButton = new System.Windows.Forms.RadioButton();
            this.reintegrateABranchRadioButton = new System.Windows.Forms.RadioButton();
            this.rangeOfRevisionsRadioButton = new System.Windows.Forms.RadioButton();
            this.mergeTypeDescriptionGroupBox = new System.Windows.Forms.GroupBox();
            this.mergeTypeDescriptionLabel = new System.Windows.Forms.Label();
            this.mergeTypePictureBox = new System.Windows.Forms.PictureBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.mergeTypeGroupBox.SuspendLayout();
            this.mergeTypeDescriptionGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mergeTypePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mergeTypeGroupBox
            // 
            resources.ApplyResources(this.mergeTypeGroupBox, "mergeTypeGroupBox");
            this.mergeTypeGroupBox.Controls.Add(this.manuallyRemoveRadioButton);
            this.mergeTypeGroupBox.Controls.Add(this.manuallyRecordRadioButton);
            this.mergeTypeGroupBox.Controls.Add(this.twoDifferentTreesRadioButton);
            this.mergeTypeGroupBox.Controls.Add(this.changesetBasedRadioButton);
            this.mergeTypeGroupBox.Controls.Add(this.reintegrateABranchRadioButton);
            this.mergeTypeGroupBox.Controls.Add(this.rangeOfRevisionsRadioButton);
            this.mergeTypeGroupBox.Name = "mergeTypeGroupBox";
            this.mergeTypeGroupBox.TabStop = false;
            // 
            // manuallyRemoveRadioButton
            // 
            resources.ApplyResources(this.manuallyRemoveRadioButton, "manuallyRemoveRadioButton");
            this.manuallyRemoveRadioButton.Name = "manuallyRemoveRadioButton";
            this.manuallyRemoveRadioButton.UseVisualStyleBackColor = true;
            this.manuallyRemoveRadioButton.CheckedChanged += new System.EventHandler(this.manuallyRemoveRadioButton_CheckedChanged);
            // 
            // manuallyRecordRadioButton
            // 
            resources.ApplyResources(this.manuallyRecordRadioButton, "manuallyRecordRadioButton");
            this.manuallyRecordRadioButton.Name = "manuallyRecordRadioButton";
            this.manuallyRecordRadioButton.UseVisualStyleBackColor = true;
            this.manuallyRecordRadioButton.CheckedChanged += new System.EventHandler(this.manuallyRecordMergeRadioButton_CheckedChanged);
            // 
            // twoDifferentTreesRadioButton
            // 
            resources.ApplyResources(this.twoDifferentTreesRadioButton, "twoDifferentTreesRadioButton");
            this.twoDifferentTreesRadioButton.Name = "twoDifferentTreesRadioButton";
            this.twoDifferentTreesRadioButton.UseVisualStyleBackColor = true;
            this.twoDifferentTreesRadioButton.CheckedChanged += new System.EventHandler(this.twoDifferentTreesRadioButton_CheckedChanged);
            // 
            // changesetBasedRadioButton
            // 
            resources.ApplyResources(this.changesetBasedRadioButton, "changesetBasedRadioButton");
            this.changesetBasedRadioButton.Name = "changesetBasedRadioButton";
            this.changesetBasedRadioButton.UseVisualStyleBackColor = true;
            this.changesetBasedRadioButton.CheckedChanged += new System.EventHandler(this.changesetBasedRadioButton_CheckedChanged);
            // 
            // reintegrateABranchRadioButton
            // 
            resources.ApplyResources(this.reintegrateABranchRadioButton, "reintegrateABranchRadioButton");
            this.reintegrateABranchRadioButton.Name = "reintegrateABranchRadioButton";
            this.reintegrateABranchRadioButton.UseVisualStyleBackColor = true;
            this.reintegrateABranchRadioButton.CheckedChanged += new System.EventHandler(this.reintegrateABranchRadioButton_CheckedChanged);
            // 
            // rangeOfRevisionsRadioButton
            // 
            resources.ApplyResources(this.rangeOfRevisionsRadioButton, "rangeOfRevisionsRadioButton");
            this.rangeOfRevisionsRadioButton.Checked = true;
            this.rangeOfRevisionsRadioButton.Name = "rangeOfRevisionsRadioButton";
            this.rangeOfRevisionsRadioButton.TabStop = true;
            this.rangeOfRevisionsRadioButton.UseVisualStyleBackColor = true;
            this.rangeOfRevisionsRadioButton.CheckedChanged += new System.EventHandler(this.rangeofRevisionsRadioButton_CheckedChanged);
            // 
            // mergeTypeDescriptionGroupBox
            // 
            resources.ApplyResources(this.mergeTypeDescriptionGroupBox, "mergeTypeDescriptionGroupBox");
            this.mergeTypeDescriptionGroupBox.Controls.Add(this.mergeTypeDescriptionLabel);
            this.mergeTypeDescriptionGroupBox.Controls.Add(this.mergeTypePictureBox);
            this.mergeTypeDescriptionGroupBox.Name = "mergeTypeDescriptionGroupBox";
            this.mergeTypeDescriptionGroupBox.TabStop = false;
            // 
            // mergeTypeDescriptionLabel
            // 
            resources.ApplyResources(this.mergeTypeDescriptionLabel, "mergeTypeDescriptionLabel");
            this.mergeTypeDescriptionLabel.Name = "mergeTypeDescriptionLabel";
            // 
            // mergeTypePictureBox
            // 
            resources.ApplyResources(this.mergeTypePictureBox, "mergeTypePictureBox");
            this.mergeTypePictureBox.Name = "mergeTypePictureBox";
            this.mergeTypePictureBox.TabStop = false;
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // MergeTypePageControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.mergeTypeDescriptionGroupBox);
            this.Controls.Add(this.mergeTypeGroupBox);
            this.Name = "MergeTypePageControl";
            this.mergeTypeGroupBox.ResumeLayout(false);
            this.mergeTypeGroupBox.PerformLayout();
            this.mergeTypeDescriptionGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mergeTypePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox mergeTypeGroupBox;
        private System.Windows.Forms.RadioButton rangeOfRevisionsRadioButton;
        private System.Windows.Forms.RadioButton reintegrateABranchRadioButton;
        private System.Windows.Forms.RadioButton changesetBasedRadioButton;
        private System.Windows.Forms.RadioButton twoDifferentTreesRadioButton;
        private System.Windows.Forms.RadioButton manuallyRecordRadioButton;
        private System.Windows.Forms.RadioButton manuallyRemoveRadioButton;
        private System.Windows.Forms.GroupBox mergeTypeDescriptionGroupBox;
        private System.Windows.Forms.PictureBox mergeTypePictureBox;
        private System.Windows.Forms.Label mergeTypeDescriptionLabel;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
