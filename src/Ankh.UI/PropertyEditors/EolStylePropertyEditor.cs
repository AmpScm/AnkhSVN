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

            components = new System.ComponentModel.Container();
            CreateMyToolTip();

            existingValue = string.Empty;
            _dirty = true;
            RadioButton rb = ToRadioButton("native");
            if (rb != null)
            {
                rb.Checked = true;
                _selectedValue = (string)rb.Tag;
            }
        }

        public void Reset()
        {
        }

        public bool Valid
        {
            get 
            {
                if (!_dirty)
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
                if( !Valid)
                {
                    throw new InvalidOperationException(
                        "Can not get a property item when Valid is false");
                }
          
                return new TextPropertyItem(_selectedValue);
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
            _selectedValue = newValue;

            // Enables save button
            Dirty = !string.IsNullOrEmpty(newValue)
                && !newValue.Equals(existingValue);
        }

        private void CreateMyToolTip()
        {
            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip( nativeRadioButton, "Default. Line endings dependant on operating system");
            conflictToolTip.SetToolTip( lfRadioButton, "End of line style is LF (Line Feed)");
            conflictToolTip.SetToolTip( crRadioButton, "End of line style is CR");
            conflictToolTip.SetToolTip( crlfRdioButton, "End of line style is CRLF");
        }

        private bool Dirty
        {
            set
            {
                if (_dirty != value)
                {
                    _dirty = value;
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
            foreach (Control c in eolStyleGroupBox.Controls)
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

        string _selectedValue;
        /// <summary>
        /// Flag for enabling/disabling save button
        /// </summary>
        bool _dirty;
       
    }
}

