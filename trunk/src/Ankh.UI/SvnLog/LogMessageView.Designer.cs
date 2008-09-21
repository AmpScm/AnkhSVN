using Ankh.UI.PendingChanges;
using System.Windows.Forms;
namespace Ankh.UI
{
    partial class LogMessageView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogMessageView));
            this.logMessageEditor = new LogMessageEditor();
            this.logMessageTextBox = new TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
            // logMessageEditor
			// 
            resources.ApplyResources(this.logMessageEditor, "logMessageEditor");
			this.logMessageEditor.BackColor = System.Drawing.SystemColors.Window;
			this.logMessageEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logMessageEditor.Name = "logMessageEditor";
			this.logMessageEditor.ReadOnly = true;
            // 
            // logMessageTextBox
            // 
            resources.ApplyResources(this.logMessageTextBox, "logMessageTextBox");
            this.logMessageTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.logMessageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logMessageTextBox.Name = "logMessageTextBox";
            this.logMessageTextBox.ReadOnly = true;
            this.logMessageTextBox.Visible = false;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// LogMessageView
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.logMessageEditor);
            this.Controls.Add(this.logMessageTextBox);
			this.Controls.Add(this.label1);
			this.Name = "LogMessageView";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private LogMessageEditor logMessageEditor;
        private TextBox logMessageTextBox;
        private System.Windows.Forms.Label label1;
    }
}
