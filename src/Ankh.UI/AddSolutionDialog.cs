using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Utils.Win32;

namespace Ankh.UI
{
	/// <summary>
	/// A dialog for adding a solution to the repository.
	/// </summary>
	public class AddSolutionDialog : System.Windows.Forms.Form
	{
		public AddSolutionDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            Win32.SHAutoComplete( this.urlTextBox.Handle, 
                Shacf.UrlAll | Shacf.Filesystem );
		}

        /// <summary>
        /// The base URL.
        /// </summary>
        public string BaseUrl
        {
            get{ return this.urlTextBox.Text; }
            set{ this.urlTextBox.Text = value; }
        }

        /// <summary>
        /// Whether to create a subdirectory of the base URL.
        /// </summary>
        public bool CreateSubDirectory
        {
            get{ return this.createSubDirCheckBox.Checked; }
        }

        /// <summary>
        /// The name of the sub directory name to optionally create.
        /// </summary>
        public string SubDirectoryName
        {
            get{ return this.subDirNameTextBox.Text; }
        }

        /// <summary>
        /// The log message to be used when creating the subdirectory.
        /// </summary>
        public string LogMessage
        {
            get{ return this.logMessageTextBox.Text; }
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.locationGroupBox = new System.Windows.Forms.GroupBox();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.subDirGroupBox = new System.Windows.Forms.GroupBox();
            this.logMessageTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.subDirNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.createSubDirCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.locationGroupBox.SuspendLayout();
            this.subDirGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // locationGroupBox
            // 
            this.locationGroupBox.Controls.Add(this.urlTextBox);
            this.locationGroupBox.Controls.Add(this.label1);
            this.locationGroupBox.Location = new System.Drawing.Point(16, 24);
            this.locationGroupBox.Name = "locationGroupBox";
            this.locationGroupBox.Size = new System.Drawing.Size(264, 72);
            this.locationGroupBox.TabIndex = 0;
            this.locationGroupBox.TabStop = false;
            this.locationGroupBox.Text = "Location";
            // 
            // urlTextBox
            // 
            this.urlTextBox.Location = new System.Drawing.Point(38, 32);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(208, 20);
            this.urlTextBox.TabIndex = 1;
            this.urlTextBox.Text = "";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(5, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "URL:";
            // 
            // subDirGroupBox
            // 
            this.subDirGroupBox.Controls.Add(this.logMessageTextBox);
            this.subDirGroupBox.Controls.Add(this.label3);
            this.subDirGroupBox.Controls.Add(this.subDirNameTextBox);
            this.subDirGroupBox.Controls.Add(this.label2);
            this.subDirGroupBox.Controls.Add(this.createSubDirCheckBox);
            this.subDirGroupBox.Location = new System.Drawing.Point(16, 104);
            this.subDirGroupBox.Name = "subDirGroupBox";
            this.subDirGroupBox.Size = new System.Drawing.Size(264, 200);
            this.subDirGroupBox.TabIndex = 1;
            this.subDirGroupBox.TabStop = false;
            this.subDirGroupBox.Text = "Subdirectory";
            // 
            // logMessageTextBox
            // 
            this.logMessageTextBox.AcceptsReturn = true;
            this.logMessageTextBox.AcceptsTab = true;
            this.logMessageTextBox.Enabled = false;
            this.logMessageTextBox.Location = new System.Drawing.Point(8, 104);
            this.logMessageTextBox.Multiline = true;
            this.logMessageTextBox.Name = "logMessageTextBox";
            this.logMessageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logMessageTextBox.Size = new System.Drawing.Size(248, 88);
            this.logMessageTextBox.TabIndex = 4;
            this.logMessageTextBox.Text = "";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 84);
            this.label3.Name = "label3";
            this.label3.TabIndex = 3;
            this.label3.Text = "Log message:";
            // 
            // subDirNameTextBox
            // 
            this.subDirNameTextBox.Enabled = false;
            this.subDirNameTextBox.Location = new System.Drawing.Point(48, 56);
            this.subDirNameTextBox.Name = "subDirNameTextBox";
            this.subDirNameTextBox.Size = new System.Drawing.Size(200, 20);
            this.subDirNameTextBox.TabIndex = 2;
            this.subDirNameTextBox.Text = "";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Name:";
            // 
            // createSubDirCheckBox
            // 
            this.createSubDirCheckBox.Location = new System.Drawing.Point(8, 24);
            this.createSubDirCheckBox.Name = "createSubDirCheckBox";
            this.createSubDirCheckBox.Size = new System.Drawing.Size(160, 24);
            this.createSubDirCheckBox.TabIndex = 0;
            this.createSubDirCheckBox.Text = "Create subdirectory";
            this.createSubDirCheckBox.CheckedChanged += new System.EventHandler(this.createSubDirCheckBox_CheckedChanged);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(128, 312);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(216, 312);
            this.button2.Name = "button2";
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            // 
            // AddSolutionDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(296, 341);
            this.ControlBox = false;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.subDirGroupBox);
            this.Controls.Add(this.locationGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AddSolutionDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add solution to repository";
            this.locationGroupBox.ResumeLayout(false);
            this.subDirGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private void createSubDirCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            bool enabled = this.createSubDirCheckBox.Checked;
            this.subDirNameTextBox.Enabled = enabled;
            this.logMessageTextBox.Enabled = enabled;
        }

        private System.Windows.Forms.GroupBox locationGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox subDirGroupBox;
        private System.Windows.Forms.CheckBox createSubDirCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox subDirNameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox logMessageTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox urlTextBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
	}
}
