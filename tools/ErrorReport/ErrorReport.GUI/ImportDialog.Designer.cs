namespace ErrorReport.GUI
{
    partial class ImportDialog
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
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.Panel panel1;
            System.Windows.Forms.GroupBox groupBox2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ImportDialog ) );
            this.folderTextBox = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.importButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.importReportsCheckBox = new System.Windows.Forms.CheckBox();
            this.importRepliesCheckBox = new System.Windows.Forms.CheckBox();
            this.itemsAfterPicker = new System.Windows.Forms.DateTimePicker();
            this.enableItemsAfterCheckBox = new System.Windows.Forms.CheckBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            panel1 = new System.Windows.Forms.Panel();
            groupBox2 = new System.Windows.Forms.GroupBox();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add( this.folderTextBox );
            groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            groupBox1.Location = new System.Drawing.Point( 5, 0 );
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size( 394, 65 );
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Folder";
            // 
            // folderTextBox
            // 
            this.folderTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.folderTextBox.Location = new System.Drawing.Point( 3, 17 );
            this.folderTextBox.Name = "folderTextBox";
            this.folderTextBox.Size = new System.Drawing.Size( 388, 21 );
            this.folderTextBox.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add( this.progressBar );
            panel1.Controls.Add( this.importButton );
            panel1.Controls.Add( this.cancelButton );
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point( 5, 225 );
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size( 394, 25 );
            panel1.TabIndex = 3;
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Enabled = false;
            this.progressBar.Location = new System.Drawing.Point( 0, 0 );
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size( 244, 25 );
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 2;
            this.progressBar.Visible = false;
            // 
            // importButton
            // 
            this.importButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.importButton.Location = new System.Drawing.Point( 244, 0 );
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size( 75, 25 );
            this.importButton.TabIndex = 0;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler( this.importButton_Click );
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.cancelButton.Location = new System.Drawing.Point( 319, 0 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 25 );
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // importReportsCheckBox
            // 
            this.importReportsCheckBox.AutoSize = true;
            this.importReportsCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.importReportsCheckBox.Location = new System.Drawing.Point( 5, 65 );
            this.importReportsCheckBox.Name = "importReportsCheckBox";
            this.importReportsCheckBox.Padding = new System.Windows.Forms.Padding( 0, 10, 0, 0 );
            this.importReportsCheckBox.Size = new System.Drawing.Size( 394, 27 );
            this.importReportsCheckBox.TabIndex = 1;
            this.importReportsCheckBox.Text = "Import reports";
            this.importReportsCheckBox.UseVisualStyleBackColor = true;
            // 
            // importRepliesCheckBox
            // 
            this.importRepliesCheckBox.AutoSize = true;
            this.importRepliesCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.importRepliesCheckBox.Location = new System.Drawing.Point( 5, 92 );
            this.importRepliesCheckBox.Name = "importRepliesCheckBox";
            this.importRepliesCheckBox.Padding = new System.Windows.Forms.Padding( 0, 10, 0, 0 );
            this.importRepliesCheckBox.Size = new System.Drawing.Size( 394, 27 );
            this.importRepliesCheckBox.TabIndex = 2;
            this.importRepliesCheckBox.Text = "Import replies";
            this.importRepliesCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add( this.enableItemsAfterCheckBox );
            groupBox2.Controls.Add( this.itemsAfterPicker );
            groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            groupBox2.Location = new System.Drawing.Point( 5, 125 );
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size( 394, 100 );
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            // 
            // itemsAfterPicker
            // 
            this.itemsAfterPicker.Location = new System.Drawing.Point( 3, 57 );
            this.itemsAfterPicker.Name = "itemsAfterPicker";
            this.itemsAfterPicker.Size = new System.Drawing.Size( 200, 21 );
            this.itemsAfterPicker.TabIndex = 0;
            // 
            // enableItemsAfterCheckBox
            // 
            this.enableItemsAfterCheckBox.AutoSize = true;
            this.enableItemsAfterCheckBox.Location = new System.Drawing.Point( 6, 20 );
            this.enableItemsAfterCheckBox.Name = "enableItemsAfterCheckBox";
            this.enableItemsAfterCheckBox.Size = new System.Drawing.Size( 140, 17 );
            this.enableItemsAfterCheckBox.TabIndex = 1;
            this.enableItemsAfterCheckBox.Text = "Only import items after:";
            this.enableItemsAfterCheckBox.UseVisualStyleBackColor = true;
            // 
            // ImportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size( 404, 250 );
            this.Controls.Add( groupBox2 );
            this.Controls.Add( panel1 );
            this.Controls.Add( this.importRepliesCheckBox );
            this.Controls.Add( this.importReportsCheckBox );
            this.Controls.Add( groupBox1 );
            this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) );
            this.Name = "ImportDialog";
            this.Padding = new System.Windows.Forms.Padding( 5, 0, 5, 0 );
            this.Text = "Import";
            groupBox1.ResumeLayout( false );
            groupBox1.PerformLayout();
            panel1.ResumeLayout( false );
            groupBox2.ResumeLayout( false );
            groupBox2.PerformLayout();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox folderTextBox;
        private System.Windows.Forms.CheckBox importReportsCheckBox;
        private System.Windows.Forms.CheckBox importRepliesCheckBox;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox enableItemsAfterCheckBox;
        private System.Windows.Forms.DateTimePicker itemsAfterPicker;
    }
}