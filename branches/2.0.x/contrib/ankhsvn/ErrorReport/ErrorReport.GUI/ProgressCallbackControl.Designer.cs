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
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox.Location = new System.Drawing.Point( 0, 0 );
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size( 385, 373 );
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            // 
            // ProgressCallbackControl
            // 
            this.Controls.Add( this.richTextBox );
            this.Name = "ProgressCallbackControl";
            this.Size = new System.Drawing.Size( 385, 373 );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox;
    }
}
