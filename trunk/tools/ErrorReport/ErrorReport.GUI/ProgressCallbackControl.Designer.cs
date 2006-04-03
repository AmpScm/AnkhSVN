namespace ErrorReport.GUI
{
    partial class ProgressCallbackControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.richTextBox.Location = new System.Drawing.Point( 0, 0 );
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size( 385, 312 );
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Location = new System.Drawing.Point( 0, 312 );
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size( 385, 61 );
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 1;
            // 
            // ProgressCallbackControl
            // 
            this.Controls.Add( this.progressBar );
            this.Controls.Add( this.richTextBox );
            this.Name = "ProgressCallbackControl";
            this.Size = new System.Drawing.Size( 385, 373 );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}
