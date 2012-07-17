namespace Ankh.UI.RepositoryOpen
{
    partial class ProjectAddInfoDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectAddInfoDialog));
            this.projectLabel = new System.Windows.Forms.Label();
            this.externalRadio = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.externalPropLocations = new System.Windows.Forms.ComboBox();
            this.externalBlock = new System.Windows.Forms.Panel();
            this.lockExternal = new System.Windows.Forms.CheckBox();
            this.externalDefInfo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.copyRadio = new System.Windows.Forms.RadioButton();
            this.copyBlock = new System.Windows.Forms.Panel();
            this.copyDefInfo = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.slnRadio = new System.Windows.Forms.RadioButton();
            this.slnBlock = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.unconnectedBlock = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.unconnectedRadio = new System.Windows.Forms.RadioButton();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.notVersionedRadio = new System.Windows.Forms.RadioButton();
            this.externalBlock.SuspendLayout();
            this.copyBlock.SuspendLayout();
            this.slnBlock.SuspendLayout();
            this.unconnectedBlock.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // projectLabel
            // 
            resources.ApplyResources(this.projectLabel, "projectLabel");
            this.projectLabel.Name = "projectLabel";
            // 
            // externalRadio
            // 
            resources.ApplyResources(this.externalRadio, "externalRadio");
            this.externalRadio.Checked = true;
            this.externalRadio.Name = "externalRadio";
            this.externalRadio.TabStop = true;
            this.externalRadio.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // externalPropLocations
            // 
            resources.ApplyResources(this.externalPropLocations, "externalPropLocations");
            this.externalPropLocations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.externalPropLocations.FormattingEnabled = true;
            this.externalPropLocations.Name = "externalPropLocations";
            // 
            // externalBlock
            // 
            resources.ApplyResources(this.externalBlock, "externalBlock");
            this.externalBlock.Controls.Add(this.lockExternal);
            this.externalBlock.Controls.Add(this.externalDefInfo);
            this.externalBlock.Controls.Add(this.label2);
            this.externalBlock.Controls.Add(this.label1);
            this.externalBlock.Controls.Add(this.externalPropLocations);
            this.externalBlock.Name = "externalBlock";
            // 
            // lockExternal
            // 
            resources.ApplyResources(this.lockExternal, "lockExternal");
            this.lockExternal.Name = "lockExternal";
            this.lockExternal.UseVisualStyleBackColor = true;
            this.lockExternal.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // externalDefInfo
            // 
            resources.ApplyResources(this.externalDefInfo, "externalDefInfo");
            this.externalDefInfo.Name = "externalDefInfo";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // copyRadio
            // 
            resources.ApplyResources(this.copyRadio, "copyRadio");
            this.copyRadio.Name = "copyRadio";
            this.copyRadio.UseVisualStyleBackColor = true;
            // 
            // copyBlock
            // 
            resources.ApplyResources(this.copyBlock, "copyBlock");
            this.copyBlock.Controls.Add(this.copyDefInfo);
            this.copyBlock.Controls.Add(this.label6);
            this.copyBlock.Name = "copyBlock";
            // 
            // copyDefInfo
            // 
            resources.ApplyResources(this.copyDefInfo, "copyDefInfo");
            this.copyDefInfo.Name = "copyDefInfo";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // slnRadio
            // 
            resources.ApplyResources(this.slnRadio, "slnRadio");
            this.slnRadio.Name = "slnRadio";
            this.slnRadio.UseVisualStyleBackColor = true;
            // 
            // slnBlock
            // 
            resources.ApplyResources(this.slnBlock, "slnBlock");
            this.slnBlock.Controls.Add(this.label5);
            this.slnBlock.Controls.Add(this.label4);
            this.slnBlock.Name = "slnBlock";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // unconnectedBlock
            // 
            resources.ApplyResources(this.unconnectedBlock, "unconnectedBlock");
            this.unconnectedBlock.Controls.Add(this.label8);
            this.unconnectedBlock.Name = "unconnectedBlock";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // unconnectedRadio
            // 
            resources.ApplyResources(this.unconnectedRadio, "unconnectedRadio");
            this.unconnectedRadio.Name = "unconnectedRadio";
            this.unconnectedRadio.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.label3);
            this.panel1.Name = "panel1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // notVersionedRadio
            // 
            resources.ApplyResources(this.notVersionedRadio, "notVersionedRadio");
            this.notVersionedRadio.Name = "notVersionedRadio";
            this.notVersionedRadio.UseVisualStyleBackColor = true;
            // 
            // ProjectAddInfoDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.notVersionedRadio);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.unconnectedBlock);
            this.Controls.Add(this.unconnectedRadio);
            this.Controls.Add(this.slnBlock);
            this.Controls.Add(this.slnRadio);
            this.Controls.Add(this.copyBlock);
            this.Controls.Add(this.copyRadio);
            this.Controls.Add(this.externalBlock);
            this.Controls.Add(this.externalRadio);
            this.Controls.Add(this.projectLabel);
            this.Name = "ProjectAddInfoDialog";
            this.externalBlock.ResumeLayout(false);
            this.externalBlock.PerformLayout();
            this.copyBlock.ResumeLayout(false);
            this.copyBlock.PerformLayout();
            this.slnBlock.ResumeLayout(false);
            this.slnBlock.PerformLayout();
            this.unconnectedBlock.ResumeLayout(false);
            this.unconnectedBlock.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label projectLabel;
        private System.Windows.Forms.RadioButton externalRadio;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox externalPropLocations;
        private System.Windows.Forms.Panel externalBlock;
        private System.Windows.Forms.CheckBox lockExternal;
        private System.Windows.Forms.Label externalDefInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton copyRadio;
        private System.Windows.Forms.Panel copyBlock;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton slnRadio;
        private System.Windows.Forms.Panel slnBlock;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel unconnectedBlock;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton unconnectedRadio;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label copyDefInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton notVersionedRadio;
    }
}