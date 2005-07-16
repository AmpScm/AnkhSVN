using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// A dialog for performing reverse merges.
	/// </summary>
	public class ReverseMergeDialog : System.Windows.Forms.Form
	{
        public event GetPathInfoDelegate GetPathInfo
        {
            add{ this.treeView.GetPathInfo += value; }
            remove{ this.treeView.GetPathInfo -= value; }
        }
        

		public ReverseMergeDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

        /// <summary>
        /// The items to choose from.
        /// </summary>
        public IList Items
        {
            get{ return this.treeView.Items; }
            set{ this.treeView.Items = value; }
        }

        /// <summary>
        /// The selected revision.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NSvn.Core.Revision Revision
        {
            get{ return this.revisionPicker.Revision; }
        }

        /// <summary>
        /// The selected items.
        /// </summary>
        public IList CheckedItems
        {
            get{ return this.treeView.CheckedItems; }
            set{ this.treeView.CheckedItems = value; }
        }

        /// <summary>
        /// Whether the operation should be recursive.
        /// </summary>
        public bool Recursive
        {
            get{ return this.recursiveCheckbox.Checked; }
            set{ this.recursiveCheckbox.Checked = this.treeView.Recursive = value; }
        }

        /// <summary>
        /// Whether to do a dry run.
        /// </summary>
        public bool DryRun
        {
            get{ return this.dryRunCheckBox.Checked; }
            set{ this.dryRunCheckBox.Checked = value; }
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

        private void recursiveCheckbox_CheckedChanged(object sender, System.EventArgs e)
        {
            this.treeView.Recursive = this.recursiveCheckbox.Checked;
        }

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.revisionPicker = new Ankh.UI.RevisionPicker();
            this.treeView = new Ankh.UI.PathSelectionTreeView();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.recursiveCheckbox = new System.Windows.Forms.CheckBox();
            this.dryRunCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // revisionPicker
            // 
            this.revisionPicker.Location = new System.Drawing.Point(8, 272);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(304, 20);
            this.revisionPicker.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.CheckBoxes = true;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeView.ImageIndex = -1;
            this.treeView.Items = new object[0];
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Recursive = false;
            this.treeView.SelectedImageIndex = -1;
            this.treeView.SingleCheck = false;
            this.treeView.Size = new System.Drawing.Size(338, 248);
            this.treeView.TabIndex = 1;
            this.treeView.UrlPaths = false;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(168, 328);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(256, 328);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 253);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(216, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "Choose the revision you want to revert to:";
            // 
            // recursiveCheckbox
            // 
            this.recursiveCheckbox.Location = new System.Drawing.Point(8, 304);
            this.recursiveCheckbox.Name = "recursiveCheckbox";
            this.recursiveCheckbox.TabIndex = 5;
            this.recursiveCheckbox.Text = "Recursive";
            this.recursiveCheckbox.CheckedChanged += new System.EventHandler(this.recursiveCheckbox_CheckedChanged);
            // 
            // dryRunCheckBox
            // 
            this.dryRunCheckBox.Location = new System.Drawing.Point(8, 328);
            this.dryRunCheckBox.Name = "dryRunCheckBox";
            this.dryRunCheckBox.TabIndex = 7;
            this.dryRunCheckBox.Text = "Dry run";
            // 
            // ReverseMergeDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(338, 359);
            this.ControlBox = false;
            this.Controls.Add(this.dryRunCheckBox);
            this.Controls.Add(this.recursiveCheckbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.revisionPicker);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ReverseMergeDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Reverse merge";
            this.ResumeLayout(false);

        }
		#endregion

        private Ankh.UI.RevisionPicker revisionPicker;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private Ankh.UI.PathSelectionTreeView treeView;
        private System.Windows.Forms.CheckBox recursiveCheckbox;
        private System.Windows.Forms.CheckBox dryRunCheckBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        
	}
}
