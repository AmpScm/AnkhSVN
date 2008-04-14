using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SharpSvn;
using System.Collections.Generic;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for performing SVN updates.
    /// </summary>
    public partial class UpdateDialog : System.Windows.Forms.Form
    {
        IAnkhServiceProvider _context;

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

        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                if (value != _context)
                {
                    _context = value;
                    OnContextChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnContextChanged(EventArgs eventArgs)
        {
            pathSelectionTreeView.Context = Context;
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
        public ICollection<SvnItem> Items
        {
            get{ return this.pathSelectionTreeView.Items; }
            set{ this.pathSelectionTreeView.Items = value; }
        }

        /// <summary>
        /// The items checked in the treeview.
        /// </summary>
        public IEnumerable<SvnItem> CheckedItems
        {
            get{ return this.pathSelectionTreeView.CheckedItems; }
        }

		public Predicate<SvnItem> CheckedFilter
		{
			get { return this.pathSelectionTreeView.CheckedFilter; }
			set { this.pathSelectionTreeView.CheckedFilter = value; }
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
