using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// Property editor for the predefined ignore property.
	/// </summary>
	public class IgnorePropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
	{
		public event EventHandler Changed;

		public IgnorePropertyEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

		}

		public void Clear()
		{
			this.ignoreTextBox.Text = "";

		}

		public bool Valid
		{
			get
			{ 
				return ( this.ignoreTextBox.Text.Trim() != "");
			}
		}

		public PropertyItem PropertyItem
		{
			get
			{
				if ( !this.Valid )
				{
					throw new InvalidOperationException(
						"Can not get a property item when Valid is false");
				}
				
				return new TextPropertyItem(this.ignoreTextBox.Text);
			}
			set
			{
				TextPropertyItem item = (TextPropertyItem) value;
                this.ignoreTextBox.Text = item.Text;
			}
		}

        public override string ToString()
        {
            return "svn:ignore";
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
            this.ignoreTextBox = new System.Windows.Forms.TextBox();
            this.ignoreLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ignoreTextBox
            // 
            this.ignoreTextBox.AcceptsReturn = true;
            this.ignoreTextBox.AcceptsTab = true;
            this.ignoreTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ignoreTextBox.Location = new System.Drawing.Point(0, 30);
            this.ignoreTextBox.Multiline = true;
            this.ignoreTextBox.Name = "ignoreTextBox";
            this.ignoreTextBox.Size = new System.Drawing.Size(250, 120);
            this.ignoreTextBox.TabIndex = 2;
            this.ignoreTextBox.Text = "";
            this.ignoreTextBox.TextChanged += new System.EventHandler(this.ignoreTextBox_TextChanged);
            // 
            // ignoreLabel
            // 
            this.ignoreLabel.Name = "ignoreLabel";
            this.ignoreLabel.Size = new System.Drawing.Size(256, 32);
            this.ignoreLabel.TabIndex = 3;
            this.ignoreLabel.Text = "Ignore the following files or patterns:";
            // 
            // IgnorePropertyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.ignoreLabel,
                                                                          this.ignoreTextBox});
            this.Name = "IgnorePropertyEditor";
            this.Size = new System.Drawing.Size(250, 150);
            this.ResumeLayout(false);

        }
		#endregion

	    private System.Windows.Forms.TextBox ignoreTextBox;
        private System.Windows.Forms.Label ignoreLabel;
	
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        private void ignoreTextBox_TextChanged(object sender, System.EventArgs e)
        {
            if (Changed != null)
			    Changed( this, EventArgs.Empty );

        }

		


	}
}
