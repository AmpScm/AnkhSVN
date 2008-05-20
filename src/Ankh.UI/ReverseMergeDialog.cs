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
    /// A dialog for performing reverse merges.
    /// </summary>
    public partial class ReverseMergeDialog : System.Windows.Forms.Form
    {
        IAnkhServiceProvider _context;

        public ReverseMergeDialog()
        {
            InitializeComponent();
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
            treeView.Context = Context;
        }

        /// <summary>
        /// The items to choose from.
        /// </summary>
        public ICollection<SvnItem> Items
        {
            get { return this.treeView.Items; }
            set { this.treeView.Items = value; }
        }

        /// <summary>
        /// The selected revision.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnRevision Revision
        {
            get { return this.revisionPicker.Revision; }
        }

        /// <summary>
        /// The selected items.
        /// </summary>
        public IEnumerable<SvnItem> CheckedItems
        {
            get { return this.treeView.CheckedItems; }
        }



        /// <summary>
        /// Whether the operation should be recursive.
        /// </summary>
        public bool Recursive
        {
            get { return this.recursiveCheckbox.Checked; }
            set { this.recursiveCheckbox.Checked = this.treeView.Recursive = value; }
        }

        /// <summary>
        /// Whether to do a dry run.
        /// </summary>
        public bool DryRun
        {
            get { return this.dryRunCheckBox.Checked; }
            set { this.dryRunCheckBox.Checked = value; }
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

        private void recursiveCheckbox_CheckedChanged(object sender, System.EventArgs e)
        {
            this.treeView.Recursive = this.recursiveCheckbox.Checked;
        }

        private void pathsLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
