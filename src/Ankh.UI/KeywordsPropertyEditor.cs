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
	/// Property editor for keywords.
	/// </summary>
	public class KeywordsPropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
	{
		public event EventHandler Changed;

		public KeywordsPropertyEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();
		}
        
        /// <summary>
        /// Resets the checkboxes.
        /// </summary>
		public void Reset()
		{
			this.dateCheckBox.Checked = false;
            this.authorCheckBox.Checked = false;
            this.revisionCheckBox.Checked = false;
            this.urlCheckBox.Checked = false;
            this.allCheckBox.Checked = false;
            this.dirty = false;
		}

        /// <summary>
        /// Indicates whether the selection is valid.
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
                    return this.dateCheckBox.Checked || 
                        this.revisionCheckBox.Checked ||
                        this.authorCheckBox.Checked ||
                        this.urlCheckBox.Checked ||
                        this.allCheckBox.Checked; 
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
                string selectedText = "";
				
                if( !this.Valid )
				{
					throw new InvalidOperationException(
						"Can not get a property item when Valid is false");
				}
                if ( this.dateCheckBox.Checked )
                {
                    selectedText = "Date ";
                }

                if ( this.revisionCheckBox.Checked )
                {
                    selectedText += "Revision ";
                }

                if (this.authorCheckBox.Checked )
                {
                    selectedText += "Author ";
                }

                if (this.urlCheckBox.Checked )
                {
                    selectedText += "URL ";
                }

                if (this.allCheckBox.Checked )
                {
                    selectedText += "Id ";
                }

				return new TextPropertyItem(selectedText);
			}

			set
			{
                TextPropertyItem item = (TextPropertyItem)value;
                this.authorCheckBox.Checked = (item.Text).IndexOf( "Author" ) != -1;
                this.dateCheckBox.Checked = (item.Text).IndexOf( "Date" ) != -1;
                this.revisionCheckBox.Checked = (item.Text).IndexOf( "Revision" ) != -1;
                this.urlCheckBox.Checked = (item.Text).IndexOf( "URL") != -1;
                this.allCheckBox.Checked = (item.Text).IndexOf( "Id" ) != -1;
			    this.dirty = false;
            }
		}

        /// <summary>
        /// The type of property for this editor.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "svn:keywords";
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
            this.keywordLabel = new System.Windows.Forms.Label();
            this.urlCheckBox = new System.Windows.Forms.CheckBox();
            this.revisionCheckBox = new System.Windows.Forms.CheckBox();
            this.authorCheckBox = new System.Windows.Forms.CheckBox();
            this.allCheckBox = new System.Windows.Forms.CheckBox();
            this.dateCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // keywordLabel
            // 
            this.keywordLabel.Name = "keywordLabel";
            this.keywordLabel.Size = new System.Drawing.Size(160, 16);
            this.keywordLabel.TabIndex = 1;
            this.keywordLabel.Text = "Select what to substitute:";
            // 
            // urlCheckBox
            // 
            this.urlCheckBox.Location = new System.Drawing.Point(0, 88);
            this.urlCheckBox.Name = "urlCheckBox";
            this.urlCheckBox.TabIndex = 5;
            this.urlCheckBox.Tag = "URL";
            this.urlCheckBox.Text = "URL";
            this.urlCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // revisionCheckBox
            // 
            this.revisionCheckBox.Location = new System.Drawing.Point(0, 40);
            this.revisionCheckBox.Name = "revisionCheckBox";
            this.revisionCheckBox.TabIndex = 3;
            this.revisionCheckBox.Tag = "Revision";
            this.revisionCheckBox.Text = "Revision";
            this.revisionCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // authorCheckBox
            // 
            this.authorCheckBox.Location = new System.Drawing.Point(0, 64);
            this.authorCheckBox.Name = "authorCheckBox";
            this.authorCheckBox.TabIndex = 4;
            this.authorCheckBox.Tag = "Author";
            this.authorCheckBox.Text = "Author";
            this.authorCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // allCheckBox
            // 
            this.allCheckBox.Location = new System.Drawing.Point(0, 112);
            this.allCheckBox.Name = "allCheckBox";
            this.allCheckBox.TabIndex = 6;
            this.allCheckBox.Text = "All";
            this.allCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // dateCheckBox
            // 
            this.dateCheckBox.Location = new System.Drawing.Point(0, 16);
            this.dateCheckBox.Name = "dateCheckBox";
            this.dateCheckBox.TabIndex = 7;
            this.dateCheckBox.Tag = "Date";
            this.dateCheckBox.Text = "Date";
            this.dateCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // KeywordsPropertyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.dateCheckBox,
                                                                          this.keywordLabel,
                                                                          this.urlCheckBox,
                                                                          this.revisionCheckBox,
                                                                          this.authorCheckBox,
                                                                          this.allCheckBox});
            this.Name = "KeywordsPropertyEditor";
            this.ResumeLayout(false);

        }
		#endregion
        /// <summary>
        /// Dispatches the Changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, System.EventArgs e)
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
            conflictToolTip.SetToolTip( this.dateCheckBox, 
                "Keyword substitution of $LastChangedDate$ in the text ($LastChangedDate: 2002-07-22 20:16:37 -0700 (Mon, 22 Jul 2002) $");
            conflictToolTip.SetToolTip( this.revisionCheckBox, 
                "Keyword substitution of $LastChangedRevision$ in the text ($LastChangedRevision: 144 $)");
            conflictToolTip.SetToolTip( this.authorCheckBox, 
                "Keyword substitution of $LastChangedBy$ in the text ($LastChangedBy: Caren $))");
            conflictToolTip.SetToolTip( this.urlCheckBox, 
                "Keyword substitution of $HeadURL$$ in the text ($HeadURL: http://svn.collab.net/repos/trunk/README $)");
            conflictToolTip.SetToolTip( this.allCheckBox, 
                "Keyword substitution of $Id$ in the text ($Id$)");
        }

		private System.Windows.Forms.Label keywordLabel;
        private System.Windows.Forms.CheckBox dateCheckBox;
        private System.Windows.Forms.CheckBox urlCheckBox;
        private System.Windows.Forms.CheckBox revisionCheckBox;
        private System.Windows.Forms.CheckBox allCheckBox;
        private System.Windows.Forms.CheckBox authorCheckBox;
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

