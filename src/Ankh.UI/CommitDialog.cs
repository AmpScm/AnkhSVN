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
        public event EventHandler DiffWanted;		

        public CommitDialog( CommitItem[] items )
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.CreateToolTips();

            foreach( CommitItem item in items )
                this.commitItemsList.Items.Add( item.Path );

            this.diffTextBox.Visible = false;

            
        }

        /// <summary>
        /// The log message to be used for this commit.
        /// </summary>
        public string LogMessage
        {
            get{ return this.logMessageBox.Text; }
        }

        public string Diff
        {
            get{ return this.diffTextBox.Diff; }
            set
            { 
                this.diff = value; 
                this.diffTextBox.Diff = this.diff;
            }
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
        private void CreateToolTips()
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
//            commitToolTip.SetToolTip( this.logMessageControl, 
//                "Write changes you have performed since last revision or update" ); 
            commitToolTip.SetToolTip( this.okButton, 
                "Files and comment will be added to the repository and made available for your collegues." ); 
            commitToolTip.SetToolTip( this.cancelButton, "The commit will be cancelled" );  
        }

        private void showDiffButton_Click(object sender, System.EventArgs e)
        {
            if ( this.diffTextBox.Visible )
            {
                this.diffTextBox.Visible = false;
                this.Height -= this.diffTextBox.Height;
                this.showDiffButton.Text = "Show diff";  
            }
            else
            {
                if ( this.diff == null && this.DiffWanted != null )
                    this.DiffWanted( this, EventArgs.Empty );
                this.Height += 400;
                this.diffTextBox.Visible = true;
                this.showDiffButton.Text = "Hide diff";
            }

        }

        private void DiffViewClosed( object sender, System.EventArgs e )
        {
            this.showDiffButton.Enabled = true; 
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
            this.logLabel = new System.Windows.Forms.Label();
            this.commitItemsList = new System.Windows.Forms.ListBox();
            this.showDiffButton = new System.Windows.Forms.Button();
            this.diffTextBox = new Ankh.UI.DiffTextBox();
            this.logMessageBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(710, 280);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            // 
            // okButton
            // 
            this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(622, 280);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 2;
            this.okButton.Text = "Ok";
            // 
            // logLabel
            // 
            this.logLabel.Location = new System.Drawing.Point(0, 112);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(296, 23);
            this.logLabel.TabIndex = 4;
            this.logLabel.Text = "Write log message:";
            // 
            // commitItemsList
            // 
            this.commitItemsList.Dock = System.Windows.Forms.DockStyle.Top;
            this.commitItemsList.Name = "commitItemsList";
            this.commitItemsList.Size = new System.Drawing.Size(816, 108);
            this.commitItemsList.TabIndex = 5;
            // 
            // showDiffButton
            // 
            this.showDiffButton.Location = new System.Drawing.Point(8, 280);
            this.showDiffButton.Name = "showDiffButton";
            this.showDiffButton.TabIndex = 6;
            this.showDiffButton.Text = "Show diff";
            this.showDiffButton.Click += new System.EventHandler(this.showDiffButton_Click);
            // 
            // diffTextBox
            // 
            this.diffTextBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.diffTextBox.Diff = "";
            this.diffTextBox.Font = new System.Drawing.Font("Courier New", 10F);
            this.diffTextBox.Location = new System.Drawing.Point(0, 312);
            this.diffTextBox.Name = "diffTextBox";
            this.diffTextBox.ReadOnly = true;
            this.diffTextBox.Size = new System.Drawing.Size(814, 0);
            this.diffTextBox.TabIndex = 7;
            this.diffTextBox.Text = "";
            // 
            // logMessageBox
            // 
            this.logMessageBox.AcceptsTab = true;
            this.logMessageBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.logMessageBox.Location = new System.Drawing.Point(0, 136);
            this.logMessageBox.Name = "logMessageBox";
            this.logMessageBox.Size = new System.Drawing.Size(816, 128);
            this.logMessageBox.TabIndex = 8;
            this.logMessageBox.Text = "";
            // 
            // CommitDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(816, 315);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.logMessageBox,
                                                                          this.diffTextBox,
                                                                          this.showDiffButton,
                                                                          this.commitItemsList,
                                                                          this.logLabel,
                                                                          this.okButton,
                                                                          this.cancelButton});
            this.MinimumSize = new System.Drawing.Size(390, 320);
            this.Name = "CommitDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Commit";
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label logLabel;
        private System.Windows.Forms.ListBox commitItemsList;
        private System.Windows.Forms.Button showDiffButton;
        private string diff;
        private Ankh.UI.DiffTextBox diffTextBox;
        private System.Windows.Forms.RichTextBox logMessageBox;
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



