namespace IssueZilla.EditIssueUserControls
{
    partial class ResolveIssueUserControl
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
            this.resolutionComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // resolutionComboBox
            // 
            this.resolutionComboBox.DisplayMember = "DisplayText";
            this.resolutionComboBox.FormattingEnabled = true;
            this.resolutionComboBox.Location = new System.Drawing.Point( 69, 3 );
            this.resolutionComboBox.Name = "resolutionComboBox";
            this.resolutionComboBox.Size = new System.Drawing.Size( 121, 21 );
            this.resolutionComboBox.TabIndex = 0;
            this.resolutionComboBox.ValueMember = "Key";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 3, 6 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 60, 13 );
            this.label1.TabIndex = 1;
            this.label1.Text = "Resolve as";
            // 
            // ResolveIssueUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.resolutionComboBox );
            this.Name = "ResolveIssueUserControl";
            this.Size = new System.Drawing.Size( 196, 27 );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox resolutionComboBox;
        private System.Windows.Forms.Label label1;
    }
}
