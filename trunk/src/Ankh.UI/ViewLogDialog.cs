// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using SharpSvn;

namespace Ankh.UI
{
    /// <summary>
    /// Summary description for ViewLogDialog.
    /// </summary>
    public partial class ViewLogDialog : System.Windows.Forms.Form
    {
        //     public event EventHandler GetLog;
		
        public ViewLogDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.components = new System.ComponentModel.Container();
            this.SetToolTips();
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

        private void SetToolTips()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip viewLogToolTip = new ToolTip(this.components);

            // Set up the delays in milliseconds for the ToolTip.
            viewLogToolTip.AutoPopDelay = 5000;
            viewLogToolTip.InitialDelay = 1000;
            viewLogToolTip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            viewLogToolTip.ShowAlways = true;
         
            // Set up the ToolTip text for the Button and Checkbox.            
            viewLogToolTip.SetToolTip( this.singleRevisionCheckBox, 
                "Only one single revision is listed in the log ( false: activates the To revision field )" );            
            viewLogToolTip.SetToolTip( this.showRevisionCheckBox, 
                "Shows revision number in the log" );
            viewLogToolTip.SetToolTip( this.showDateCheckBox, 
                "Shows date in the log" );
            viewLogToolTip.SetToolTip( this.showAuthorCheckBox, 
                "Shows author in the log" ); 
            viewLogToolTip.SetToolTip( this.showMessageCheckBox, 
                "Shows message in the log" ); 
            viewLogToolTip.SetToolTip( this.showModifiedFilesCheckBox, 
                "Shows which files that have been changed" ); 
            viewLogToolTip.SetToolTip( this.getLogButton, 
                "Will generate a log");
            viewLogToolTip.SetToolTip( this.logRichTextBox, 
                "Lists log-information related to selected revisions");
            viewLogToolTip.SetToolTip( this.closeButton, 
                "");

        }

        private static readonly Regex validateRevisionNumber = 
            new Regex(@"\w{1,}", RegexOptions.Compiled);
       
        private void singleRevisionCheckBoxChecked(object sender, System.EventArgs e)
        {
            this.toRevision.Enabled = !this.singleRevisionCheckBox.Checked;
        }

        private void getLogButton_Click(object sender, System.EventArgs e)
        {
            this.logRichTextBox.Enabled = true;
        }

        private void fromRevision_Changed(object sender, System.EventArgs e)
        {
            if ( fromRevision.Revision == SvnRevision.Head )
                this.singleRevisionCheckBox.Checked = true;            
        }                                     
 
    }

}




