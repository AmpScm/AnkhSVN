namespace Ankh.UI.Controls
{
	partial class StatusPanelTestForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.statusContainer = new Ankh.UI.Controls.StatusContainer();
            this.statusPanel1 = new Ankh.UI.Controls.StatusPanel(this.components);
            this.statusPanel2 = new Ankh.UI.Controls.StatusPanel(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.statusPanel3 = new Ankh.UI.Controls.StatusPanel(this.components);
            this.statusPanel4 = new Ankh.UI.Controls.StatusPanel(this.components);
            this.statusContainer.SuspendLayout();
            this.statusPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusContainer
            // 
            this.statusContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.statusContainer.AutoScroll = true;
            this.statusContainer.AutoScrollMinSize = new System.Drawing.Size(621, 366);
            this.statusContainer.Controls.Add(this.statusPanel1);
            this.statusContainer.Controls.Add(this.statusPanel2);
            this.statusContainer.Controls.Add(this.statusPanel3);
            this.statusContainer.Controls.Add(this.statusPanel4);
            this.statusContainer.Location = new System.Drawing.Point(12, 24);
            this.statusContainer.Name = "statusContainer";
            this.statusContainer.PanelSpace = 9;
            this.statusContainer.Size = new System.Drawing.Size(640, 460);
            this.statusContainer.TabIndex = 0;
            // 
            // statusPanel1
            // 
            this.statusPanel1.Height = 64;
            this.statusPanel1.Name = "statusPanel1";
            this.statusPanel1.PanelMode = Ankh.UI.Controls.StatusPanelMode.Ok;
            this.statusPanel1.TabIndex = 0;
            this.statusPanel1.Title = "Log Message";
            // 
            // statusPanel2
            // 
            this.statusPanel2.Controls.Add(this.button1);
            this.statusPanel2.Height = 26;
            this.statusPanel2.Name = "statusPanel2";
            this.statusPanel2.PanelMode = Ankh.UI.Controls.StatusPanelMode.Warning;
            this.statusPanel2.TabIndex = 1;
            this.statusPanel2.Title = "Busy Debugging";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(283, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // statusPanel3
            // 
            this.statusPanel3.Height = 38;
            this.statusPanel3.Name = "statusPanel3";
            this.statusPanel3.PanelMode = Ankh.UI.Controls.StatusPanelMode.Error;
            this.statusPanel3.TabIndex = 2;
            this.statusPanel3.Title = "TortoiseSVN is breaking your WC";
            // 
            // statusPanel4
            // 
            this.statusPanel4.Height = 37;
            this.statusPanel4.Name = "statusPanel4";
            this.statusPanel4.PanelMode = Ankh.UI.Controls.StatusPanelMode.Suggestion;
            this.statusPanel4.TabIndex = 3;
            this.statusPanel4.Title = "You should upgrade to a Daily Build";
            // 
            // StatusPanelTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(664, 510);
            this.Controls.Add(this.statusContainer);
            this.Name = "StatusPanelTestForm";
            this.Text = "Form1";
            this.statusContainer.ResumeLayout(false);
            this.statusPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private StatusContainer statusContainer;
		private StatusPanel statusPanel1;
		private StatusPanel statusPanel2;
		private StatusPanel statusPanel3;
		private StatusPanel statusPanel4;
		private System.Windows.Forms.Button button1;
	}
}