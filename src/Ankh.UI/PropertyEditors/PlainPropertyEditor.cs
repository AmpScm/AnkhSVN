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
    internal partial class PlainPropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
    {
        public event EventHandler Changed;
        
        public PlainPropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();

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

        private void valueTextBox_TextChanged(object sender, System.EventArgs e)
        {
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
            conflictToolTip.SetToolTip( this.valueTextBox, 
                "Enter value of your self defined property" );      
        }
    }
}

