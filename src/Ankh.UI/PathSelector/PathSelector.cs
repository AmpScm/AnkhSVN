using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SharpSvn;
using System.Collections.Generic;

namespace Ankh.UI
{
    public enum PathSelectorOptions
    {
        NoRevision,
        DisplaySingleRevision,
        DisplayRevisionRange
    }
    /// <summary>
    /// Summary description for PathSelector.
    /// </summary>
    public partial class PathSelector : System.Windows.Forms.Form
    {
        PathSelectorInfo _info;
        IAnkhServiceProvider _context;



        public PathSelector()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.Options = PathSelectorOptions.NoRevision;
        }

        public PathSelector(PathSelectorInfo info)
            :this()
        {
            _info = info;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if(!DesignMode)
                EnsureSelection();
        }

        void EnsureSelection()
        {
            EnableRecursive = _info.EnableRecursive;
            Items = _info.VisibleItems;
            //selector.CheckedFilter = _info.CheckedFilter;
            Recursive = _info.Depth == SvnDepth.Infinity;
            SingleSelection = _info.SingleSelection;
            Caption = _info.Caption;

            // do we need go get a revision range?
            if (_info.RevisionStart == null && _info.RevisionEnd == null)
            {
                Options = PathSelectorOptions.NoRevision;
            }
            else if (_info.RevisionEnd == null)
            {
                RevisionStart = _info.RevisionStart;
                Options = PathSelectorOptions.DisplaySingleRevision;
            }
            else
            {
                RevisionStart = _info.RevisionStart;
                RevisionEnd = _info.RevisionEnd;
                Options = PathSelectorOptions.DisplayRevisionRange;
            }
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
            get{ return this.pathSelectionTreeView.Items; }
            set{ this.pathSelectionTreeView.Items = value; }
        }

		/// <summary>
		/// 
		/// </summary>
		public Predicate<SvnItem> CheckedFilter
		{
			get { return pathSelectionTreeView.CheckedFilter; }
			set { pathSelectionTreeView.CheckedFilter = value; }
		}

        /// <summary>
        /// The items checked in the treeview.
        /// </summary>
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public IEnumerable<SvnItem> CheckedItems
        {
            get{ return this.pathSelectionTreeView.CheckedItems; }
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
        /// Whether only a single item can be checked.
        /// </summary>
        public bool SingleSelection
        {
            get{ return this.pathSelectionTreeView.SingleCheck; }
            set{ this.pathSelectionTreeView.SingleCheck = value; }
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnRevision RevisionStart
        {
            get{ return this.revisionPickerStart.Revision; }
            set{ this.revisionPickerStart.Revision = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnRevision RevisionEnd
        {
            get{ return this.revisionPickerEnd.Revision; }
            set{ this.revisionPickerEnd.Revision = value; }
        }

        public PathSelectorOptions Options
        {
            get{ return this.options; }
            set
            {
                this.options = value;
                switch( this.options )
                {
                    case PathSelectorOptions.NoRevision:
                        this.revisionEndGroupBox.Enabled = false;
                        this.revisionStartGroupBox.Enabled = false;
                        break;
                    case PathSelectorOptions.DisplaySingleRevision:
                        this.revisionStartGroupBox.Text = "Revision";
                        this.revisionStartGroupBox.Enabled = true;
                        this.revisionEndGroupBox.Enabled = false;
                        break;
                    case PathSelectorOptions.DisplayRevisionRange:
                        this.revisionStartGroupBox.Enabled = true;
                        this.revisionStartGroupBox.Text = "Revision start";
                        this.revisionEndGroupBox.Enabled = true;
                        break;
                    default:
                        throw new ArgumentException( "Invalid value for Options" );
                }
            }
        }

        protected PathSelectionTreeView TreeView
        {
            get{ return this.pathSelectionTreeView; }
        }

        protected Button OkButton
        {
            get{ return this.okButton; }
        }

        protected Button DoCancelButton
        {
            get{ return this.cancelButton; }
        }

        protected RevisionPicker RevisionPickerStart
        {
            get{ return this.revisionPickerStart; }
        }

        protected RevisionPicker RevisionPickerEnd
        {
            get{ return this.revisionPickerEnd; }
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

        private void RecursiveCheckedChanged(object sender, System.EventArgs e)
        {
            this.pathSelectionTreeView.Recursive = this.recursiveCheckBox.Checked;        
        }

        private PathSelectorOptions options;        
    }
}
