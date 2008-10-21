namespace Ankh.UI.MergeWizard
{
    partial class MergeRevisionsSelectionPageControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeRevisionsSelectionPageControl));
            this.logToolControl1 = new Ankh.UI.SvnLog.LogControl(this.components);
            this.SuspendLayout();
            // 
            // logToolControl1
            // 
            this.logToolControl1.ChangedPathsVisible = true;
            this.logToolControl1.IncludeMerged = false;
            resources.ApplyResources(this.logToolControl1, "logToolControl1");
            this.logToolControl1.LogMessageVisible = true;
            this.logToolControl1.Mode = Ankh.UI.SvnLog.LogMode.Log;
            this.logToolControl1.Name = "logToolControl1";
            this.logToolControl1.StrictNodeHistory = false;
            this.logToolControl1.BatchFinished += new System.EventHandler<Ankh.UI.SvnLog.BatchFinishedEventArgs>(this.logToolControl1_BatchFinished);
            // 
            // MergeRevisionsSelectionPageControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logToolControl1);
            this.Name = "MergeRevisionsSelectionPageControl";
            this.ResumeLayout(false);

        }

        #endregion

		private Ankh.UI.SvnLog.LogControl logToolControl1;
    }
}
