using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SharpSvn;

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
        /// <summary>
        /// Invoked when the treeview needs more information about a node.
        /// </summary>
        public event EventHandler<ResolvingPathEventArgs> GetPathInfo
        {
            add
            { 
                this.getPathInfo += value;
                this.pathSelectionTreeView.ResolvingPathInfo += value; 
            }
            remove
            { 
                this.pathSelectionTreeView.ResolvingPathInfo -= value; 
                this.getPathInfo -= value;
            }
        }


        public PathSelector()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.Options = PathSelectorOptions.NoRevision;
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
        public IList Items
        {
            get{ return this.pathSelectionTreeView.Items; }
            set{ this.pathSelectionTreeView.Items = value; }
        }

        /// <summary>
        /// The items checked in the treeview.
        /// </summary>
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
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

        protected void RaiseGetPathInfo( ResolvingPathEventArgs args )
        {
            if ( this.getPathInfo != null )
                this.getPathInfo( this, args );
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
        private EventHandler<ResolvingPathEventArgs> getPathInfo;


        public static void Main()
        {
            PathSelector s = new PathSelector();
            s.Options = PathSelectorOptions.NoRevision;
            s.ShowDialog();

            s.Options = PathSelectorOptions.DisplaySingleRevision;
            s.ShowDialog();

            s.Options = PathSelectorOptions.DisplayRevisionRange;
            s.ShowDialog();
        }

       
    }
}
