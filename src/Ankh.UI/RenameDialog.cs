// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for entering a new name for a file.
    /// </summary>
    public partial class RenameDialog : System.Windows.Forms.Form
    {
        protected RenameDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oldName">The old name of the file</param>
        public RenameDialog( string oldName )
            : this()
        {            
            this.oldName = oldName;
            this.oldNameLabel.Text = String.Format( "New name for {0}:", this.oldName );
            this.newNameTextBox.Text = this.oldName;
        }

        /// <summary>
        /// The new name entered by the user.
        /// </summary>
        public string NewName
        {
            get{ return this.newNameTextBox.Text; }
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

        private void NewNameChanged(object sender, System.EventArgs e)
        {
            if ( this.newNameTextBox.Text != String.Empty &&
                String.Compare(this.newNameTextBox.Text, this.oldName, true) != 0 )
                this.okButton.Enabled = true;
            else 
                this.okButton.Enabled = false;
        }        

        private string oldName;

        
    }
}
