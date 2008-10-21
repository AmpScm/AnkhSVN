namespace Ankh.UI
{
    partial class LogViewerDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogViewerDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.logViewerControl = new Ankh.UI.SvnLog.LogControl(this.components);
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // logViewerControl
            // 
            resources.ApplyResources(this.logViewerControl, "logViewerControl");
            this.logViewerControl.ChangedPathsVisible = true;
            this.logViewerControl.IncludeMerged = false;
            this.logViewerControl.LogMessageVisible = true;
            this.logViewerControl.Mode = Ankh.UI.SvnLog.LogMode.Log;
            this.logViewerControl.Name = "logViewerControl";
            this.logViewerControl.StrictNodeHistory = false;
            // 
            // LogViewerDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.logViewerControl);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogViewerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.LogViewerDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private Ankh.UI.SvnLog.LogControl logViewerControl;
    }
}