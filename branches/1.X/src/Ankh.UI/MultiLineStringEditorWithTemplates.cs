using System;
using System.Text;
using System.Drawing;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog that lets the user edit a string, with templates.
    /// </summary>
    public class MultiLineStringEditorWithTemplates : MultiLineStringTypeEditorDialog
    {
    
        public MultiLineStringEditorWithTemplates()
        {
            this.InitializeComponent();
        }

        public void SetTemplates( StringEditorTemplate[] templates )
        {
            this.templatePopup = new PopUpListForm();
            this.templatePopup.ValueMember = "Value";
            this.templatePopup.DisplayMember = "Text";
            this.templatePopup.ToolTipMember = "ToolTip";
            this.templatePopup.DataSource = templates;
            this.templatePopup.SelectionCommitted += new EventHandler( templatePopup_SelectionCommitted );
        }

        
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Size = new System.Drawing.Size( 524, 157 );
            // 
            // panel1
            // 
            this.panel1.Controls.Add( label1 );
            this.panel1.Location = new System.Drawing.Point( 0, 157 );
            this.panel1.Size = new System.Drawing.Size( 524, 29 );
            this.panel1.Controls.SetChildIndex( label1, 0 );
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point( 12, 8 );
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size( 178, 13 );
            label1.TabIndex = 2;
            label1.Text = "Insert a template by pressing Ctrl-T";
            // 
            // MultiLineStringEditorWithTemplates
            // 
            this.ClientSize = new System.Drawing.Size( 524, 186 );
            this.KeyPreview = true;
            this.Name = "MultiLineStringEditorWithTemplates";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler( this.MultiLineStringEditorWithTemplates_KeyDown );
            this.panel1.ResumeLayout( false );
            this.panel1.PerformLayout();
            this.ResumeLayout( false );

        }

        private void MultiLineStringEditorWithTemplates_KeyDown( object sender, System.Windows.Forms.KeyEventArgs e )
        {
            if ( e.KeyCode == System.Windows.Forms.Keys.T && e.Control )
            {
                this.ShowTemplatePopup();
            }
        }

        private void ShowTemplatePopup()
        {
            this.templatePopup.Location = this.textBox.PointToScreen(
                this.textBox.GetPositionFromCharIndex( this.textBox.SelectionStart ) );

            using ( Graphics g = this.textBox.CreateGraphics() )
            {
                SizeF size = g.MeasureString( "A", this.textBox.Font );

                this.templatePopup.Top += (int)( size.Height );
                this.templatePopup.Left -= (int)( size.Width );
            }
            this.templatePopup.Show();
        }

        void templatePopup_SelectionCommitted( object sender, EventArgs e )
        {
            if ( this.templatePopup.SelectedValue != null )
            {
                this.textBox.AppendText( (string)this.templatePopup.SelectedValue );
            }
        }


        private PopUpListForm templatePopup;
    }

    /// <summary>
    /// A template in the dialog above.
    /// </summary>
    public class StringEditorTemplate
    {
        public string Text { get { return this.text; } }
        public string Value { get { return this.value; } }
        public string ToolTip { get { return this.toolTip; } }

        public StringEditorTemplate( string value, string toolTip, string text)
        {
            this.text = text;
            this.value = value;
            this.toolTip = toolTip;
        }

        public StringEditorTemplate( string value, string toolTip ): this(value, toolTip, value)
        {
        }

        public StringEditorTemplate( string value ) : this(value, value, value)
        {
            
        }

        private string text, value, toolTip;
    }
}
