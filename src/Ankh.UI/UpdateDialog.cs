using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for performing SVN updates.
    /// </summary>
    public partial class UpdateDialog : System.Windows.Forms.Form
    {

        /// <summary>
        /// Used to retrieve information about a path.
        /// </summary>
        public event ResolvingPathInfoHandler GetPathInfo
        {
            add{ this.pathSelectionTreeView.ResolvingPathInfo += value; }
            remove{ this.pathSelectionTreeView.ResolvingPathInfo -= value; }
        }


        public UpdateDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.revisionPicker.WorkingEnabled = false;

            this.revisionPicker.Changed += new EventHandler(RevisionPickerChanged);
            this.RevisionPickerChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// The chosen revision
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnRevision Revision
        {
            get{ return this.revisionPicker.Revision; }
        }

        /// <summary>
        /// The items to put in the treeview.
        /// </summary>
        public IList Items
        {
            get{ return this.pathSelectionTreeView.Items; }
            set{ this.pathSelectionTreeView.Items = value; }
        }

        /// <summary>
        /// The items checked in the treeview.
        /// </summary>
        public IList CheckedItems
        {
            get{ return this.pathSelectionTreeView.CheckedItems; }
            set{ this.pathSelectionTreeView.CheckedItems = value; }
        }

        /// <summary>
        /// Whether the "Recursive" checkbox should be enabled
        /// </summary>
        public bool EnableRecursive
        {
            get{ return this.recursiveCheckBox.Enabled; }
            set{ this.recursiveCheckBox.Enabled = value; }
        }

        /// <summary>
        /// Whether the selection in the treeview is recursive.
        /// </summary>
        public bool Recursive
        {
            get{ return this.recursiveCheckBox.Checked; }
            set
            {
                this.recursiveCheckBox.Checked = value; 
                this.pathSelectionTreeView.Recursive = value;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void RevisionPickerChanged( object sender, EventArgs e )
        {
            this.okButton.Enabled = this.revisionPicker.Valid;
        }
    }
}
