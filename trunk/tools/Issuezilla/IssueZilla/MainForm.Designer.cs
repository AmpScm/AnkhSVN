namespace IssueZilla
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( MainForm ) );
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.issueList = new IssueZilla.IssueList();
            this.filterControl = new IssueZilla.FilterControl();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem} );
            this.menuStrip1.Location = new System.Drawing.Point( 0, 0 );
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size( 1133, 24 );
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem} );
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size( 35, 20 );
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size( 109, 22 );
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler( this.ExecuteClick );
            // 
            // issueList
            // 
            this.issueList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.issueList.Issues = null;
            this.issueList.Location = new System.Drawing.Point( 0, 71 );
            this.issueList.Name = "issueList";
            this.issueList.Size = new System.Drawing.Size( 1133, 638 );
            this.issueList.TabIndex = 0;
            this.issueList.UrlFormat = null;
            // 
            // filterControl
            // 
            this.filterControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterControl.Location = new System.Drawing.Point( 0, 24 );
            this.filterControl.Name = "filterControl";
            this.filterControl.Size = new System.Drawing.Size( 1133, 47 );
            this.filterControl.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = global::IssueZilla.Properties.Settings.Default.FormSize;
            this.Controls.Add( this.issueList );
            this.Controls.Add( this.filterControl );
            this.Controls.Add( this.menuStrip1 );
            this.DataBindings.Add( new System.Windows.Forms.Binding( "Location", global::IssueZilla.Properties.Settings.Default, "FormLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged ) );
            this.DataBindings.Add( new System.Windows.Forms.Binding( "ClientSize", global::IssueZilla.Properties.Settings.Default, "FormSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged ) );
            this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) );
            this.KeyPreview = true;
            this.Location = global::IssueZilla.Properties.Settings.Default.FormLocation;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Issuezilla";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.MainForm_FormClosed );
            this.KeyDown += new System.Windows.Forms.KeyEventHandler( this.MainForm_KeyDown );
            this.Load += new System.EventHandler( this.MainForm_Load );
            this.menuStrip1.ResumeLayout( false );
            this.menuStrip1.PerformLayout();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private IssueList issueList;
        private FilterControl filterControl;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
    }
}