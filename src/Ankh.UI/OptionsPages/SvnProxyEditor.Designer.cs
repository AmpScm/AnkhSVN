namespace Ankh.UI.OptionsPages
{
    partial class SvnProxyEditor
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
            this.proxyEnabled = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.proxyGroup = new System.Windows.Forms.GroupBox();
            this.exceptionsBox = new System.Windows.Forms.TextBox();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.usernameBox = new System.Windows.Forms.TextBox();
            this.portBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.hostBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.proxyGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // proxyEnabled
            // 
            this.proxyEnabled.AutoSize = true;
            this.proxyEnabled.Location = new System.Drawing.Point(12, 54);
            this.proxyEnabled.Name = "proxyEnabled";
            this.proxyEnabled.Size = new System.Drawing.Size(122, 17);
            this.proxyEnabled.TabIndex = 1;
            this.proxyEnabled.Text = "&Enable Proxy Server";
            this.proxyEnabled.UseVisualStyleBackColor = true;
            this.proxyEnabled.CheckedChanged += new System.EventHandler(this.proxyEnabled_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(406, 42);
            this.label1.TabIndex = 0;
            this.label1.Text = "In this window you can setup a proxy server for connecting with http and https se" +
                "rvers. These settings apply to all subversion clients using the global configura" +
                "tion.";
            // 
            // proxyGroup
            // 
            this.proxyGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.proxyGroup.Controls.Add(this.exceptionsBox);
            this.proxyGroup.Controls.Add(this.passwordBox);
            this.proxyGroup.Controls.Add(this.label6);
            this.proxyGroup.Controls.Add(this.label5);
            this.proxyGroup.Controls.Add(this.label4);
            this.proxyGroup.Controls.Add(this.usernameBox);
            this.proxyGroup.Controls.Add(this.portBox);
            this.proxyGroup.Controls.Add(this.label3);
            this.proxyGroup.Controls.Add(this.hostBox);
            this.proxyGroup.Controls.Add(this.label2);
            this.proxyGroup.Enabled = false;
            this.proxyGroup.Location = new System.Drawing.Point(12, 77);
            this.proxyGroup.Name = "proxyGroup";
            this.proxyGroup.Size = new System.Drawing.Size(406, 207);
            this.proxyGroup.TabIndex = 2;
            this.proxyGroup.TabStop = false;
            this.proxyGroup.Text = "Proxy Settings";
            // 
            // exceptionsBox
            // 
            this.exceptionsBox.AcceptsReturn = true;
            this.exceptionsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.exceptionsBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.exceptionsBox.Location = new System.Drawing.Point(78, 97);
            this.exceptionsBox.Multiline = true;
            this.exceptionsBox.Name = "exceptionsBox";
            this.exceptionsBox.Size = new System.Drawing.Size(197, 91);
            this.exceptionsBox.TabIndex = 9;
            // 
            // passwordBox
            // 
            this.passwordBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordBox.Location = new System.Drawing.Point(78, 71);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.PasswordChar = '*';
            this.passwordBox.Size = new System.Drawing.Size(197, 20);
            this.passwordBox.TabIndex = 7;
            this.passwordBox.UseSystemPasswordChar = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "&Exceptions:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "&Password:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "&Username:";
            // 
            // usernameBox
            // 
            this.usernameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.usernameBox.Location = new System.Drawing.Point(78, 48);
            this.usernameBox.Name = "usernameBox";
            this.usernameBox.Size = new System.Drawing.Size(197, 20);
            this.usernameBox.TabIndex = 5;
            // 
            // portBox
            // 
            this.portBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.portBox.Location = new System.Drawing.Point(332, 22);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(68, 20);
            this.portBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(297, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "&Port:";
            // 
            // hostBox
            // 
            this.hostBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hostBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.hostBox.Location = new System.Drawing.Point(78, 22);
            this.hostBox.Name = "hostBox";
            this.hostBox.Size = new System.Drawing.Size(197, 20);
            this.hostBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "&Server:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(343, 305);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(262, 305);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // SvnProxyEditor
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(430, 340);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.proxyGroup);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.proxyEnabled);
            this.Name = "SvnProxyEditor";
            this.Text = "Subversion Proxy Settings";
            this.proxyGroup.ResumeLayout(false);
            this.proxyGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox proxyEnabled;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox proxyGroup;
        private System.Windows.Forms.TextBox exceptionsBox;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox usernameBox;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox hostBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}