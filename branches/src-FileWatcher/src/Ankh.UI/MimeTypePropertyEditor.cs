// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;

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

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();
        }

        /// <summary>
        /// Resets the textbox.
        /// </summary>
        public void Reset()
        {
            this.mimeTextBox.Text = "";
            this.dirty = false;
        }

        /// <summary>
        /// Indicates whether the property is valid.
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
                    return validateMimeType.IsMatch(this.mimeTextBox.Text);
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
                        "Can not get a property item when Valid is false");
                }	
                return new TextPropertyItem(this.mimeTextBox.Text);
            }

            set
            {
                TextPropertyItem item = (TextPropertyItem)value;
                this.mimeTextBox.Text = item.Text;
                this.dirty = false;
            }
        }

        /// <summary>
        /// Indicates the type of property.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "mime-type";
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
            // Enables save button
            this.dirty = true;
            if ( Changed != null  )
                Changed(this, EventArgs.Empty);
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
            conflictToolTip.SetToolTip( this.mimeTextBox, 
                "Defult is text/*, everything else is binary");
        }

        private System.Windows.Forms.Label mimeLabel;
        private System.Windows.Forms.TextBox mimeTextBox;
        private static readonly Regex validateMimeType = 
            new Regex(@"\w{2,}/\*{1}|(\w{2,})", RegexOptions.Compiled);
        /// <summary>
        /// Flag for enabled/disabled save button
        /// </summary>
        private bool dirty;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;


       

        
    }
}

