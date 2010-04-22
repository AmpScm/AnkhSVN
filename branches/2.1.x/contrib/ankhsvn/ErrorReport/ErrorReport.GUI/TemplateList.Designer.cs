namespace ErrorReport.GUI
{
    partial class TemplateList
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
            this.listBox = new System.Windows.Forms.ListBox();
            this.replyTemplateBindingSource = new System.Windows.Forms.BindingSource( this.components );
            this.toolTip = new System.Windows.Forms.ToolTip( this.components );
            this.timer = new System.Windows.Forms.Timer( this.components );
            ( (System.ComponentModel.ISupportInitialize)( this.replyTemplateBindingSource ) ).BeginInit();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.DataBindings.Add( new System.Windows.Forms.Binding( "SelectedValue", this.replyTemplateBindingSource, "TemplateText", true ) );
            this.listBox.DataSource = this.replyTemplateBindingSource;
            this.listBox.DisplayMember = "TemplateText";
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point( 0, 0 );
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size( 242, 199 );
            this.listBox.TabIndex = 0;
            // 
            // replyTemplateBindingSource
            // 
            this.replyTemplateBindingSource.DataSource = typeof( ErrorReportExtractor.IReplyTemplate );
            // 
            // timer
            // 
            this.timer.Interval = 300;
            // 
            // TemplateList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 242, 199 );
            this.Controls.Add( this.listBox );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "TemplateList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TemplateList";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler( this.TemplateList_KeyDown );
            ( (System.ComponentModel.ISupportInitialize)( this.replyTemplateBindingSource ) ).EndInit();
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.BindingSource replyTemplateBindingSource;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer timer;
    }
}