using System;
using System.Text;
using System.Windows.Forms;
using Utils.Win32;
using System.IO;

namespace Ankh.UI
{
    public class AddWorkingCopyExplorerRootDialog : Form
    {
        
    
        public AddWorkingCopyExplorerRootDialog()
        {
            this.InitializeComponent();

            Win32.SHAutoComplete( this.workingCopyRootTextBox.Handle,
                Shacf.Filesystem );
        }

        public string NewRoot
        {
            get { return this.workingCopyRootTextBox.Text; }
        }


        #region InitializeComponent
        private void InitializeComponent()
        {
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.workingCopyRootTextBox = new System.Windows.Forms.TextBox();
            this.folderBrowser = new Utils.FolderBrowser();
            this.browseFolderButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point( 108, 47 );
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size( 75, 23 );
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point( 189, 47 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // workingCopyRootTextBox
            // 
            this.workingCopyRootTextBox.Location = new System.Drawing.Point( 12, 12 );
            this.workingCopyRootTextBox.Name = "workingCopyRootTextBox";
            this.workingCopyRootTextBox.Size = new System.Drawing.Size( 215, 21 );
            this.workingCopyRootTextBox.TabIndex = 2;
            this.workingCopyRootTextBox.TextChanged += new System.EventHandler( this.workingCopyRootTextBox_TextChanged );
            // 
            // browseFolderButton
            // 
            this.browseFolderButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.browseFolderButton.Location = new System.Drawing.Point( 233, 10 );
            this.browseFolderButton.Name = "browseFolderButton";
            this.browseFolderButton.Size = new System.Drawing.Size( 31, 23 );
            this.browseFolderButton.TabIndex = 3;
            this.browseFolderButton.Text = "...";
            this.browseFolderButton.UseVisualStyleBackColor = true;
            this.browseFolderButton.Click += new System.EventHandler( this.browseFolderButton_Click );
            // 
            // AddWorkingCopyExplorerRootDialog
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size( 276, 82 );
            this.Controls.Add( this.browseFolderButton );
            this.Controls.Add( this.workingCopyRootTextBox );
            this.Controls.Add( this.cancelButton );
            this.Controls.Add( this.okButton );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddWorkingCopyExplorerRootDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add working copy root";
            this.ResumeLayout( false );
            this.PerformLayout();

        }
        #endregion

        private void workingCopyRootTextBox_TextChanged( object sender, EventArgs e )
        {
            this.okButton.Enabled = Directory.Exists( this.workingCopyRootTextBox.Text );
        }

        

        private void browseFolderButton_Click( object sender, EventArgs e )
        {
            if ( folderBrowser.ShowDialog( this ) == DialogResult.OK )
            {
                this.workingCopyRootTextBox.Text = folderBrowser.DirectoryPath;
            }
        }

        private Button okButton;
        private TextBox workingCopyRootTextBox;
        private Utils.FolderBrowser folderBrowser;
        private Button browseFolderButton;
        private Button cancelButton;
    }
}
