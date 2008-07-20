﻿using System;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyEditorDialog));
            this.propListView = new System.Windows.Forms.ListView();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.valueColumn = new System.Windows.Forms.ColumnHeader();
            this.editButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.propGroupBox = new System.Windows.Forms.GroupBox();
            this.svnItemLabel = new System.Windows.Forms.Label();
            this.propGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // propListView
            // 
            resources.ApplyResources(this.propListView, "propListView");
            this.propListView.AutoArrange = false;
            this.propListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.valueColumn});
            this.propListView.FullRowSelect = true;
            this.propListView.GridLines = true;
            this.propListView.Name = "propListView";
            this.propListView.UseCompatibleStateImageBehavior = false;
            this.propListView.View = System.Windows.Forms.View.Details;
            this.propListView.SelectedIndexChanged += new System.EventHandler(this.propListView_SelectedIndexChanged);
            // 
            // nameColumn
            // 
            resources.ApplyResources(this.nameColumn, "nameColumn");
            // 
            // valueColumn
            // 
            resources.ApplyResources(this.valueColumn, "valueColumn");
            // 
            // editButton
            // 
            resources.ApplyResources(this.editButton, "editButton");
            this.editButton.Name = "editButton";
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // deleteButton
            // 
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // addButton
            // 
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.Name = "addButton";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // propGroupBox
            // 
            this.propGroupBox.Controls.Add(this.svnItemLabel);
            this.propGroupBox.Controls.Add(this.propListView);
            this.propGroupBox.Controls.Add(this.addButton);
            this.propGroupBox.Controls.Add(this.deleteButton);
            this.propGroupBox.Controls.Add(this.editButton);
            resources.ApplyResources(this.propGroupBox, "propGroupBox");
            this.propGroupBox.Name = "propGroupBox";
            this.propGroupBox.TabStop = false;
            // 
            // svnItemLabel
            // 
            resources.ApplyResources(this.svnItemLabel, "svnItemLabel");
            this.svnItemLabel.Name = "svnItemLabel";
            // 
            // PropertyEditorDialog
            // 
            this.AcceptButton = this.addButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.propGroupBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyEditorDialog";
            this.ShowInTaskbar = false;
            this.propGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader valueColumn;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ListView propListView;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button addButton;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.GroupBox propGroupBox;
        private System.Windows.Forms.Label svnItemLabel;
    }
}
