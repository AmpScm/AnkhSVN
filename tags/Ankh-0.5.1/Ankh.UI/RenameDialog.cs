// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for entering a new name for a file.
    /// </summary>
    public class RenameDialog : System.Windows.Forms.Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oldName">The old name of the file</param>
        public RenameDialog( string oldName )
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.oldName = oldName;
            this.oldNameLabel.Text = String.Format( "New name for {0}:", this.oldName );
            this.newNameTextBox.Text = this.oldName;
        }

        /// <summary>
        /// The new name entered by the user.
        /// </summary>
        public string NewName
        {
            get{ return this.newNameTextBox.Text; }
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
            this.oldNameLabel = new System.Windows.Forms.Label();
            this.newNameTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // oldNameLabel
            // 
            this.oldNameLabel.Location = new System.Drawing.Point(8, 8);
            this.oldNameLabel.Name = "oldNameLabel";
            this.oldNameLabel.Size = new System.Drawing.Size(224, 16);
            this.oldNameLabel.TabIndex = 0;
            this.oldNameLabel.Text = "label1";
            // 
            // newNameTextBox
            // 
            this.newNameTextBox.Location = new System.Drawing.Point(8, 32);
            this.newNameTextBox.Name = "newNameTextBox";
            this.newNameTextBox.Size = new System.Drawing.Size(224, 20);
            this.newNameTextBox.TabIndex = 1;
            this.newNameTextBox.Text = "textBox1";
            this.newNameTextBox.TextChanged += new System.EventHandler(this.NewNameChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(160, 64);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(72, 64);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            // 
            // RenameDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(248, 93);
            this.ControlBox = false;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.newNameTextBox);
            this.Controls.Add(this.oldNameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RenameDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rename file";
            this.ResumeLayout(false);

        }
        #endregion


        private void NewNameChanged(object sender, System.EventArgs e)
        {
            if ( this.newNameTextBox.Text != String.Empty &&
                String.Compare(this.newNameTextBox.Text, this.oldName, true) != 0 )
                this.okButton.Enabled = true;
            else 
                this.okButton.Enabled = false;
        }

        private System.Windows.Forms.Label oldNameLabel;
        private System.Windows.Forms.TextBox newNameTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private string oldName;

        
    }
}
