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
            this.reportsIndexPanel = new System.Windows.Forms.Panel();
            this.reportsStartIndexTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.repliesIndexPanel = new System.Windows.Forms.Panel();
            this.repliesStartIndexTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            panel1 = new System.Windows.Forms.Panel();
            groupBox2 = new System.Windows.Forms.GroupBox();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            this.reportsIndexPanel.SuspendLayout();
            this.repliesIndexPanel.SuspendLayout();
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
            panel1.Location = new System.Drawing.Point( 5, 189 );
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
            // groupBox2
            // 
            groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            groupBox2.Location = new System.Drawing.Point( 5, 142 );
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size( 394, 47 );
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
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
            this.importRepliesCheckBox.Location = new System.Drawing.Point( 5, 123 );
            this.importRepliesCheckBox.Name = "importRepliesCheckBox";
            this.importRepliesCheckBox.Padding = new System.Windows.Forms.Padding( 0, 10, 0, 0 );
            this.importRepliesCheckBox.Size = new System.Drawing.Size( 394, 27 );
            this.importRepliesCheckBox.TabIndex = 2;
            this.importRepliesCheckBox.Text = "Import replies";
            this.importRepliesCheckBox.UseVisualStyleBackColor = true;
            // 
            // reportsIndexPanel
            // 
            this.reportsIndexPanel.Controls.Add( this.reportsStartIndexTextBox );
            this.reportsIndexPanel.Controls.Add( this.label1 );
            this.reportsIndexPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.reportsIndexPanel.Location = new System.Drawing.Point( 5, 92 );
            this.reportsIndexPanel.Name = "reportsIndexPanel";
            this.reportsIndexPanel.Padding = new System.Windows.Forms.Padding( 20, 5, 0, 0 );
            this.reportsIndexPanel.Size = new System.Drawing.Size( 394, 31 );
            this.reportsIndexPanel.TabIndex = 2;
            // 
            // reportsStartIndexTextBox
            // 
            this.reportsStartIndexTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.reportsStartIndexTextBox.Location = new System.Drawing.Point( 111, 5 );
            this.reportsStartIndexTextBox.Mask = "00000";
            this.reportsStartIndexTextBox.Name = "reportsStartIndexTextBox";
            this.reportsStartIndexTextBox.Size = new System.Drawing.Size( 100, 21 );
            this.reportsStartIndexTextBox.TabIndex = 1;
            this.reportsStartIndexTextBox.ValidatingType = typeof( int );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point( 20, 5 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 91, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Starting at index:";
            // 
            // repliesIndexPanel
            // 
            this.repliesIndexPanel.Controls.Add( this.repliesStartIndexTextBox );
            this.repliesIndexPanel.Controls.Add( this.label2 );
            this.repliesIndexPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.repliesIndexPanel.Location = new System.Drawing.Point( 5, 150 );
            this.repliesIndexPanel.Name = "repliesIndexPanel";
            this.repliesIndexPanel.Padding = new System.Windows.Forms.Padding( 20, 5, 0, 0 );
            this.repliesIndexPanel.Size = new System.Drawing.Size( 394, 31 );
            this.repliesIndexPanel.TabIndex = 3;
            // 
            // repliesStartIndexTextBox
            // 
            this.repliesStartIndexTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.repliesStartIndexTextBox.Location = new System.Drawing.Point( 111, 5 );
            this.repliesStartIndexTextBox.Mask = "00000";
            this.repliesStartIndexTextBox.Name = "repliesStartIndexTextBox";
            this.repliesStartIndexTextBox.Size = new System.Drawing.Size( 100, 21 );
            this.repliesStartIndexTextBox.TabIndex = 1;
            this.repliesStartIndexTextBox.ValidatingType = typeof( int );
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point( 20, 5 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 91, 13 );
            this.label2.TabIndex = 0;
            this.label2.Text = "Starting at index:";
            // 
            // ImportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size( 404, 214 );
            this.Controls.Add( this.repliesIndexPanel );
            this.Controls.Add( this.importRepliesCheckBox );
            this.Controls.Add( this.reportsIndexPanel );
            this.Controls.Add( groupBox2 );
            this.Controls.Add( panel1 );
            this.Controls.Add( this.importReportsCheckBox );
            this.Controls.Add( groupBox1 );
            this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) );
            this.Name = "ImportDialog";
            this.Padding = new System.Windows.Forms.Padding( 5, 0, 5, 0 );
            this.Text = "Import";
            groupBox1.ResumeLayout( false );
            groupBox1.PerformLayout();
            panel1.ResumeLayout( false );
            this.reportsIndexPanel.ResumeLayout( false );
            this.reportsIndexPanel.PerformLayout();
            this.repliesIndexPanel.ResumeLayout( false );
            this.repliesIndexPanel.PerformLayout();
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
        private System.Windows.Forms.Panel reportsIndexPanel;
        private System.Windows.Forms.MaskedTextBox reportsStartIndexTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel repliesIndexPanel;
        private System.Windows.Forms.MaskedTextBox repliesStartIndexTextBox;
        private System.Windows.Forms.Label label2;
    }
}