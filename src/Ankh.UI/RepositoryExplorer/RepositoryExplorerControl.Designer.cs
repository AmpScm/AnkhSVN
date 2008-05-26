using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    public partial class RepositoryExplorerControl
    {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.backgroundListingCheck = new System.Windows.Forms.CheckBox();
            this.toolBar = new System.Windows.Forms.ToolBar();
            this.enableBackgroundListingButton = new System.Windows.Forms.ToolBarButton();
            this.addRepoURLButton = new System.Windows.Forms.ToolBarButton();
            this.toolbarImageList = new System.Windows.Forms.ImageList(this.components);
            this.treeView = new Ankh.UI.RepositoryTreeView();
            this.SuspendLayout();
            // 
            // backgroundListingCheck
            // 
            this.backgroundListingCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.backgroundListingCheck.Location = new System.Drawing.Point(80, 160);
            this.backgroundListingCheck.Name = "backgroundListingCheck";
            this.backgroundListingCheck.Size = new System.Drawing.Size(184, 16);
            this.backgroundListingCheck.TabIndex = 6;
            this.backgroundListingCheck.Text = "Enable background listing";
            // 
            // toolBar
            // 
            this.toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.enableBackgroundListingButton,
            this.addRepoURLButton});
            this.toolBar.ButtonSize = new System.Drawing.Size(16, 16);
            this.toolBar.DropDownArrows = true;
            this.toolBar.ImageList = this.toolbarImageList;
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.ShowToolTips = true;
            this.toolBar.Size = new System.Drawing.Size(296, 28);
            this.toolBar.TabIndex = 10;
            this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.ToolBarButtonClicked);
            // 
            // enableBackgroundListingButton
            // 
            this.enableBackgroundListingButton.ImageIndex = 0;
            this.enableBackgroundListingButton.Name = "enableBackgroundListingButton";
            this.enableBackgroundListingButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.enableBackgroundListingButton.ToolTipText = "Enable background listing";
            // 
            // addRepoURLButton
            // 
            this.addRepoURLButton.ImageIndex = 1;
            this.addRepoURLButton.Name = "addRepoURLButton";
            // 
            // toolbarImageList
            // 
            this.toolbarImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.toolbarImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.toolbarImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.Context = null;
            this.treeView.Location = new System.Drawing.Point(0, 28);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(296, 268);
            this.treeView.TabIndex = 9;
            this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeExpand);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeViewMouseDown);
            // 
            // RepositoryExplorerControl
            // 
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.backgroundListingCheck);
            this.Name = "RepositoryExplorerControl";
            this.Size = new System.Drawing.Size(296, 296);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
    }
}
