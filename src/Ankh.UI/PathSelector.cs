using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
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

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// The text to display in the label area.
        /// </summary>
        public string LabelText
        {
            get
            {
                return this.textLabel.Text;
            }
            set
            {
                this.textLabel.Text = value; 
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
            this.pathSelectionTreeView = new Ankh.UI.PathSelectionTreeView();
            this.recursiveCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.textLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pathSelectionTreeView
            // 
            this.pathSelectionTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.pathSelectionTreeView.CheckBoxes = true;
            this.pathSelectionTreeView.ImageIndex = -1;
            this.pathSelectionTreeView.Location = new System.Drawing.Point(0, 0);
            this.pathSelectionTreeView.Name = "pathSelectionTreeView";
            //this.pathSelectionTreeView.Paths = new object[0];
            this.pathSelectionTreeView.Recursive = false;
            this.pathSelectionTreeView.SelectedImageIndex = -1;
            this.pathSelectionTreeView.SingleCheck = false;
            this.pathSelectionTreeView.Size = new System.Drawing.Size(400, 272);
            this.pathSelectionTreeView.TabIndex = 0;
            this.pathSelectionTreeView.UrlPaths = false;
            // 
            // recursiveCheckBox
            // 
            this.recursiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.recursiveCheckBox.Location = new System.Drawing.Point(8, 273);
            this.recursiveCheckBox.Name = "recursiveCheckBox";
            this.recursiveCheckBox.Size = new System.Drawing.Size(152, 24);
            this.recursiveCheckBox.TabIndex = 1;
            this.recursiveCheckBox.Text = "Recursive";
            this.recursiveCheckBox.CheckedChanged += new System.EventHandler(this.RecursiveCheckedChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(224, 328);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(320, 328);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            // 
            // textLabel
            // 
            this.textLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.textLabel.Location = new System.Drawing.Point(8, 304);
            this.textLabel.Name = "textLabel";
            this.textLabel.Size = new System.Drawing.Size(384, 16);
            this.textLabel.TabIndex = 4;
            // 
            // PathSelector
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(400, 357);
            this.ControlBox = false;
            this.Controls.Add(this.textLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.recursiveCheckBox);
            this.Controls.Add(this.pathSelectionTreeView);
            this.Name = "PathSelector";
            this.ShowInTaskbar = false;
            this.Text = "PathSelector";
            this.ResumeLayout(false);

        }
        #endregion

        private Ankh.UI.PathSelectionTreeView pathSelectionTreeView;
        private System.Windows.Forms.CheckBox recursiveCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label textLabel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

       
    }
}
