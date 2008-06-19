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
    /// Property editor for executable properties.
    /// </summary>
    internal partial class ExecutablePropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
    {
		
        public event EventHandler Changed;
	
        public ExecutablePropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();

            // TODO: Add any initialization after the InitForm call

        }

        public void Reset()
        {
            this.executableCheckBox.Checked = false;
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
                    return this.executableCheckBox.Checked; 
            }
        }

        public PropertyItem PropertyItem
        {
            get
            {
                if ( !this.Valid )
                {
                    throw new InvalidOperationException(
                        "Can not get a property item when valid is false");
                }
				
                return new TextPropertyItem("File is executable");
            }

            set
            {
                this.executableCheckBox.Checked = true;
                this.dirty = false;
            }
        }

        public override string ToString()
        {
            return PropertyEditorConstants.SVN_PROP_EXECUTABLE;
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

        private void executableCheckBox_Click(object sender, System.EventArgs e)
        {
            // Enables save button
            this.dirty = true;
            if (Changed != null)
                Changed (this, EventArgs.Empty );
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
            conflictToolTip.SetToolTip( this.executableCheckBox, "File is executable");
        }
        
        /// <summary>
        /// Flag for enabling/disabling save button
        /// </summary>
        private bool dirty;      
    }
	

	
}

