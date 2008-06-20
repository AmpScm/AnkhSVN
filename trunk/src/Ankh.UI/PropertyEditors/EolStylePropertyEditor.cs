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
    /// Summary description for EolStylePropertyEditor.
    /// </summary>
    internal partial class EolStylePropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
    {
        public event EventHandler Changed;

        public EolStylePropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();

            this.selectedValue = "native";

            // TODO: Add any initialization after the InitForm call

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
                this.dirty = false;
            }
        }

        public override string ToString()
        {
            return PropertyEditorConstants.SVN_PROP_EOLSTYLE;
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
            // Enables save button
            this.dirty = true;
            if ( Changed != null )
                Changed(this, EventArgs.Empty);
    
            selectedValue = ((RadioButton)sender).Text;
        }

        private void CreateMyToolTip()
        {
            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip( this.nativeRadioButton, "Default. Line endings dependant on operating system");
            conflictToolTip.SetToolTip( this.lfRadioButton, "End of line style is LF (Line Feed)");
            conflictToolTip.SetToolTip( this.crRadioButton, "End of line style is CR");
            conflictToolTip.SetToolTip( this.crlfRdioButton, "End of line style is CRLF");
        }
        
        private string selectedValue;
        /// <summary>
        /// Flag for enabling/disabling save button
        /// </summary>
        private bool dirty;

      

       
    }
}

