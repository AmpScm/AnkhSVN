// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NSvn.Core;

namespace Ankh.UI
{
	/// <summary>
	/// Summary description for CommitDialog.
	/// </summary>
	public class CommitDialog : System.Windows.Forms.Form
	{
        
		public CommitDialog( CommitItem[] items )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
            CreateMyToolTip();

            foreach( CommitItem item in items )
                this.commitItemsList.Items.Add( item.Path );
		}

        /// <summary>
        /// The log message to be used for this commit.
        /// </summary>
        public string LogMessage
        {
            get{ return this.logMessageControl.LogMessage; }
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

        /// <summary>
        /// Makes tooltips on buttons and fields. 
        /// </summary>
        private void CreateMyToolTip()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip commitToolTip = new ToolTip();

            // Set up the delays in milliseconds for the ToolTip.
            commitToolTip.AutoPopDelay = 5000;
            commitToolTip.InitialDelay = 1000;
            commitToolTip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            commitToolTip.ShowAlways = true;
         
            // Set up the ToolTip text for the Button and Checkbox.
//            commitToolTip.SetToolTip( this.fileTreeView, 
//                "Three view of files you attempt to publish/commit. Files will be added into the repository." ); 
             //Doesn't work:(. Don't understand why.
            commitToolTip.SetToolTip( this.logMessageControl, 
                "Write changes you have performed since last revision or update" ); 
            commitToolTip.SetToolTip( this.okButton, 
                "Files and comment will be added to the repository and made available for your collegues." ); 
            commitToolTip.SetToolTip( this.cancelButton, "The commit will be cancelled" );  
        }



		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.logMessageControl = new Ankh.UI.LogMessageControl();
            this.logLabel = new System.Windows.Forms.Label();
            this.commitItemsList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(436, 316);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            // 
            // okButton
            // 
            this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(356, 316);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 2;
            this.okButton.Text = "Ok";
            // 
            // logMessageControl
            // 
            this.logMessageControl.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.logMessageControl.Location = new System.Drawing.Point(0, 184);
            this.logMessageControl.Name = "logMessageControl";
            this.logMessageControl.Size = new System.Drawing.Size(508, 128);
            this.logMessageControl.TabIndex = 1;
            // 
            // logLabel
            // 
            this.logLabel.Location = new System.Drawing.Point(0, 160);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(296, 23);
            this.logLabel.TabIndex = 4;
            this.logLabel.Text = "Write log message:";
            // 
            // commitItemsList
            // 
            this.commitItemsList.Dock = System.Windows.Forms.DockStyle.Top;
            this.commitItemsList.Name = "commitItemsList";
            this.commitItemsList.Size = new System.Drawing.Size(512, 147);
            this.commitItemsList.TabIndex = 5;
            // 
            // CommitDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(512, 341);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.commitItemsList,
                                                                          this.logLabel,
                                                                          this.logMessageControl,
                                                                          this.okButton,
                                                                          this.cancelButton});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimumSize = new System.Drawing.Size(390, 350);
            this.Name = "CommitDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Commit";
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label logLabel;
        private Ankh.UI.LogMessageControl logMessageControl;
        private System.Windows.Forms.ListBox commitItemsList;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

//        [STAThread] 
//        public static void Main()
//        {
//            CommitDialog the = new CommitDialog();
//            the.ShowDialog();
//        }



	}
}



