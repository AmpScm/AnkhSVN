namespace IssueZilla
{
    partial class EditIssueForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label componentLabel;
            System.Windows.Forms.Label subcomponentLabel;
            System.Windows.Forms.Label versionLabel;
            System.Windows.Forms.Label rep_platformLabel;
            System.Windows.Forms.Label priorityLabel;
            System.Windows.Forms.Label issue_typeLabel;
            System.Windows.Forms.Label op_sysLabel;
            System.Windows.Forms.Label short_descLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( EditIssueForm ) );
            this.componentComboBox = new System.Windows.Forms.ComboBox();
            this.subcomponentComboBox = new System.Windows.Forms.ComboBox();
            this.versionComboBox = new System.Windows.Forms.ComboBox();
            this.rep_platformComboBox = new System.Windows.Forms.ComboBox();
            this.priorityComboBox = new System.Windows.Forms.ComboBox();
            this.issue_typeComboBox = new System.Windows.Forms.ComboBox();
            this.op_sysComboBox = new System.Windows.Forms.ComboBox();
            this.short_descTextBox = new System.Windows.Forms.TextBox();
            this.commentsRichTextBox = new System.Windows.Forms.RichTextBox();
            this.postButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.actionComboBox = new System.Windows.Forms.ComboBox();
            this.issueBindingSource = new System.Windows.Forms.BindingSource( this.components );
            this.metadataSourceBindingSource = new System.Windows.Forms.BindingSource( this.components );
            this.editIssueActionPanel = new System.Windows.Forms.Panel();
            componentLabel = new System.Windows.Forms.Label();
            subcomponentLabel = new System.Windows.Forms.Label();
            versionLabel = new System.Windows.Forms.Label();
            rep_platformLabel = new System.Windows.Forms.Label();
            priorityLabel = new System.Windows.Forms.Label();
            issue_typeLabel = new System.Windows.Forms.Label();
            op_sysLabel = new System.Windows.Forms.Label();
            short_descLabel = new System.Windows.Forms.Label();
            ( (System.ComponentModel.ISupportInitialize)( this.issueBindingSource ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.metadataSourceBindingSource ) ).BeginInit();
            this.SuspendLayout();
            // 
            // componentLabel
            // 
            componentLabel.AutoSize = true;
            componentLabel.Location = new System.Drawing.Point( 15, 49 );
            componentLabel.Name = "componentLabel";
            componentLabel.Size = new System.Drawing.Size( 63, 13 );
            componentLabel.TabIndex = 1;
            componentLabel.Text = "component:";
            // 
            // subcomponentLabel
            // 
            subcomponentLabel.AutoSize = true;
            subcomponentLabel.Location = new System.Drawing.Point( -2, 76 );
            subcomponentLabel.Name = "subcomponentLabel";
            subcomponentLabel.Size = new System.Drawing.Size( 80, 13 );
            subcomponentLabel.TabIndex = 2;
            subcomponentLabel.Text = "subcomponent:";
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.Location = new System.Drawing.Point( 34, 103 );
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new System.Drawing.Size( 44, 13 );
            versionLabel.TabIndex = 4;
            versionLabel.Text = "version:";
            // 
            // rep_platformLabel
            // 
            rep_platformLabel.AutoSize = true;
            rep_platformLabel.Location = new System.Drawing.Point( 13, 130 );
            rep_platformLabel.Name = "rep_platformLabel";
            rep_platformLabel.Size = new System.Drawing.Size( 65, 13 );
            rep_platformLabel.TabIndex = 6;
            rep_platformLabel.Text = "rep platform:";
            // 
            // priorityLabel
            // 
            priorityLabel.AutoSize = true;
            priorityLabel.Location = new System.Drawing.Point( 38, 184 );
            priorityLabel.Name = "priorityLabel";
            priorityLabel.Size = new System.Drawing.Size( 40, 13 );
            priorityLabel.TabIndex = 8;
            priorityLabel.Text = "priority:";
            // 
            // issue_typeLabel
            // 
            issue_typeLabel.AutoSize = true;
            issue_typeLabel.Location = new System.Drawing.Point( 21, 22 );
            issue_typeLabel.Name = "issue_typeLabel";
            issue_typeLabel.Size = new System.Drawing.Size( 57, 13 );
            issue_typeLabel.TabIndex = 10;
            issue_typeLabel.Text = "issue type:";
            // 
            // op_sysLabel
            // 
            op_sysLabel.AutoSize = true;
            op_sysLabel.Location = new System.Drawing.Point( 38, 157 );
            op_sysLabel.Name = "op_sysLabel";
            op_sysLabel.Size = new System.Drawing.Size( 40, 13 );
            op_sysLabel.TabIndex = 12;
            op_sysLabel.Text = "op sys:";
            // 
            // short_descLabel
            // 
            short_descLabel.AutoSize = true;
            short_descLabel.Location = new System.Drawing.Point( 19, 211 );
            short_descLabel.Name = "short_descLabel";
            short_descLabel.Size = new System.Drawing.Size( 59, 13 );
            short_descLabel.TabIndex = 14;
            short_descLabel.Text = "short desc:";
            // 
            // componentComboBox
            // 
            this.componentComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "DataSource", this.metadataSourceBindingSource, "Components", true ) );
            this.componentComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "SelectedValue", this.issueBindingSource, "component", true ) );
            this.componentComboBox.DisplayMember = "DisplayText";
            this.componentComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.componentComboBox.FormattingEnabled = true;
            this.componentComboBox.Location = new System.Drawing.Point( 84, 46 );
            this.componentComboBox.Name = "componentComboBox";
            this.componentComboBox.Size = new System.Drawing.Size( 121, 21 );
            this.componentComboBox.TabIndex = 1;
            this.componentComboBox.ValueMember = "Key";
            // 
            // subcomponentComboBox
            // 
            this.subcomponentComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "DataSource", this.metadataSourceBindingSource, "SubComponents", true ) );
            this.subcomponentComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "SelectedValue", this.issueBindingSource, "subcomponent", true ) );
            this.subcomponentComboBox.DisplayMember = "DisplayText";
            this.subcomponentComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.subcomponentComboBox.FormattingEnabled = true;
            this.subcomponentComboBox.Location = new System.Drawing.Point( 84, 73 );
            this.subcomponentComboBox.Name = "subcomponentComboBox";
            this.subcomponentComboBox.Size = new System.Drawing.Size( 121, 21 );
            this.subcomponentComboBox.TabIndex = 2;
            this.subcomponentComboBox.ValueMember = "Key";
            // 
            // versionComboBox
            // 
            this.versionComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "DataSource", this.metadataSourceBindingSource, "Versions", true ) );
            this.versionComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "SelectedValue", this.issueBindingSource, "version", true ) );
            this.versionComboBox.DisplayMember = "DisplayText";
            this.versionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.versionComboBox.FormattingEnabled = true;
            this.versionComboBox.Location = new System.Drawing.Point( 84, 100 );
            this.versionComboBox.Name = "versionComboBox";
            this.versionComboBox.Size = new System.Drawing.Size( 121, 21 );
            this.versionComboBox.TabIndex = 3;
            this.versionComboBox.ValueMember = "Key";
            // 
            // rep_platformComboBox
            // 
            this.rep_platformComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "DataSource", this.metadataSourceBindingSource, "Platforms", true ) );
            this.rep_platformComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "SelectedValue", this.issueBindingSource, "rep_platform", true ) );
            this.rep_platformComboBox.DisplayMember = "DisplayText";
            this.rep_platformComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rep_platformComboBox.FormattingEnabled = true;
            this.rep_platformComboBox.Location = new System.Drawing.Point( 84, 127 );
            this.rep_platformComboBox.Name = "rep_platformComboBox";
            this.rep_platformComboBox.Size = new System.Drawing.Size( 121, 21 );
            this.rep_platformComboBox.TabIndex = 4;
            this.rep_platformComboBox.ValueMember = "Key";
            // 
            // priorityComboBox
            // 
            this.priorityComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "DataSource", this.metadataSourceBindingSource, "Priorities", true ) );
            this.priorityComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "SelectedValue", this.issueBindingSource, "priority", true ) );
            this.priorityComboBox.DisplayMember = "DisplayText";
            this.priorityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.priorityComboBox.FormattingEnabled = true;
            this.priorityComboBox.Location = new System.Drawing.Point( 84, 181 );
            this.priorityComboBox.Name = "priorityComboBox";
            this.priorityComboBox.Size = new System.Drawing.Size( 121, 21 );
            this.priorityComboBox.TabIndex = 6;
            this.priorityComboBox.ValueMember = "Key";
            // 
            // issue_typeComboBox
            // 
            this.issue_typeComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "DataSource", this.metadataSourceBindingSource, "IssueTypes", true ) );
            this.issue_typeComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "SelectedValue", this.issueBindingSource, "issue_type", true ) );
            this.issue_typeComboBox.DisplayMember = "DisplayText";
            this.issue_typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.issue_typeComboBox.FormattingEnabled = true;
            this.issue_typeComboBox.Location = new System.Drawing.Point( 84, 19 );
            this.issue_typeComboBox.Name = "issue_typeComboBox";
            this.issue_typeComboBox.Size = new System.Drawing.Size( 121, 21 );
            this.issue_typeComboBox.TabIndex = 0;
            this.issue_typeComboBox.ValueMember = "Key";
            // 
            // op_sysComboBox
            // 
            this.op_sysComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "DataSource", this.metadataSourceBindingSource, "OperatingSystems", true ) );
            this.op_sysComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "SelectedValue", this.issueBindingSource, "op_sys", true ) );
            this.op_sysComboBox.DisplayMember = "DisplayText";
            this.op_sysComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.op_sysComboBox.FormattingEnabled = true;
            this.op_sysComboBox.Location = new System.Drawing.Point( 84, 154 );
            this.op_sysComboBox.Name = "op_sysComboBox";
            this.op_sysComboBox.Size = new System.Drawing.Size( 121, 21 );
            this.op_sysComboBox.TabIndex = 5;
            this.op_sysComboBox.ValueMember = "Key";
            // 
            // short_descTextBox
            // 
            this.short_descTextBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.short_descTextBox.DataBindings.Add( new System.Windows.Forms.Binding( "Text", this.issueBindingSource, "short_desc", true ) );
            this.short_descTextBox.Location = new System.Drawing.Point( 84, 208 );
            this.short_descTextBox.Name = "short_descTextBox";
            this.short_descTextBox.Size = new System.Drawing.Size( 330, 20 );
            this.short_descTextBox.TabIndex = 7;
            // 
            // commentsRichTextBox
            // 
            this.commentsRichTextBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.commentsRichTextBox.Location = new System.Drawing.Point( 84, 324 );
            this.commentsRichTextBox.Name = "commentsRichTextBox";
            this.commentsRichTextBox.Size = new System.Drawing.Size( 330, 106 );
            this.commentsRichTextBox.TabIndex = 8;
            this.commentsRichTextBox.Text = "";
            // 
            // postButton
            // 
            this.postButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.postButton.Location = new System.Drawing.Point( 260, 450 );
            this.postButton.Name = "postButton";
            this.postButton.Size = new System.Drawing.Size( 75, 23 );
            this.postButton.TabIndex = 17;
            this.postButton.Text = "Post";
            this.postButton.UseVisualStyleBackColor = true;
            this.postButton.Click += new System.EventHandler( this.postButton_Click );
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point( 341, 450 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.cancelButton.TabIndex = 18;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // actionComboBox
            // 
            this.actionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.actionComboBox.FormattingEnabled = true;
            this.actionComboBox.Location = new System.Drawing.Point( 84, 238 );
            this.actionComboBox.Name = "actionComboBox";
            this.actionComboBox.Size = new System.Drawing.Size( 121, 21 );
            this.actionComboBox.TabIndex = 19;
            // 
            // issueBindingSource
            // 
            this.issueBindingSource.DataSource = typeof( Fines.IssueZillaLib.issue );
            // 
            // metadataSourceBindingSource
            // 
            this.metadataSourceBindingSource.DataSource = typeof( Fines.IssueZillaLib.IMetadataSource );
            // 
            // editIssueActionPanel
            // 
            this.editIssueActionPanel.Location = new System.Drawing.Point( 211, 234 );
            this.editIssueActionPanel.Name = "editIssueActionPanel";
            this.editIssueActionPanel.Size = new System.Drawing.Size( 203, 84 );
            this.editIssueActionPanel.TabIndex = 20;
            // 
            // EditIssueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 428, 485 );
            this.Controls.Add( this.editIssueActionPanel );
            this.Controls.Add( this.actionComboBox );
            this.Controls.Add( this.cancelButton );
            this.Controls.Add( this.postButton );
            this.Controls.Add( this.commentsRichTextBox );
            this.Controls.Add( short_descLabel );
            this.Controls.Add( this.short_descTextBox );
            this.Controls.Add( op_sysLabel );
            this.Controls.Add( this.op_sysComboBox );
            this.Controls.Add( issue_typeLabel );
            this.Controls.Add( this.issue_typeComboBox );
            this.Controls.Add( priorityLabel );
            this.Controls.Add( this.priorityComboBox );
            this.Controls.Add( rep_platformLabel );
            this.Controls.Add( this.rep_platformComboBox );
            this.Controls.Add( versionLabel );
            this.Controls.Add( this.versionComboBox );
            this.Controls.Add( subcomponentLabel );
            this.Controls.Add( this.subcomponentComboBox );
            this.Controls.Add( componentLabel );
            this.Controls.Add( this.componentComboBox );
            this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) );
            this.Name = "EditIssueForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New issue";
            ( (System.ComponentModel.ISupportInitialize)( this.issueBindingSource ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.metadataSourceBindingSource ) ).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource issueBindingSource;
        private System.Windows.Forms.ComboBox componentComboBox;
        private System.Windows.Forms.ComboBox subcomponentComboBox;
        private System.Windows.Forms.ComboBox versionComboBox;
        private System.Windows.Forms.ComboBox rep_platformComboBox;
        private System.Windows.Forms.ComboBox priorityComboBox;
        private System.Windows.Forms.ComboBox issue_typeComboBox;
        private System.Windows.Forms.ComboBox op_sysComboBox;
        private System.Windows.Forms.TextBox short_descTextBox;
        private System.Windows.Forms.RichTextBox commentsRichTextBox;
        private System.Windows.Forms.Button postButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.BindingSource metadataSourceBindingSource;
        private System.Windows.Forms.ComboBox actionComboBox;
        private System.Windows.Forms.Panel editIssueActionPanel;
    }
}