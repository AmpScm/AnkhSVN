namespace Ankh.UI.PendingChanges
{
	partial class PendingActivationPage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PendingActivationPage));
			this.openSccSelectorLink = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// openSccSelectorLink
			// 
			resources.ApplyResources(this.openSccSelectorLink, "openSccSelectorLink");
			this.openSccSelectorLink.Name = "openSccSelectorLink";
			this.openSccSelectorLink.TabStop = true;
			this.openSccSelectorLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.openSccSelectorLink_LinkClicked);
			// 
			// PendingActivationPage
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.openSccSelectorLink);
			this.Name = "PendingActivationPage";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.LinkLabel openSccSelectorLink;
	}
}
