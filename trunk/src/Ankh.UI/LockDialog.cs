using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Ankh.UI
{
    /// <summary>
    /// The dialog to lock SVN items.
    /// </summary>
    public partial class LockDialog : VSContainerForm
	{
        PathSelectorInfo _info;

		public LockDialog()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
            ContainerMode = VSContainerMode.UseTextEditorScope | VSContainerMode.TranslateKeys;
		}

        public LockDialog(PathSelectorInfo info) : this()
        {
            this._info = info;
        }

        bool _initialized, _hooked;
        void Initialize()
        {
            if (!_initialized && Context != null)
            {
                logMessageEditor.Init(Context, true);
                _initialized = true;
            }

            if (!_hooked && _initialized && Context != null && IsHandleCreated)
            {
                AddCommandTarget(logMessageEditor.CommandTarget);
                AddWindowPane(logMessageEditor.WindowPane);
                _hooked = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                EnsureSelection();
            }
            Initialize();
            Message = _originalText;
        }

        void EnsureSelection()
        {
            Items = _info.VisibleItems;
            //selector.CheckedFilter = _info.CheckedFilter;
            Caption = _info.Caption;
            pathSelectionTreeView.CheckedFilter += _info.EvaluateChecked;
        }

        protected override  void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);

            pathSelectionTreeView.Context = Context;
            Initialize();
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

        string _originalText;
        public string Message
        {
            get { return this.logMessageEditor.Text; }
            set { this.logMessageEditor.Text = _originalText = value; }
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

