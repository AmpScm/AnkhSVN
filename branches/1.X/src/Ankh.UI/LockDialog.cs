using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ankh.UI
{
	public class LockDialog : Ankh.UI.PathSelector
	{
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.CheckBox stealLocksCheckBox;
        private System.Windows.Forms.GroupBox groupBox1;
		private System.ComponentModel.IContainer components = null;

		public LockDialog()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
            this.RevisionPickerStart.Visible = false;
            this.RevisionPickerEnd.Visible = false;
            this.revisionStartGroupBox.Visible = false;
            this.revisionEndGroupBox.Visible = false;
            this.recursiveCheckBox.Visible = false;
            this.Options = PathSelectorOptions.NoRevision;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.stealLocksCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.suppressGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pathSelectionTreeView
            // 
            this.pathSelectionTreeView.Size = new System.Drawing.Size( 370, 240 );
            // 
            // messageTextBox
            // 
            this.messageTextBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.messageTextBox.Location = new System.Drawing.Point( 8, 16 );
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size( 342, 60 );
            this.messageTextBox.TabIndex = 7;
            // 
            // stealLocksCheckBox
            // 
            this.stealLocksCheckBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.stealLocksCheckBox.Location = new System.Drawing.Point( 16, 384 );
            this.stealLocksCheckBox.Name = "stealLocksCheckBox";
            this.stealLocksCheckBox.Size = new System.Drawing.Size( 104, 24 );
            this.stealLocksCheckBox.TabIndex = 8;
            this.stealLocksCheckBox.Text = "Steal Locks";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.groupBox1.Controls.Add( this.messageTextBox );
            this.groupBox1.Location = new System.Drawing.Point( 8, 248 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 360, 85 );
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lock message";
            // 
            // LockDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.Caption = "Lock";
         
            this.ClientSize = new System.Drawing.Size( 370, 416 );
            this.Controls.Add( this.stealLocksCheckBox );
            this.Controls.Add( this.groupBox1 );
            this.MinimumSize = new System.Drawing.Size( 376, 300 );
            this.Name = "LockDialog";
            this.Text = "Lock";
            this.Controls.SetChildIndex( this.groupBox1, 0 );
            this.Controls.SetChildIndex( this.revisionEndGroupBox, 0 );
            this.Controls.SetChildIndex( this.pathSelectionTreeView, 0 );
            this.Controls.SetChildIndex( this.recursiveCheckBox, 0 );
            this.Controls.SetChildIndex( this.okButton, 0 );
            this.Controls.SetChildIndex( this.cancelButton, 0 );
            this.Controls.SetChildIndex( this.revisionStartGroupBox, 0 );
            this.Controls.SetChildIndex( this.suppressGroupBox, 0 );
            this.Controls.SetChildIndex( this.stealLocksCheckBox, 0 );
            this.suppressGroupBox.ResumeLayout( false );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout();
            this.ResumeLayout( false );

        }
		#endregion
	
        public string Message
        {
            get { return this.messageTextBox.Text; }
            set { this.messageTextBox.Text = value; }
        }

        public bool StealLocks
        {
            get { return this.stealLocksCheckBox.Checked; }
            set { this.stealLocksCheckBox.Checked = value; }
        }
    }
}

