using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// Summary description for Loader.
	/// </summary>
	public class Loader : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Label LoaderText;
        private System.Windows.Forms.PictureBox animatedGif;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        /// <summary>
        /// Loader Form
        /// </summary>
        /// <param name="inText">Text to be printed in the form.</param>
		public Loader(string inText)
		{
			//
			// Required for Windows Form Designer support
			//
            this.outText = inText;
			InitializeComponent();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Loader));
            this.LoaderText = new System.Windows.Forms.Label();
            this.animatedGif = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // LoaderText
            // 
            this.LoaderText.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.LoaderText.Location = new System.Drawing.Point(0, 21);
            this.LoaderText.Name = "LoaderText";
            this.LoaderText.Text = this.outText;
            this.LoaderText.Size = new System.Drawing.Size(176, 24);
            this.LoaderText.TabIndex = 0;
            // 
            // animatedGif
            // 
            this.animatedGif.Image = ((System.Drawing.Bitmap)(resources.GetObject("animatedGif.Image")));
            this.animatedGif.Location = new System.Drawing.Point(184, 8);
            this.animatedGif.Name = "animatedGif";
            this.animatedGif.Size = new System.Drawing.Size(40, 40);
            this.animatedGif.TabIndex = 1;
            this.animatedGif.TabStop = false;
            this.animatedGif.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // Loader
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(226, 56);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.animatedGif,
                                                                          this.LoaderText});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Loader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ankh";
            this.ResumeLayout(false);

        }
		#endregion

        public static void Main()
        {
            Loader loader = new Loader(" Ankh er kult! ");
            loader.ShowDialog();
        }

        private string outText = null;

        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
        
        }
	}
}
