using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Text;

namespace UseCase
{
	/// <summary>
	/// Summary description for XmlViewForm.
	/// </summary>
	public class XmlViewForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public XmlViewForm( UseCaseModel useCaseModel )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();  
           
            this.useCaseModel = useCaseModel;

            this.useCaseModel.Changed += new EventHandler( this.Changed );
            this.RefreshView();
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
            this.useCaseModel.Changed -= new EventHandler( this.Changed );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.textBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.Size = new System.Drawing.Size(448, 371);
            this.textBox.TabIndex = 0;
            this.textBox.Text = "";
            // 
            // XmlViewForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(448, 371);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.textBox});
            this.Name = "XmlViewForm";
            this.Text = "XmlViewForm";
            this.ResumeLayout(false);

        }
		#endregion

        private void Changed( object sender, EventArgs e )
        {
            this.RefreshView();
        }

        

        private void RefreshView()
        {
            this.textBox.Clear();
            this.textBox.Text = this.useCaseModel.AsXml;
        }

        


        #region private data
        private System.Windows.Forms.RichTextBox textBox;
        private UseCaseModel useCaseModel;
        #endregion
	}
}
