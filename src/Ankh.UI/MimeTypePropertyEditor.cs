using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// Editor for the mime-type properties.
	/// </summary>
	public class MimeTypePropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
	{
		public event EventHandler Changed;

		public MimeTypePropertyEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

        /// <summary>
        /// Resets the textbox.
        /// </summary>
		public void Clear()
		{
			this.mimeTextBox.Text = "";
		}

        /// <summary>
        /// Indicates whether the property is valid.
        /// </summary>
		public bool Valid
		{
			get
			{
				return this.mimeTextBox.Text != ""; 
			} 
		}

        /// <summary>
        /// Sets and gets the property item.
        /// </summary>
		public PropertyItem PropertyItem
		{
			get
			{
				if ( !this.Valid )
				{
					throw new InvalidOperationException(
						"Can not get a property item when Valid is false");
				}	
					return new TextPropertyItem("text/*");
			}

			set
			{
				TextPropertyItem item = (TextPropertyItem)value;
                this.mimeTextBox.Text = item.Text;
			}
		}

        /// <summary>
        /// Indicates the type of property.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "svn:mime-type";
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.mimeLabel = new System.Windows.Forms.Label();
            this.mimeTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mimeLabel
            // 
            this.mimeLabel.Name = "mimeLabel";
            this.mimeLabel.Size = new System.Drawing.Size(152, 16);
            this.mimeLabel.TabIndex = 1;
            this.mimeLabel.Text = "Enter mime-type property:";
            // 
            // mimeTextBox
            // 
            this.mimeTextBox.Location = new System.Drawing.Point(0, 21);
            this.mimeTextBox.Name = "mimeTextBox";
            this.mimeTextBox.Size = new System.Drawing.Size(152, 20);
            this.mimeTextBox.TabIndex = 2;
            this.mimeTextBox.Text = "";
            this.mimeTextBox.TextChanged += new System.EventHandler(this.mimeTextBox_TextChanged);
            // 
            // MimeTypePropertyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.mimeTextBox,
                                                                          this.mimeLabel});
            this.Name = "MimeTypePropertyEditor";
            this.ResumeLayout(false);

        }
		#endregion
        /// <summary>
        /// Dispatches the Changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mimeTextBox_TextChanged(object sender, System.EventArgs e)
        {
            if ( Changed != null )
                Changed(this, EventArgs.Empty);
        }

        private System.Windows.Forms.Label mimeLabel;
        private System.Windows.Forms.TextBox mimeTextBox;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


       

        
	}
}
