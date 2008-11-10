// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.UI.PropertyEditors
{
    /// <summary>
    /// Property editor for the predefined ignore property.
    /// </summary>
    partial class IgnorePropertyEditor : PropertyEditControl, IPropertyEditor
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
            this.ignoreTextBox.Text = this.originalValue;
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
                    string value = this.ignoreTextBox.Text.Trim();
                    return (!string.IsNullOrEmpty(value));
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
                this.originalValue = item.Text;
                this.ignoreTextBox.Text = this.originalValue;
            }
        }

        /// <summary>
        /// Directory property
        /// </summary>
        public SvnNodeKind GetAllowedNodeKind()
        {
            return SvnNodeKind.Directory;
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
            string newValue = this.ignoreTextBox.Text;
            // Enables/Disables save button
            this.dirty = !newValue.Equals(this.originalValue);
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

        private string originalValue = string.Empty;
    }
}

