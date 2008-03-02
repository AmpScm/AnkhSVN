using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using SharpSvn;



namespace Ankh.UI
{
    /// <summary>
    /// A dialog that allows the user to perform the Switch command.
    /// </summary>
    public partial class SwitchDialog : PathSelector
    {
        public SwitchDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.SingleSelection = true;
            this.TreeView.AfterCheck += new TreeViewEventHandler(AfterCheck);
            this.RevisionPickerStart.Changed += new EventHandler(RevisionPickerStart_Changed);
        }

        public string ToUrl
        {
            get{ return this.urlTextBox.Text; }
        }

        public string SelectedPath
        {
            get
            { 
                if ( this.TreeView.CheckedItems.Count > 0 )
                    return this.TreeView.CheckedItems[0].ToString();
                else 
                    return null;
                
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

        private void AfterCheck(object sender, TreeViewEventArgs e)
        {
            IList items = this.TreeView.CheckedItems;
            if ( items.Count > 0 )
            {
                ResolvingPathEventArgs args = new ResolvingPathEventArgs( items[0] );
                this.RaiseGetPathInfo( args );
                this.currentUrl = args.Path;
                this.urlTextBox.Text = this.currentUrl;
                this.urlTextBox.Enabled = true;
            }
            else
            {
                this.currentUrl = null;
                this.urlTextBox.Enabled = false;             
            }

            this.ValidateDialog();
    
        }
       
        private void urlTextBox_TextChanged(object sender, System.EventArgs e)
        {
            this.ValidateDialog();
        }

        private void RevisionPickerStart_Changed(object sender, EventArgs e)
        {
            this.ValidateDialog();
        }

        private void ValidateDialog()
        {
            bool hasUrl = this.currentUrl != null;
            bool userEnteredUrl = (this.currentUrl != this.urlTextBox.Text &&
                        this.urlTextBox.Text != String.Empty);
            bool hasNewRevision = this.RevisionPickerStart.Revision != SvnRevision.Head; 
            if ( hasUrl && (userEnteredUrl || hasNewRevision) )
            {
                this.OkButton.Enabled = true;
            }
            else
            {
                this.OkButton.Enabled = false;
            }
        }

        private string currentUrl;        
    }
}
