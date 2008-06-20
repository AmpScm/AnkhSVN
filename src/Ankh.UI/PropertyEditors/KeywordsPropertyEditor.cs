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
    public partial class KeywordsPropertyEditor : System.Windows.Forms.UserControl, IPropertyEditor
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

                if (!this.Valid)
                {
                    throw new InvalidOperationException(
                        "Can not get a property item when Valid is false");
                }
                if (this.dateCheckBox.Checked)
                {
                    selectedText = "Date ";
                }

                if (this.revisionCheckBox.Checked)
                {
                    selectedText += "Revision ";
                }

                if (this.authorCheckBox.Checked)
                {
                    selectedText += "Author ";
                }

                if (this.urlCheckBox.Checked)
                {
                    selectedText += "URL ";
                }

                if (this.allCheckBox.Checked)
                {
                    selectedText += "Id ";
                }

                return new TextPropertyItem(selectedText);
            }

            set
            {
                TextPropertyItem item = (TextPropertyItem)value;
                this.authorCheckBox.Checked = (item.Text).IndexOf("Author") != -1;
                this.dateCheckBox.Checked = (item.Text).IndexOf("Date") != -1;
                this.revisionCheckBox.Checked = (item.Text).IndexOf("Revision") != -1;
                this.urlCheckBox.Checked = (item.Text).IndexOf("URL") != -1;
                this.allCheckBox.Checked = (item.Text).IndexOf("Id") != -1;
                this.dirty = false;
            }
        }

        /// <summary>
        /// The type of property for this editor.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return PropertyEditorConstants.SVN_PROP_KEYWORDS;
        }


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


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
                Changed(this, EventArgs.Empty);
        }

        private void CreateMyToolTip()
        {
            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip(this.dateCheckBox,
                "Keyword substitution of $LastChangedDate$ in the text ($LastChangedDate: 2002-07-22 20:16:37 -0700 (Mon, 22 Jul 2002) $");
            conflictToolTip.SetToolTip(this.revisionCheckBox,
                "Keyword substitution of $LastChangedRevision$ in the text ($LastChangedRevision: 144 $)");
            conflictToolTip.SetToolTip(this.authorCheckBox,
                "Keyword substitution of $LastChangedBy$ in the text ($LastChangedBy: Caren $))");
            conflictToolTip.SetToolTip(this.urlCheckBox,
                "Keyword substitution of $HeadURL$$ in the text ($HeadURL: http://svn.collab.net/repos/trunk/README $)");
            conflictToolTip.SetToolTip(this.allCheckBox,
                "Keyword substitution of $Id$ in the text ($Id$)");
        }

        /// <summary>
        /// Flag for enabling/disabling save button
        /// </summary>
        private bool dirty;
    }
}

