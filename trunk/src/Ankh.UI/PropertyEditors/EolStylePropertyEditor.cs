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
    /// Summary description for EolStylePropertyEditor.
    /// </summary>
    internal partial class EolStylePropertyEditor : PropertyEditControl, IPropertyEditor
    {
        public event EventHandler Changed;
        public string existingValue;

        public EolStylePropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();

            existingValue = string.Empty;
            this.dirty = true;
            RadioButton rb = ToRadioButton("native");
            if (rb != null)
            {
                rb.Checked = true;
            }
        }

        public void Reset()
        {
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
                    return true;
            }

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
          
                return new TextPropertyItem(selectedValue);
            }
            set
            {
                TextPropertyItem item = (TextPropertyItem)value;
                existingValue = item.Text;
                RadioButton rb = ToRadioButton(existingValue);
                if (rb != null)
                {
                    rb.Checked = true;
                }
            }
        }

        public override string ToString()
        {
            return SvnPropertyNames.SvnEolStyle;
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

        private void RadioButton_CheckedChanged(object sender, System.EventArgs e)
        {
            string newValue = (string)((RadioButton)sender).Tag;
            this.selectedValue = newValue;

            // Enables save button
            this.Dirty = !string.IsNullOrEmpty(newValue)
                && !newValue.Equals(existingValue);
        }

        private void CreateMyToolTip()
        {
            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip( this.nativeRadioButton, "Default. Line endings dependant on operating system");
            conflictToolTip.SetToolTip( this.lfRadioButton, "End of line style is LF (Line Feed)");
            conflictToolTip.SetToolTip( this.crRadioButton, "End of line style is CR");
            conflictToolTip.SetToolTip( this.crlfRdioButton, "End of line style is CRLF");
        }

        private bool Dirty
        {
            set
            {
                if (this.dirty != value)
                {
                    this.dirty = value;
                    if (Changed != null)
                    {
                        Changed(this, EventArgs.Empty);
                    }
                }
            }
        }

        private RadioButton ToRadioButton(string propertyValue)
        {
            if (string.IsNullOrEmpty(propertyValue))
            {
                return null;
            }
            foreach (Control c in this.eolStyleGroupBox.Controls)
            {
                if (c is RadioButton
                    && c.Tag is string
                    && propertyValue.Equals((string) c.Tag))
                {
                    return (RadioButton)c;
                }
            }
            return null;
        }

        private string selectedValue;
        /// <summary>
        /// Flag for enabling/disabling save button
        /// </summary>
        private bool dirty;
       
    }
}

