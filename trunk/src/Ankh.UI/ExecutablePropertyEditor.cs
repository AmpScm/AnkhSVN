using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// Property editor for executable properties.
	/// </summary>
	internal class ExecutablePropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
	{
		
		public event EventHandler Changed;
	
		public ExecutablePropertyEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

		}

		public void Clear()
		{
            this.executableCheckBox.Checked = false;
		}

		public bool Valid
		{
			get{ return this.executableCheckBox.Checked; }
		}

		public PropertyItem PropertyItem
		{
			get
			{
				if ( !this.Valid )
				{
					throw new InvalidOperationException(
						"Can not get a property item when valid is false");
				}
				
				return new TextPropertyItem("File is executable");
			}

			set
			{
				TextPropertyItem item = (TextPropertyItem)value;
			}
		}

        public override string ToString()
        {
            return "svn:executable";
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
            this.executableCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // executableCheckBox
            // 
            this.executableCheckBox.Name = "executableCheckBox";
            this.executableCheckBox.Size = new System.Drawing.Size(160, 24);
            this.executableCheckBox.TabIndex = 1;
            this.executableCheckBox.Text = "Executable";
            this.executableCheckBox.Click += new System.EventHandler(this.executableCheckBox_Click);
            // 
            // ExecutablePropertyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                             this.executableCheckBox});
            this.Name = "ExecutablePropertyEditor";
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.CheckBox executableCheckBox;

		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        private void executableCheckBox_Click(object sender, System.EventArgs e)
        {
          if (Changed != null)
				Changed (this, EventArgs.Empty );
        }
	}
	

	
}
