using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class PropertyEditorDialog
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.nameLabel = new System.Windows.Forms.Label();
            this.nameCombo = new System.Windows.Forms.ComboBox();
            this.propListView = new System.Windows.Forms.ListView();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.valueColumn = new System.Windows.Forms.ColumnHeader();
            this.newButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.editorPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.Location = new System.Drawing.Point(24, 24);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(40, 16);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Name:";
            // 
            // nameCombo
            // 
            this.nameCombo.Location = new System.Drawing.Point(69, 21);
            this.nameCombo.Name = "nameCombo";
            this.nameCombo.Size = new System.Drawing.Size(121, 21);
            this.nameCombo.TabIndex = 1;
            this.nameCombo.TextChanged += new System.EventHandler(this.nameCombo_TextChanged);
            this.nameCombo.SelectedValueChanged += new System.EventHandler(this.nameCombo_SelectedValueChanged);
            // 
            // propListView
            // 
            this.propListView.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.propListView.AutoArrange = false;
            this.propListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                           this.nameColumn,
                                                                                           this.valueColumn});
            this.propListView.GridLines = true;
            this.propListView.Location = new System.Drawing.Point(0, 296);
            this.propListView.Name = "propListView";
            this.propListView.Scrollable = false;
            this.propListView.Size = new System.Drawing.Size(544, 112);
            this.propListView.TabIndex = 6;
            this.propListView.View = System.Windows.Forms.View.Details;
            this.propListView.SelectedIndexChanged += new System.EventHandler(this.propListView_SelectedIndexChanged);
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "Name";
            this.nameColumn.Width = 168;
            // 
            // valueColumn
            // 
            this.valueColumn.Text = "Value";
            this.valueColumn.Width = 544;
            // 
            // newButton
            // 
            this.newButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.newButton.Location = new System.Drawing.Point(8, 264);
            this.newButton.Name = "newButton";
            this.newButton.TabIndex = 3;
            this.newButton.Text = "Reset";
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(456, 264);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.TabIndex = 5;
            this.deleteButton.Text = "Delete";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(368, 416);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 7;
            this.okButton.Text = "Save";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(456, 416);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            // 
            // saveButton
            // 
            this.saveButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(372, 264);
            this.saveButton.Name = "saveButton";
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Add";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // editorPanel
            // 
            this.editorPanel.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.editorPanel.Location = new System.Drawing.Point(64, 56);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Size = new System.Drawing.Size(456, 192);
            this.editorPanel.TabIndex = 2;
            this.editorPanel.TabStop = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(192, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(328, 23);
            this.label1.TabIndex = 9;
            this.label1.Text = "Select an item from the list or type in your own property name";
            // 
            // PropertyEditorDialog
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(538, 447);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.label1,
                                                                          this.editorPanel,
                                                                          this.saveButton,
                                                                          this.cancelButton,
                                                                          this.okButton,
                                                                          this.deleteButton,
                                                                          this.newButton,
                                                                          this.propListView,
                                                                          this.nameCombo,
                                                                          this.nameLabel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PropertyEditorDialog";
            this.Text = "Edit Properties";
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.ComboBox nameCombo;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader valueColumn;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ListView propListView;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Panel editorPanel;
        private System.Windows.Forms.Label label1;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
    }
}
