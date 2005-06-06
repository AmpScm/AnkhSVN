using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

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
    public class PathSelector : System.Windows.Forms.Form
    {
        /// <summary>
        /// Invoked when the treeview needs more information about a node.
        /// </summary>
        public event GetPathInfoDelegate GetPathInfo
        {
            add{ this.pathSelectionTreeView.GetPathInfo += value; }
            remove{ this.pathSelectionTreeView.GetPathInfo -= value; }
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

        public NSvn.Core.Revision RevisionStart
        {
            get{ return this.revisionPickerStart.Revision; }
            set{ this.revisionPickerStart.Revision = value; }
        }

        public NSvn.Core.Revision RevisionEnd
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PathSelector));
            this.pathSelectionTreeView = new Ankh.UI.PathSelectionTreeView();
            this.recursiveCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.revisionStartGroupBox = new System.Windows.Forms.GroupBox();
            this.revisionPickerStart = new Ankh.UI.RevisionPicker();
            this.revisionEndGroupBox = new System.Windows.Forms.GroupBox();
            this.revisionPickerEnd = new Ankh.UI.RevisionPicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.revisionStartGroupBox.SuspendLayout();
            this.revisionEndGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pathSelectionTreeView
            // 
            this.pathSelectionTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.pathSelectionTreeView.CheckBoxes = true;
            this.pathSelectionTreeView.ImageIndex = -1;
            this.pathSelectionTreeView.Items = new object[0];
            this.pathSelectionTreeView.Location = new System.Drawing.Point(0, 0);
            this.pathSelectionTreeView.Name = "pathSelectionTreeView";
            this.pathSelectionTreeView.Recursive = false;
            this.pathSelectionTreeView.SelectedImageIndex = -1;
            this.pathSelectionTreeView.SingleCheck = false;
            this.pathSelectionTreeView.Size = new System.Drawing.Size(368, 184);
            this.pathSelectionTreeView.TabIndex = 0;
            this.pathSelectionTreeView.UrlPaths = false;
            // 
            // recursiveCheckBox
            // 
            this.recursiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.recursiveCheckBox.Location = new System.Drawing.Point(8, 340);
            this.recursiveCheckBox.Name = "recursiveCheckBox";
            this.recursiveCheckBox.Size = new System.Drawing.Size(88, 24);
            this.recursiveCheckBox.TabIndex = 3;
            this.recursiveCheckBox.Text = "Recursive";
            this.recursiveCheckBox.CheckedChanged += new System.EventHandler(this.RecursiveCheckedChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(208, 344);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(288, 344);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            // 
            // revisionStartGroupBox
            // 
            this.revisionStartGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.revisionStartGroupBox.Controls.Add(this.revisionPickerStart);
            this.revisionStartGroupBox.Location = new System.Drawing.Point(0, 192);
            this.revisionStartGroupBox.Name = "revisionStartGroupBox";
            this.revisionStartGroupBox.Size = new System.Drawing.Size(368, 48);
            this.revisionStartGroupBox.TabIndex = 4;
            this.revisionStartGroupBox.TabStop = false;
            this.revisionStartGroupBox.Text = "Revision";
            // 
            // revisionPickerStart
            // 
            this.revisionPickerStart.Location = new System.Drawing.Point(8, 16);
            this.revisionPickerStart.Name = "revisionPickerStart";
            this.revisionPickerStart.Size = new System.Drawing.Size(352, 24);
            this.revisionPickerStart.TabIndex = 1;
            // 
            // revisionEndGroupBox
            // 
            this.revisionEndGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.revisionEndGroupBox.Controls.Add(this.revisionPickerEnd);
            this.revisionEndGroupBox.Location = new System.Drawing.Point(0, 248);
            this.revisionEndGroupBox.Name = "revisionEndGroupBox";
            this.revisionEndGroupBox.Size = new System.Drawing.Size(368, 48);
            this.revisionEndGroupBox.TabIndex = 5;
            this.revisionEndGroupBox.TabStop = false;
            this.revisionEndGroupBox.Text = "Revision end";
            // 
            // revisionPickerEnd
            // 
            this.revisionPickerEnd.Location = new System.Drawing.Point(8, 16);
            this.revisionPickerEnd.Name = "revisionPickerEnd";
            this.revisionPickerEnd.Size = new System.Drawing.Size(352, 24);
            this.revisionPickerEnd.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(0, 304);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 32);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(344, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "You can suppress this dialog by holding down the Shift key";
            // 
            // PathSelector
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(368, 373);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.revisionStartGroupBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.recursiveCheckBox);
            this.Controls.Add(this.pathSelectionTreeView);
            this.Controls.Add(this.revisionEndGroupBox);
            this.MinimumSize = new System.Drawing.Size(376, 0);
            this.Name = "PathSelector";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PathSelector";
            this.revisionStartGroupBox.ResumeLayout(false);
            this.revisionEndGroupBox.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private Ankh.UI.PathSelectionTreeView pathSelectionTreeView;
        private System.Windows.Forms.CheckBox recursiveCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.GroupBox revisionStartGroupBox;
        private Ankh.UI.RevisionPicker revisionPickerStart;
        private System.Windows.Forms.GroupBox revisionEndGroupBox;
        private Ankh.UI.RevisionPicker revisionPickerEnd;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;

        private PathSelectorOptions options;


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
