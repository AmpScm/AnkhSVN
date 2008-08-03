namespace IssueZilla
{
    partial class FilterControl
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
            this.components = new System.ComponentModel.Container();
            this.statusComboBox = new System.Windows.Forms.ComboBox();
            this.filterBindingSource = new System.Windows.Forms.BindingSource( this.components );
            this.label1 = new System.Windows.Forms.Label();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            ( (System.ComponentModel.ISupportInitialize)( this.filterBindingSource ) ).BeginInit();
            this.SuspendLayout();
            // 
            // statusComboBox
            // 
            this.statusComboBox.DataBindings.Add( new System.Windows.Forms.Binding( "SelectedItem", this.filterBindingSource, "SelectedStatus", true ) );
            this.statusComboBox.FormattingEnabled = true;
            this.statusComboBox.Location = new System.Drawing.Point( 64, 13 );
            this.statusComboBox.Name = "statusComboBox";
            this.statusComboBox.Size = new System.Drawing.Size( 140, 21 );
            this.statusComboBox.TabIndex = 0;
            // 
            // filterBindingSource
            // 
            this.filterBindingSource.DataSource = typeof( Fines.IssueZillaLib.Filter );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 20, 16 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 38, 13 );
            this.label1.TabIndex = 1;
            this.label1.Text = "Status";
            // 
            // searchTextBox
            // 
            this.searchTextBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.searchTextBox.Font = new System.Drawing.Font( "Tahoma", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.searchTextBox.Location = new System.Drawing.Point( 613, 13 );
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size( 100, 21 );
            this.searchTextBox.TabIndex = 2;
            this.searchTextBox.Text = "Search";
            this.searchTextBox.Enter += new System.EventHandler( this.searchTextBox_Enter );
            this.searchTextBox.Leave += new System.EventHandler( this.searchTextBox_Leave );
            this.searchTextBox.TextChanged += new System.EventHandler( this.searchTextBox_TextChanged );
            // 
            // FilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add( this.searchTextBox );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.statusComboBox );
            this.Name = "FilterControl";
            this.Size = new System.Drawing.Size( 716, 56 );
            ( (System.ComponentModel.ISupportInitialize)( this.filterBindingSource ) ).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox statusComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.BindingSource filterBindingSource;
        private System.Windows.Forms.TextBox searchTextBox;
    }
}
