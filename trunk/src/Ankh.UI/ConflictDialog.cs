// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog that allows the user to resolve a conflicted file.
    /// </summary>
    public partial class ConflictDialog : System.Windows.Forms.Form
    {
        public ConflictDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        /// <summary>
        /// The selection made by the user.
        /// </summary>
        public string Selection
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.selectedChoice; }
        }

        /// <summary>
        /// The filenames to resolve, in the order {mine, new, old, base}.
        /// </summary>
        public string[] Filenames
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            { 
                return new string[]{
                                       (string)this.mineFileRadioButton.Tag,
                                       (string)this.newRevRadioButton.Tag,
                                       (string)this.oldRevRadioButton.Tag,
                                       (string)this.fileRadioButton.Tag
                                   };
            }

            [System.Diagnostics.DebuggerStepThrough]
            set
            { 
                Debug.Assert( value.Length == 4, "There should be 4 filenames" );
                this.mineFileRadioButton.Text = Path.GetFileName(value[0]);
                this.mineFileRadioButton.Tag = value[0];

                this.oldRevRadioButton.Text = Path.GetFileName(value[1]);
                this.oldRevRadioButton.Tag = value[1];

                this.newRevRadioButton.Text = Path.GetFileName(value[2]);
                this.newRevRadioButton.Tag = value[2];

                this.fileRadioButton.Text = Path.GetFileName(value[3]);
                this.fileRadioButton.Tag = value[3];
                this.selectedChoice = value[3];
                this.fileRadioButton.Checked = true;

                
            }
        }

        /// <summary>
        /// Whether this is a merge of two binary files.
        /// </summary>
        public bool Binary
        {
            get{ return this.binary; }
            set
            {
                this.binary = value;
                
                this.mineFileRadioButton.Enabled = 
                    this.mineFileRadioButton.Visible = !this.binary;

                // make sure there's at least one button checked.
                if ( this.binary )
                {
                    this.fileRadioButton.Checked = true;
                    this.selectedChoice = (string)this.fileRadioButton.Tag;
                }
            }
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

        private void selectedButton(object sender, System.EventArgs e)
        {
            this.selectedChoice = (string)((RadioButton) sender).Tag;
        }
		
        private string selectedChoice;        
        private bool binary = false;
    }

}

