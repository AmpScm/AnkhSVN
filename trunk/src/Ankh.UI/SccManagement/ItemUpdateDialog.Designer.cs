namespace Ankh.UI.SccManagement
{
    partial class ItemUpdateDialog
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemUpdateDialog));
			this.pendingList = new Ankh.UI.PendingChanges.Commits.PendingCommitsView(this.components);
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.revisionPickerStart = new Ankh.UI.PathSelector.VersionSelector();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pendingList
			// 
			this.pendingList.AllowColumnReorder = true;
			resources.ApplyResources(this.pendingList, "pendingList");
			this.pendingList.CheckBoxes = true;
			this.pendingList.HideSelection = false;
			this.pendingList.Name = "pendingList";
			this.pendingList.ShowSelectAllCheckBox = true;
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
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.revisionPickerStart);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// revisionPickerStart
			// 
			resources.ApplyResources(this.revisionPickerStart, "revisionPickerStart");
			this.revisionPickerStart.Name = "revisionPickerStart";
			this.revisionPickerStart.SvnOrigin = null;
			// 
			// ItemUpdateDialog
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.pendingList);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Name = "ItemUpdateDialog";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private Ankh.UI.PendingChanges.Commits.PendingCommitsView pendingList;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private Ankh.UI.PathSelector.VersionSelector revisionPickerStart;
    }
}