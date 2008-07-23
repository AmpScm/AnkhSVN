// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.UI
{
    /// <summary>
    /// Property editor for the predefined ignore property.
    /// </summary>
    public partial class IgnorePropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
    {
        public event EventHandler Changed;

        public IgnorePropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();
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
            return SvnPropertyNames.SvnIgnore;
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

        private void ignoreTextBox_TextChanged(object sender, System.EventArgs e)
        {
            // Enables save button
            this.dirty = true;
            if (Changed != null)
                Changed( this, EventArgs.Empty );
        }

        private void CreateMyToolTip()
        {         
            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip( this.ignoreTextBox, 
                "Eks *.obj, subdir. Names of file-categories and directories to be ignored.");
        }

        /// <summary>
        /// Flag for enabling/disabling save button
        /// </summary>
        private bool dirty;        
    }
}

