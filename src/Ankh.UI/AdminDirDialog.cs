using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// Summary description for AdminDirDialog.
    /// </summary>
    public partial class AdminDirDialog : System.Windows.Forms.Form
    {
        

        public AdminDirDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();            
        }

        /// <summary>
        /// The name of the admin dir.
        /// </summary>
        public string AdminDirName
        {
            get{ return this.adminDirTextBox.Text; }
            set{ this.adminDirTextBox.Text = value; }
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
    }
}
