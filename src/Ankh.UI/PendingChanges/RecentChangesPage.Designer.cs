namespace Ankh.UI.PendingChanges
{
    partial class RecentChangesPage
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
            this.syncView = new Ankh.UI.PendingChanges.Synchronize.SynchronizeListView();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.syncView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.syncView.Location = new System.Drawing.Point(0, 0);
            this.syncView.Name = "listView1";
            this.syncView.Size = new System.Drawing.Size(768, 300);
            this.syncView.TabIndex = 0;
            this.syncView.UseCompatibleStateImageBehavior = false;
            // 
            // RecentChangesPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.syncView);
            this.Name = "RecentChangesPage";
            this.Text = "Recent Changes";
            this.ResumeLayout(false);

        }

        #endregion

        private Ankh.UI.PendingChanges.Synchronize.SynchronizeListView syncView;

    }
}
