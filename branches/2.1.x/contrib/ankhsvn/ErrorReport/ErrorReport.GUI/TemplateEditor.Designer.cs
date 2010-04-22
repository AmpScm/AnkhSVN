namespace ErrorReport.GUI
{
    partial class TemplateEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( TemplateEditor ) );
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.ToolStripButton newButton;
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.templatesListView = new System.Windows.Forms.ListView();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.replyTemplateBindingSource = new System.Windows.Forms.BindingSource( this.components );
            this.timer = new System.Windows.Forms.Timer( this.components );
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            newButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize)( this.replyTemplateBindingSource ) ).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.richTextBox.AcceptsTab = true;
            this.richTextBox.DataBindings.Add( new System.Windows.Forms.Binding( "Text", this.replyTemplateBindingSource, "TemplateText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged ) );
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox.Location = new System.Drawing.Point( 0, 140 );
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size( 357, 121 );
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            this.richTextBox.TextChanged += new System.EventHandler( this.richTextBox_TextChanged );
            // 
            // templatesListView
            // 
            this.templatesListView.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            columnHeader1} );
            this.templatesListView.Dock = System.Windows.Forms.DockStyle.Top;
            this.templatesListView.Location = new System.Drawing.Point( 0, 0 );
            this.templatesListView.Name = "templatesListView";
            this.templatesListView.Size = new System.Drawing.Size( 357, 115 );
            this.templatesListView.TabIndex = 1;
            this.templatesListView.UseCompatibleStateImageBehavior = false;
            this.templatesListView.View = System.Windows.Forms.View.Details;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.deleteButton,
            newButton} );
            this.toolStrip.Location = new System.Drawing.Point( 0, 115 );
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size( 357, 25 );
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip1";
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.deleteButton.Image = ( (System.Drawing.Image)( resources.GetObject( "deleteButton.Image" ) ) );
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size( 42, 22 );
            this.deleteButton.Text = "Delete";
            this.deleteButton.Click += new System.EventHandler( this.deleteButton_Click );
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Template";
            columnHeader1.Width = 317;
            // 
            // newButton
            // 
            newButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            newButton.Image = ( (System.Drawing.Image)( resources.GetObject( "newButton.Image" ) ) );
            newButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            newButton.Name = "newButton";
            newButton.Size = new System.Drawing.Size( 32, 22 );
            newButton.Text = "New";
            newButton.Click += new System.EventHandler( this.newButton_Click );
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.okButton );
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point( 0, 261 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 357, 45 );
            this.panel1.TabIndex = 3;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point( 270, 10 );
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size( 75, 23 );
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler( this.okButton_Click );
            // 
            // replyTemplateBindingSource
            // 
            this.replyTemplateBindingSource.DataSource = typeof( ErrorReportExtractor.IReplyTemplate );
            // 
            // timer
            // 
            this.timer.Interval = 300;
            // 
            // TemplateEditor
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 357, 306 );
            this.Controls.Add( this.richTextBox );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.toolStrip );
            this.Controls.Add( this.templatesListView );
            this.Name = "TemplateEditor";
            this.Text = "TemplateEditor";
            this.toolStrip.ResumeLayout( false );
            this.toolStrip.PerformLayout();
            this.panel1.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize)( this.replyTemplateBindingSource ) ).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.ListView templatesListView;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.BindingSource replyTemplateBindingSource;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Timer timer;
    }
}