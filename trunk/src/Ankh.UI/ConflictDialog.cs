// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog that allows the user to resolve a conflicted file.
    /// </summary>
    public class ConflictDialog : System.Windows.Forms.Form
    {
        public ConflictDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.mineFileRadioButton.Checked = true;

            this.CreateToolTips();
        }

        /// <summary>
        /// The selection made by the user.
        /// </summary>
        public string Selection
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.selectedChoice; }
        }

        /// <summary>
        /// The filenames to resolve, in the order {mine, new, old, base}.
        /// </summary>
        public string[] Filenames
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            { 
                return new string[]{
                                       (string)this.mineFileRadioButton.Tag,
                                       (string)this.newRevRadioButton.Tag,
                                       (string)this.oldRevRadioButton.Tag,
                                       (string)this.fileRadioButton.Tag
                                   };
            }

            [System.Diagnostics.DebuggerStepThrough]
            set
            { 
                Debug.Assert( value.Length == 4, "There should be 4 filenames" );
                this.mineFileRadioButton.Text = Path.GetFileName(value[0]);
                this.mineFileRadioButton.Tag = value[0];

                this.oldRevRadioButton.Text = Path.GetFileName(value[1]);
                this.oldRevRadioButton.Tag = value[1];

                this.newRevRadioButton.Text = Path.GetFileName(value[2]);
                this.newRevRadioButton.Tag = value[2];

                this.fileRadioButton.Text = Path.GetFileName(value[3]);
                this.fileRadioButton.Tag = value[3];
                this.selectedChoice = value[3];
                this.fileRadioButton.Checked = true;

                
            }
        }

        /// <summary>
        /// Whether this is a merge of two binary files.
        /// </summary>
        public bool Binary
        {
            get{ return this.binary; }
            set
            {
                this.binary = value;
                
                this.mineFileRadioButton.Enabled = 
                    this.mineFileRadioButton.Visible = !this.binary;

                // make sure there's at least one button checked.
                if ( this.binary )
                {
                    this.fileRadioButton.Checked = true;
                    this.selectedChoice = (string)this.fileRadioButton.Tag;
                }
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

        private void selectedButton(object sender, System.EventArgs e)
        {
            this.selectedChoice = (string)((RadioButton) sender).Tag;
        }

        private void CreateToolTips()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip conflictToolTip = new ToolTip();

            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip( this.mineFileRadioButton, "Latest local file" ); 
            conflictToolTip.SetToolTip( this.oldRevRadioButton, "Latest updated revision" );
            conflictToolTip.SetToolTip( this.newRevRadioButton, "Latest version in repository" );
            conflictToolTip.SetToolTip( this.fileRadioButton, "File with conflict markers" );
            conflictToolTip.SetToolTip( this.okButton, "Resolve conflict and delete the three files that are not selected." ); 
            conflictToolTip.SetToolTip( this.cancelButton, "Cancel this dialog." );  
        }


		#region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mineFileRadioButton = new System.Windows.Forms.RadioButton();
            this.oldRevRadioButton = new System.Windows.Forms.RadioButton();
            this.newRevRadioButton = new System.Windows.Forms.RadioButton();
            this.fileRadioButton = new System.Windows.Forms.RadioButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mineFileRadioButton
            // 
            this.mineFileRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.mineFileRadioButton.Location = new System.Drawing.Point(8, 96);
            this.mineFileRadioButton.Name = "mineFileRadioButton";
            this.mineFileRadioButton.Size = new System.Drawing.Size(280, 24);
            this.mineFileRadioButton.TabIndex = 0;
            this.mineFileRadioButton.TabStop = true;
            this.mineFileRadioButton.Text = "test.mine";
            this.mineFileRadioButton.Click += new System.EventHandler(this.selectedButton);
            // 
            // oldRevRadioButton
            // 
            this.oldRevRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.oldRevRadioButton.Location = new System.Drawing.Point(8, 48);
            this.oldRevRadioButton.Name = "oldRevRadioButton";
            this.oldRevRadioButton.Size = new System.Drawing.Size(280, 24);
            this.oldRevRadioButton.TabIndex = 1;
            this.oldRevRadioButton.TabStop = true;
            this.oldRevRadioButton.Text = "test.r1";
            this.oldRevRadioButton.Click += new System.EventHandler(this.selectedButton);
            // 
            // newRevRadioButton
            // 
            this.newRevRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.newRevRadioButton.Location = new System.Drawing.Point(8, 72);
            this.newRevRadioButton.Name = "newRevRadioButton";
            this.newRevRadioButton.Size = new System.Drawing.Size(280, 24);
            this.newRevRadioButton.TabIndex = 2;
            this.newRevRadioButton.TabStop = true;
            this.newRevRadioButton.Text = "test.r2";
            this.newRevRadioButton.Click += new System.EventHandler(this.selectedButton);
            // 
            // fileRadioButton
            // 
            this.fileRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.fileRadioButton.Location = new System.Drawing.Point(8, 24);
            this.fileRadioButton.Name = "fileRadioButton";
            this.fileRadioButton.Size = new System.Drawing.Size(280, 24);
            this.fileRadioButton.TabIndex = 3;
            this.fileRadioButton.TabStop = true;
            this.fileRadioButton.Text = "test.txt";
            this.fileRadioButton.Click += new System.EventHandler(this.selectedButton);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(228, 145);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(148, 145);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Resolve";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.oldRevRadioButton);
            this.groupBox1.Controls.Add(this.newRevRadioButton);
            this.groupBox1.Controls.Add(this.fileRadioButton);
            this.groupBox1.Controls.Add(this.mineFileRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(296, 128);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select file";
            // 
            // ConflictDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(312, 176);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(312, 192);
            this.Name = "ConflictDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Resolve conflicted file";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.RadioButton mineFileRadioButton;
        private System.Windows.Forms.RadioButton fileRadioButton;
        private string selectedChoice;

        private System.Windows.Forms.RadioButton oldRevRadioButton;
        private System.Windows.Forms.RadioButton newRevRadioButton;
        private System.Windows.Forms.GroupBox groupBox1;

        private System.ComponentModel.IContainer components = null;
        private bool binary = false;
    }

}

