using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Ankh.UI
{
    /// <summary>
    /// The dialog to lock SVN items.
    /// </summary>
    public partial class LockDialog : System.Windows.Forms.Form
	{
        PathSelectorInfo _info;
        IAnkhServiceProvider _context;

		public LockDialog()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

        public LockDialog(PathSelectorInfo info) : this()
        {
            this._info = info;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                EnsureSelection();
            }
        }

        void EnsureSelection()
        {
            Items = _info.VisibleItems;
            //selector.CheckedFilter = _info.CheckedFilter;
            Caption = _info.Caption;
            pathSelectionTreeView.CheckedFilter += _info.EvaluateChecked;
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
        /// The text to display in the label area.
        /// </summary>
        public string Caption
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        /// <summary>
        /// The items to put in the treeview.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICollection<SvnItem> Items
        {
            get { return this.pathSelectionTreeView.Items; }
            set { this.pathSelectionTreeView.Items = value; }
        }

        /// <summary>
        /// The items checked in the treeview.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<SvnItem> CheckedItems
        {
            get { return this.pathSelectionTreeView.CheckedItems; }
        }

        public string Message
        {
            get { return this.messageTextBox.Text; }
            set { this.messageTextBox.Text = value; }
        }

        public bool StealLocks
        {
            get { return this.stealLocksCheckBox.Checked; }
            set { this.stealLocksCheckBox.Checked = value; }
        }

        protected PathSelectionTreeView TreeView
        {
            get { return this.pathSelectionTreeView; }
        }

        protected Button OkButton
        {
            get { return this.okButton; }
        }

        protected Button DoCancelButton
        {
            get { return this.cancelButton; }
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

        private void pathSelectionTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            bool result = false;
            foreach (SvnItem item in this.CheckedItems)
            {
                // enable when a checked file is discovered.
                if (item.IsFile)
                {
                    result = true;
                    break;
                }
            }
            this.OkButton.Enabled = result;
        }
    }
}

