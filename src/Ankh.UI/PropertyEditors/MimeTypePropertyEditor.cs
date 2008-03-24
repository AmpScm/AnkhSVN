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
    public partial class MimeTypePropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
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

        
        private static readonly Regex validateMimeType = 
            new Regex(@"\w{2,}/\*{1}|(\w{2,})", RegexOptions.Compiled);
        /// <summary>
        /// Flag for enabled/disabled save button
        /// </summary>
        private bool dirty; 
    }
}

