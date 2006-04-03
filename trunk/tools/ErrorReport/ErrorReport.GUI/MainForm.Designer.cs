namespace ErrorReport.GUI
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
            System.Windows.Forms.SplitContainer splitContainer1;
            System.Windows.Forms.SplitContainer splitContainer2;
            System.Windows.Forms.ColumnHeader columnDate;
            System.Windows.Forms.ColumnHeader columnVersion;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( MainForm ) );
            this.reportsListView = new System.Windows.Forms.ListView();
            this.columnFrom = new System.Windows.Forms.ColumnHeader();
            this.columnException = new System.Windows.Forms.ColumnHeader();
            this.splitContainerBottom = new System.Windows.Forms.SplitContainer();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.messageDetailRichTextBox = new System.Windows.Forms.RichTextBox();
            this.replyTextBox = new System.Windows.Forms.RichTextBox();
            this.columnDTEVersion = new System.Windows.Forms.ColumnHeader();
            this.progressCallback = new ErrorReport.GUI.ProgressCallbackControl();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            columnDate = new System.Windows.Forms.ColumnHeader();
            columnVersion = new System.Windows.Forms.ColumnHeader();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            this.splitContainerBottom.Panel1.SuspendLayout();
            this.splitContainerBottom.Panel2.SuspendLayout();
            this.splitContainerBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point( 0, 0 );
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add( splitContainer2 );
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add( this.splitContainerBottom );
            splitContainer1.Panel2.Controls.Add( this.toolStrip );
            splitContainer1.Size = new System.Drawing.Size( 971, 561 );
            splitContainer1.SplitterDistance = 307;
            splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point( 0, 0 );
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add( this.progressCallback );
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add( this.reportsListView );
            splitContainer2.Size = new System.Drawing.Size( 971, 307 );
            splitContainer2.SplitterDistance = 323;
            splitContainer2.TabIndex = 1;
            // 
            // reportsListView
            // 
            this.reportsListView.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            columnDate,
            this.columnFrom,
            this.columnException,
            columnVersion,
            this.columnDTEVersion} );
            this.reportsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportsListView.HideSelection = false;
            this.reportsListView.Location = new System.Drawing.Point( 0, 0 );
            this.reportsListView.Name = "reportsListView";
            this.reportsListView.Size = new System.Drawing.Size( 644, 307 );
            this.reportsListView.TabIndex = 0;
            this.reportsListView.UseCompatibleStateImageBehavior = false;
            this.reportsListView.View = System.Windows.Forms.View.Details;
            // 
            // columnDate
            // 
            columnDate.Text = "Date";
            columnDate.Width = 131;
            // 
            // columnFrom
            // 
            this.columnFrom.Text = "From";
            this.columnFrom.Width = 146;
            // 
            // columnException
            // 
            this.columnException.Text = "Exception";
            this.columnException.Width = 204;
            // 
            // splitContainerBottom
            // 
            this.splitContainerBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBottom.Location = new System.Drawing.Point( 0, 25 );
            this.splitContainerBottom.Name = "splitContainerBottom";
            // 
            // splitContainerBottom.Panel1
            // 
            this.splitContainerBottom.Panel1.Controls.Add( this.messageDetailRichTextBox );
            // 
            // splitContainerBottom.Panel2
            // 
            this.splitContainerBottom.Panel2.Controls.Add( this.replyTextBox );
            this.splitContainerBottom.Size = new System.Drawing.Size( 971, 225 );
            this.splitContainerBottom.SplitterDistance = 466;
            this.splitContainerBottom.TabIndex = 2;
            // 
            // toolStrip
            // 
            this.toolStrip.Location = new System.Drawing.Point( 0, 0 );
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size( 971, 25 );
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // messageDetailRichTextBox
            // 
            this.messageDetailRichTextBox.DataBindings.Add( new System.Windows.Forms.Binding( "Font", global::ErrorReport.GUI.Properties.Settings.Default, "MessageFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged ) );
            this.messageDetailRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageDetailRichTextBox.Font = global::ErrorReport.GUI.Properties.Settings.Default.MessageFont;
            this.messageDetailRichTextBox.Location = new System.Drawing.Point( 0, 0 );
            this.messageDetailRichTextBox.Name = "messageDetailRichTextBox";
            this.messageDetailRichTextBox.ReadOnly = true;
            this.messageDetailRichTextBox.Size = new System.Drawing.Size( 466, 225 );
            this.messageDetailRichTextBox.TabIndex = 1;
            this.messageDetailRichTextBox.Text = "";
            // 
            // replyTextBox
            // 
            this.replyTextBox.DataBindings.Add( new System.Windows.Forms.Binding( "Font", global::ErrorReport.GUI.Properties.Settings.Default, "ReplyFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged ) );
            this.replyTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replyTextBox.Font = global::ErrorReport.GUI.Properties.Settings.Default.ReplyFont;
            this.replyTextBox.Location = new System.Drawing.Point( 0, 0 );
            this.replyTextBox.Name = "replyTextBox";
            this.replyTextBox.Size = new System.Drawing.Size( 501, 225 );
            this.replyTextBox.TabIndex = 0;
            this.replyTextBox.Text = "";
            // 
            // columnVersion
            // 
            columnVersion.Text = "Version";
            // 
            // columnDTEVersion
            // 
            this.columnDTEVersion.Text = "DTE";
            // 
            // progressCallback
            // 
            this.progressCallback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressCallback.Location = new System.Drawing.Point( 0, 0 );
            this.progressCallback.Name = "progressCallback";
            this.progressCallback.Size = new System.Drawing.Size( 323, 307 );
            this.progressCallback.TabIndex = 0;
            this.progressCallback.VerboseMode = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 971, 561 );
            this.Controls.Add( splitContainer1 );
            this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) );
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Error reports";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler( this.MainForm_KeyDown );
            splitContainer1.Panel1.ResumeLayout( false );
            splitContainer1.Panel2.ResumeLayout( false );
            splitContainer1.Panel2.PerformLayout();
            splitContainer1.ResumeLayout( false );
            splitContainer2.Panel1.ResumeLayout( false );
            splitContainer2.Panel2.ResumeLayout( false );
            splitContainer2.ResumeLayout( false );
            this.splitContainerBottom.Panel1.ResumeLayout( false );
            this.splitContainerBottom.Panel2.ResumeLayout( false );
            this.splitContainerBottom.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.ListView reportsListView;
        private System.Windows.Forms.ColumnHeader columnFrom;
        private System.Windows.Forms.ColumnHeader columnException;
        private ProgressCallbackControl progressCallback;
        private System.Windows.Forms.RichTextBox messageDetailRichTextBox;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.SplitContainer splitContainerBottom;
        private System.Windows.Forms.RichTextBox replyTextBox;
        private System.Windows.Forms.ColumnHeader columnDTEVersion;
    }
}