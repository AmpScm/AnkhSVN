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
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Displays a Loader 
        /// </summary>
        /// <param name="name">Name of the Form</param>
        /// <param name="head">Text in the Form</param>
        public Loader(string inText)
        {
            //
            // Required for Windows Form Designer support
            //
            this.outText = inText;
            InitializeComponent();
             

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.loaderText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Bitmap)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(194, -3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(37, 49);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // loaderText
            // 
            this.loaderText.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.loaderText.Location = new System.Drawing.Point(6, 8);
            this.loaderText.Name = "loaderText";
            this.loaderText.Text = this.outText;
            this.loaderText.Size = new System.Drawing.Size(191, 36);
            // 
            // Loader
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(235, 52);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.loaderText,
                                                                          this.pictureBox1});
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Loader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ankh";
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label loaderText;
        private string outText;

        public static void Main()
        {
            Loader load = new Loader("test");
            load.ShowDialog();

        }

        
    }
   

}
