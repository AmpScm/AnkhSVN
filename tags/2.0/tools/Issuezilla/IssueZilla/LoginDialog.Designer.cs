namespace IssueZilla
{
    partial class LoginDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 12, 9 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 55, 13 );
            this.label1.TabIndex = 1;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 12, 35 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 53, 13 );
            this.label2.TabIndex = 3;
            this.label2.Text = "Password";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point( 172, 63 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point( 91, 63 );
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size( 75, 23 );
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.DataBindings.Add( new System.Windows.Forms.Binding( "Text", global::IssueZilla.Properties.Settings.Default, "Password", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged ) );
            this.passwordTextBox.Location = new System.Drawing.Point( 72, 32 );
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size( 175, 20 );
            this.passwordTextBox.TabIndex = 2;
            this.passwordTextBox.Text = global::IssueZilla.Properties.Settings.Default.Password;
            this.passwordTextBox.UseSystemPasswordChar = true;
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.DataBindings.Add( new System.Windows.Forms.Binding( "Text", global::IssueZilla.Properties.Settings.Default, "Username", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged ) );
            this.usernameTextBox.Location = new System.Drawing.Point( 72, 6 );
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size( 175, 20 );
            this.usernameTextBox.TabIndex = 0;
            this.usernameTextBox.Text = global::IssueZilla.Properties.Settings.Default.Username;
            // 
            // LoginDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size( 268, 98 );
            this.ControlBox = false;
            this.Controls.Add( this.okButton );
            this.Controls.Add( this.cancelButton );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.passwordTextBox );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.usernameTextBox );
            this.Name = "LoginDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Login";
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}