namespace Ankh.UI.Controls
{
    partial class CloudForgeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloudForgeControl));
            this.cloudForgePictureBox = new System.Windows.Forms.PictureBox();
            this.cloudForgeToolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cloudForgePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cloudForgePictureBox
            // 
            resources.ApplyResources(this.cloudForgePictureBox, "cloudForgePictureBox");
            this.cloudForgePictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cloudForgePictureBox.Name = "cloudForgePictureBox";
            this.cloudForgePictureBox.TabStop = false;
            this.cloudForgeToolTip.SetToolTip(this.cloudForgePictureBox, resources.GetString("cloudForgePictureBox.ToolTip"));
            this.cloudForgePictureBox.Click += new System.EventHandler(this.cloudForgePictureBox_Click);
            // 
            // CloudForgeControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cloudForgePictureBox);
            this.Name = "CloudForgeControl";
            ((System.ComponentModel.ISupportInitialize)(this.cloudForgePictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox cloudForgePictureBox;
        private System.Windows.Forms.ToolTip cloudForgeToolTip;
    }
}
