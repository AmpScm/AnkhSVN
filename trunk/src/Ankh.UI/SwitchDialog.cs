using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NSvn.Core;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog that allows the user to perform the Switch command.
    /// </summary>
    public class SwitchDialog : System.Windows.Forms.Form
    {
        

        public SwitchDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.items = new ArrayList();
            this.pathSelectionTreeView.SingleCheck = true;
            this.pathSelectionTreeView.AfterCheck += new TreeViewEventHandler(AfterCheck);
            this.pathSelectionTreeView.GetPathInfo += new GetPathInfoDelegate(pathSelectionTreeView_GetPathInfo);
        }

        /// <summary>
        /// The selected path the user wants to switch.
        /// </summary>
        public string Path
        {
            get{ return this.checkedItem.Path; }
        }

        /// <summary>
        /// The revision picked.
        /// </summary>
        public Revision Revision
        {
            get{ return this.revisionPicker.Revision; }
        }



        /// <summary>
        /// The selected URL the user wants to switch to.
        /// </summary>
        public string Url
        {
            get{ return this.urlTextBox.Text; }
        }

        /// <summary>
        /// Whether the switch operation should be recursive.
        /// </summary>
        public bool Recursive
        {
            get{ return this.recursiveCheckBox.Checked; }
        }

        /// <summary>
        /// Add a new item.
        /// </summary>
        /// <param name="path">The path of the item.</param>
        /// <param name="url">The existing URL of the item.</param>
        public void AddItem( string path, string url )
        {
            this.items.Add( new Item( path, url ) );
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
            IList items = this.pathSelectionTreeView.CheckedItems;
            if ( items.Count > 0 )
            {
                this.checkedItem = (Item)items[0];
                this.urlTextBox.Text = this.checkedItem.Url;
                this.urlTextBox.Enabled = true;
            }
            else
            {
                this.okButton.Enabled = false;
                this.urlTextBox.Enabled = false;
                this.checkedItem = null;
            }
    
        }

        private void SwitchDialog_Load(object sender, System.EventArgs e)
        {
            this.pathSelectionTreeView.Items = this.items;
        }

        private void urlTextBox_TextChanged(object sender, System.EventArgs e)
        {
            if ( this.checkedItem.Url != this.urlTextBox.Text &&
                this.urlTextBox.Text != String.Empty )
            {
                this.okButton.Enabled = true;
            }
            else
            {
                this.okButton.Enabled = false;
            }
        }

        private void pathSelectionTreeView_GetPathInfo(object sender, GetPathInfoEventArgs args)
        {
            Item item = (Item)args.Item;
            args.Path = item.Path;
        }

        private void recursiveCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            this.pathSelectionTreeView.Recursive = this.recursiveCheckBox.Checked;
        }

        private class Item
        {
            public Item( string path, string url )
            {
                this.Url = url;
                this.Path = path;
            }

            public override string ToString()
            {
                return this.Path;
            }

            public string Url;
            public string Path;
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pathSelectionTreeView = new Ankh.UI.PathSelectionTreeView();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.revisionPicker = new Ankh.UI.RevisionPicker();
            this.recursiveCheckBox = new System.Windows.Forms.CheckBox();
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
            this.pathSelectionTreeView.Size = new System.Drawing.Size(448, 248);
            this.pathSelectionTreeView.TabIndex = 0;
            this.pathSelectionTreeView.UrlPaths = false;
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.urlTextBox.Enabled = false;
            this.urlTextBox.Location = new System.Drawing.Point(8, 336);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(432, 20);
            this.urlTextBox.TabIndex = 1;
            this.urlTextBox.Text = "";
            this.urlTextBox.TextChanged += new System.EventHandler(this.urlTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Location = new System.Drawing.Point(8, 312);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "New URL:";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(256, 368);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(352, 368);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            // 
            // revisionPicker
            // 
            this.revisionPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.revisionPicker.Location = new System.Drawing.Point(0, 280);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(344, 24);
            this.revisionPicker.TabIndex = 5;
            // 
            // recursiveCheckBox
            // 
            this.recursiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.recursiveCheckBox.Location = new System.Drawing.Point(8, 248);
            this.recursiveCheckBox.Name = "recursiveCheckBox";
            this.recursiveCheckBox.TabIndex = 6;
            this.recursiveCheckBox.Text = "Recursive";
            this.recursiveCheckBox.CheckedChanged += new System.EventHandler(this.recursiveCheckBox_CheckedChanged);
            // 
            // SwitchDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(448, 397);
            this.ControlBox = false;
            this.Controls.Add(this.recursiveCheckBox);
            this.Controls.Add(this.revisionPicker);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.urlTextBox);
            this.Controls.Add(this.pathSelectionTreeView);
            this.MinimumSize = new System.Drawing.Size(352, 336);
            this.Name = "SwitchDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Switch";
            this.Load += new System.EventHandler(this.SwitchDialog_Load);
            this.ResumeLayout(false);

        }
        #endregion

        private ArrayList items;
        private Item checkedItem;

        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Ankh.UI.PathSelectionTreeView pathSelectionTreeView;
        private Ankh.UI.RevisionPicker revisionPicker;
        private System.Windows.Forms.CheckBox recursiveCheckBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        

        
    }
}
