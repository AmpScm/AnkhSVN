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
	/// Property editor for the predefined ignore property.
	/// </summary>
	public class IgnorePropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
	{
		public event EventHandler Changed;

		public IgnorePropertyEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

             this.components = new System.ComponentModel.Container();
            CreateMyToolTip();

			// TODO: Add any initialization after the InitForm call

		}

		public void Reset()
		{
			this.ignoreTextBox.Text = "";
            this.dirty = false;
		}

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
                    return ( this.ignoreTextBox.Text.Trim() != "");
                }
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
                this.dirty = false;
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
            this.ignoreLabel.Size = new System.Drawing.Size(256, 24);
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

        private void ignoreTextBox_TextChanged(object sender, System.EventArgs e)
        {
            // Enables save button
            this.dirty = true;
            if (Changed != null)
                Changed( this, EventArgs.Empty );
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
            conflictToolTip.SetToolTip( this.ignoreTextBox, 
                "Eks *.obj, subdir. Names of file-categories and directories to be ignored.");
         }

	    private System.Windows.Forms.TextBox ignoreTextBox;
        private System.Windows.Forms.Label ignoreLabel;
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

