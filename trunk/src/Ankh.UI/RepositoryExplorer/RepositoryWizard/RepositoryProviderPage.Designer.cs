namespace Ankh.UI.RepositoryExplorer.RepositoryWizard
{
    partial class RepositoryProviderPage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepositoryProviderPage));
            this.providerPanel = new System.Windows.Forms.TableLayoutPanel();
            this.urlRadioButton = new System.Windows.Forms.RadioButton();
            this.urlComboBox = new System.Windows.Forms.ComboBox();
            this.providerRadioButton = new System.Windows.Forms.RadioButton();
            this.wikiLinkLabel = new System.Windows.Forms.LinkLabel();
            this.providerListView = new Ankh.UI.VSSelectionControls.SmartListView();
            this.cloudForgeControl1 = new Ankh.UI.Controls.CloudForgeControl();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.wikiLinkLabel1 = new System.Windows.Forms.LinkLabel();
            this.providerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // providerPanel
            // 
            resources.ApplyResources(this.providerPanel, "providerPanel");
            this.providerPanel.Controls.Add(this.urlRadioButton, 0, 0);
            this.providerPanel.Controls.Add(this.urlComboBox, 0, 1);
            this.providerPanel.Controls.Add(this.providerRadioButton, 0, 3);
            this.providerPanel.Controls.Add(this.wikiLinkLabel, 0, 2);
            this.providerPanel.Controls.Add(this.providerListView, 0, 4);
            this.providerPanel.Controls.Add(this.wikiLinkLabel1, 0, 5);
            this.providerPanel.Name = "providerPanel";
            // 
            // urlRadioButton
            // 
            resources.ApplyResources(this.urlRadioButton, "urlRadioButton");
            this.urlRadioButton.Checked = true;
            this.providerPanel.SetColumnSpan(this.urlRadioButton, 2);
            this.urlRadioButton.Name = "urlRadioButton";
            this.urlRadioButton.TabStop = true;
            this.urlRadioButton.UseVisualStyleBackColor = true;
            this.urlRadioButton.CheckedChanged += new System.EventHandler(this.urlRadioButton_CheckedChanged);
            // 
            // urlComboBox
            // 
            resources.ApplyResources(this.urlComboBox, "urlComboBox");
            this.urlComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.urlComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.providerPanel.SetColumnSpan(this.urlComboBox, 2);
            this.urlComboBox.FormattingEnabled = true;
            this.urlComboBox.Name = "urlComboBox";
            this.urlComboBox.TextChanged += new System.EventHandler(this.urlComboBox_TextChanged);
            // 
            // providerRadioButton
            // 
            resources.ApplyResources(this.providerRadioButton, "providerRadioButton");
            this.providerRadioButton.Name = "providerRadioButton";
            this.providerRadioButton.TabStop = true;
            this.providerRadioButton.UseVisualStyleBackColor = true;
            this.providerRadioButton.CheckedChanged += new System.EventHandler(this.providerRadioButton_CheckedChanged);
            // 
            // wikiLinkLabel
            // 
            resources.ApplyResources(this.wikiLinkLabel, "wikiLinkLabel");
            this.providerPanel.SetColumnSpan(this.wikiLinkLabel, 2);
            this.wikiLinkLabel.Name = "wikiLinkLabel";
            this.wikiLinkLabel.TabStop = true;
            this.wikiLinkLabel.UseCompatibleTextRendering = true;
            this.wikiLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.wikiLinkLabel_LinkClicked);
            // 
            // providerListView
            // 
            this.providerPanel.SetColumnSpan(this.providerListView, 2);
            resources.ApplyResources(this.providerListView, "providerListView");
            this.providerListView.GridLines = true;
            this.providerListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.providerListView.HideSelection = false;
            this.providerListView.MultiSelect = false;
            this.providerListView.Name = "providerListView";
            this.providerListView.SelectedIndexChanged += new System.EventHandler(this.providerListView_SelectedIndexChanged);
            // 
            // cloudForgeControl1
            // 
            this.cloudForgeControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.cloudForgeControl1, "cloudForgeControl1");
            this.cloudForgeControl1.Name = "cloudForgeControl1";
            // 
            // nameColumn
            // 
            resources.ApplyResources(this.nameColumn, "nameColumn");
            // 
            // wikiLinkLabel1
            // 
            resources.ApplyResources(this.wikiLinkLabel1, "wikiLinkLabel1");
            this.providerPanel.SetColumnSpan(this.wikiLinkLabel1, 2);
            this.wikiLinkLabel1.Name = "wikiLinkLabel1";
            this.wikiLinkLabel1.TabStop = true;
            this.wikiLinkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.wikiLinkLabel_LinkClicked);
            // 
            // RepositoryProviderPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.providerPanel);
            this.Controls.Add(this.cloudForgeControl1);
            this.Name = "RepositoryProviderPage";
            this.providerPanel.ResumeLayout(false);
            this.providerPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel providerPanel;
        private Ankh.UI.Controls.CloudForgeControl cloudForgeControl1;
        private System.Windows.Forms.RadioButton urlRadioButton;
        private System.Windows.Forms.ComboBox urlComboBox;
        private System.Windows.Forms.RadioButton providerRadioButton;
        private System.Windows.Forms.LinkLabel wikiLinkLabel;
        private Ankh.UI.VSSelectionControls.SmartListView providerListView;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.LinkLabel wikiLinkLabel1;
    }
}
