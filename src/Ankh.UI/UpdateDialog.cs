using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// A dialog for performing SVN updates.
	/// </summary>
	public class UpdateDialog : System.Windows.Forms.Form
	{

        /// <summary>
        /// Used to retrieve information about a path.
        /// </summary>
        public event GetPathInfoDelegate GetPathInfo
        {
            add{ this.pathSelectionTreeView.GetPathInfo += value; }
            remove{ this.pathSelectionTreeView.GetPathInfo -= value; }
        }
        

		public UpdateDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            this.revisionPicker.WorkingEnabled = false;

            this.revisionPicker.Changed +=new EventHandler(RevisionPickerChanged);
            this.RevisionPickerChanged(this, EventArgs.Empty);
		}

        /// <summary>
        /// The chosen revision
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NSvn.Core.Revision Revision
        {
            get{ return this.revisionPicker.Revision; }
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

        private void RevisionPickerChanged( object sender, EventArgs e )
        {
            this.okButton.Enabled = this.revisionPicker.Valid;
        }

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.pathSelectionTreeView = new Ankh.UI.PathSelectionTreeView();
            this.revisionGroupBox = new System.Windows.Forms.GroupBox();
            this.revisionPicker = new Ankh.UI.RevisionPicker();
            this.recursiveCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.revisionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(208, 352);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(294, 352);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
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
            this.pathSelectionTreeView.Size = new System.Drawing.Size(376, 208);
            this.pathSelectionTreeView.TabIndex = 2;
            this.pathSelectionTreeView.UrlPaths = false;
            // 
            // revisionGroupBox
            // 
            this.revisionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionGroupBox.Controls.Add(this.revisionPicker);
            this.revisionGroupBox.Location = new System.Drawing.Point(8, 216);
            this.revisionGroupBox.Name = "revisionGroupBox";
            this.revisionGroupBox.Size = new System.Drawing.Size(360, 72);
            this.revisionGroupBox.TabIndex = 3;
            this.revisionGroupBox.TabStop = false;
            this.revisionGroupBox.Text = "Revision";
            // 
            // revisionPicker
            // 
            this.revisionPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPicker.Location = new System.Drawing.Point(8, 32);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(344, 24);
            this.revisionPicker.TabIndex = 0;
            // 
            // recursiveCheckBox
            // 
            this.recursiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.recursiveCheckBox.Location = new System.Drawing.Point(10, 352);
            this.recursiveCheckBox.Name = "recursiveCheckBox";
            this.recursiveCheckBox.Size = new System.Drawing.Size(78, 24);
            this.recursiveCheckBox.TabIndex = 4;
            this.recursiveCheckBox.Text = "Recursive";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(8, 304);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(360, 40);
            this.label1.TabIndex = 5;
            this.label1.Text = "You can suppress this dialog by holding down the Shift key while clicking Update";
            // 
            // UpdateDialog
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(376, 381);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.recursiveCheckBox);
            this.Controls.Add(this.revisionGroupBox);
            this.Controls.Add(this.pathSelectionTreeView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.MinimumSize = new System.Drawing.Size(266, 280);
            this.Name = "UpdateDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update";
            this.revisionGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox revisionGroupBox;
        private Ankh.UI.RevisionPicker revisionPicker;
        private Ankh.UI.PathSelectionTreeView pathSelectionTreeView;
        private System.Windows.Forms.CheckBox recursiveCheckBox;
        private System.Windows.Forms.Label label1;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
	}
}
