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
	/// Editor for externals properties.
	/// </summary>
	public class ExternalsPropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
	{
        public event EventHandler Changed;

		public ExternalsPropertyEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();
		}
        
        /// <summary>
        /// Resets the textbox.
        /// </summary>
        public void Reset()
        {
            this.externalsTextBox.Text = "";
            this.dirty = false;
         }

        /// <summary>
        /// Indicates whether the property item is valid.
        /// </summary>
        public bool Valid
        {
            get
            {
                if (!this.dirty)
                {
                    return false;
                }
                else 
                {
                    return this.externalsTextBox.Text.Trim() != ""; 
                }
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
                        "Can not get a property item when valid is false");
                }
				
                return new TextPropertyItem(this.externalsTextBox.Text);
            }

            set
            {
              TextPropertyItem item = (TextPropertyItem)value;
                this.externalsTextBox.Text = item.Text;
                this.dirty = false;
            }
        }

        /// <summary>
        /// Indicates the type of property.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "externals";
        }

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null)
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
            this.externalsTextBox = new System.Windows.Forms.TextBox();
            this.externalsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // externalsTextBox
            // 
            this.externalsTextBox.AcceptsReturn = true;
            this.externalsTextBox.AcceptsTab = true;
            this.externalsTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.externalsTextBox.Location = new System.Drawing.Point(0, 22);
            this.externalsTextBox.Multiline = true;
            this.externalsTextBox.Name = "externalsTextBox";
            this.externalsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.externalsTextBox.Size = new System.Drawing.Size(400, 128);
            this.externalsTextBox.TabIndex = 2;
            this.externalsTextBox.Text = "";
            this.externalsTextBox.TextChanged += new System.EventHandler(this.externalsTextBox_TextChanged);
            // 
            // externalsLabel
            // 
            this.externalsLabel.Location = new System.Drawing.Point(0, 1);
            this.externalsLabel.Name = "externalsLabel";
            this.externalsLabel.Size = new System.Drawing.Size(408, 16);
            this.externalsLabel.TabIndex = 3;
            this.externalsLabel.Text = "Write path and URL:";
            // 
            // ExternalsPropertyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.externalsLabel,
                                                                          this.externalsTextBox});
            this.Name = "ExternalsPropertyEditor";
            this.Size = new System.Drawing.Size(400, 150);
            this.ResumeLayout(false);

        }
		#endregion

        /// <summary>
        /// Dispatches the Changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void externalsTextBox_TextChanged(object sender, System.EventArgs e)
        {
            // Enables save button
            this.dirty = true;
            if (Changed != null)
                Changed (this, EventArgs.Empty );
        }

        private void CreateMyToolTip()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip conflictToolTip = new ToolTip(this.components);

            // Set up the delays in milliseconds for the ToolTip.
            conflictToolTip.AutoPopDelay = 5000;
            conflictToolTip.InitialDelay = 1000;
            conflictToolTip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            conflictToolTip.ShowAlways = true;
         
            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip( this.externalsTextBox, 
                "Example: subdir1/foo   http://url.for.external.source/foo. Could be used to make your own module.");
        }

        private System.Windows.Forms.TextBox externalsTextBox;
        private System.Windows.Forms.Label externalsLabel;
        /// <summary>
        /// Flag for enabling/disabling save button
        /// </summary>
        private bool dirty;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

       

	}
}

