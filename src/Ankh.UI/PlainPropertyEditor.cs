// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// Property editor for plain properties
	/// </summary>
	internal class PlainPropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
	{
       
        public event EventHandler Changed;
        
		public PlainPropertyEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

		}

        public void Reset()
        {
            this.valueTextBox.Text = "";
        }

        public bool Valid
        {
            get{ return this.valueTextBox.Text.Trim() != "";}
        }

        public PropertyItem PropertyItem
        {
            get
            {   
                if( !this.Valid)
                {
                    throw new InvalidOperationException(
                        "Can not get a property item when Valid is false");
                }

                return new TextPropertyItem(this.valueTextBox.Text);
            }

            set
            {
                TextPropertyItem item = (TextPropertyItem)value;
                this.valueTextBox.Text = item.Text;
            }
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
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.plainLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // valueTextBox
            // 
            this.valueTextBox.AcceptsReturn = true;
            this.valueTextBox.AcceptsTab = true;
            this.valueTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.valueTextBox.Location = new System.Drawing.Point(0, 22);
            this.valueTextBox.Multiline = true;
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.Size = new System.Drawing.Size(240, 128);
            this.valueTextBox.TabIndex = 0;
            this.valueTextBox.Text = "";
            this.valueTextBox.TextChanged += new System.EventHandler(this.valueTextBox_TextChanged);
            // 
            // plainLabel
            // 
            this.plainLabel.Name = "plainLabel";
            this.plainLabel.Size = new System.Drawing.Size(264, 23);
            this.plainLabel.TabIndex = 1;
            this.plainLabel.Text = "Value:";
            // 
            // PlainPropertyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.plainLabel,
                                                                          this.valueTextBox});
            this.Name = "PlainPropertyEditor";
            this.Size = new System.Drawing.Size(240, 150);
            this.ResumeLayout(false);

        }
        #endregion 
        
        private System.Windows.Forms.TextBox valueTextBox;
        private System.Windows.Forms.Label plainLabel;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private void valueTextBox_TextChanged(object sender, System.EventArgs e)
        {
            if (Changed != null)
                Changed( this, EventArgs.Empty );
        }

       
	}
}

