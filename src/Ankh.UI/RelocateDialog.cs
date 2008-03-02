using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for doing the equivalent of svn switch --relocate
    /// </summary>
    public partial class RelocateDialog : System.Windows.Forms.Form
    {
        public RelocateDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();      
      
            this.OnChange( this, EventArgs.Empty );
        }

        /// <summary>
        /// The current URL.
        /// </summary>
        public string CurrentUrl
        {
            get{ return this.currentUrlTextBox.Text; }
            set{ this.currentUrlTextBox.Text = value; }
        }

        /// <summary>
        /// The from segment.
        /// </summary>
        public string FromSegment
        {
            get{ return this.fromTextBox.Text; }
            set{ this.fromTextBox.Text = value; }
        }

        /// <summary>
        /// The to segment.
        /// </summary>
        public string ToSegment
        {
            get{ return this.toTextBox.Text; }
            set{ this.toTextBox.Text = value; }
        }

        /// <summary>
        /// Whether the user wants to do it recursively.
        /// </summary>
        public bool Recursive
        {
            get{ return this.recursiveCheckBox.Checked; }
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        private void OnChange( object sender, EventArgs e )
        {
            // make sure the stuff in the from textbox actually
            // is a part of the original URL.
            this.okButton.Enabled = 
                this.currentUrlTextBox.Text.StartsWith( this.fromTextBox.Text );
        }
    }
}
