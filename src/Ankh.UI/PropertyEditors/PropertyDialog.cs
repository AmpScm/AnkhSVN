using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ankh.UI.PropertyEditors
{
    public partial class PropertyDialog : Form
    {
        private IPropertyEditor currentEditor;
        private PropertyItem existingItem;
        private Dictionary<string, IPropertyEditor> keyPropEditor = new Dictionary<string, IPropertyEditor>();

        public PropertyDialog() : this(null)
        {
        }

        public PropertyDialog(PropertyItem editItem)
        {
            InitializeComponent();
            // Dialog is in Edit mode if the "existingItem" is not null.
            this.existingItem = editItem;
            InitializeEditors();
        }

        private void InitializeEditors()
        {
            //TODO this list needs to be filtered based on the context
            // some properties can only be set for directories and some for only files
            IPropertyEditor propEditor = new ExecutablePropertyEditor();
            this.keyPropEditor.Add(propEditor.ToString(), propEditor);

            propEditor = new MimeTypePropertyEditor();
            this.keyPropEditor.Add(propEditor.ToString(), propEditor);

            propEditor = new IgnorePropertyEditor();
            this.keyPropEditor.Add(propEditor.ToString(), propEditor);
            
            propEditor = new KeywordsPropertyEditor();
            this.keyPropEditor.Add(propEditor.ToString(), propEditor);
            
            propEditor = new EolStylePropertyEditor();
            this.keyPropEditor.Add(propEditor.ToString(), propEditor);
            
            propEditor = new ExternalsPropertyEditor();
            this.keyPropEditor.Add(propEditor.ToString(), propEditor);

            this.nameComboBox.Items.Clear();
            foreach (IPropertyEditor editor in this.keyPropEditor.Values)
            {
                this.nameComboBox.Items.Add(editor);
            }

            PopulateData();
        }

        /// <remarks>
        /// Populates the field values.Uses the exsting <code>PropertItem</code> in edit mode.
        /// </remarks>
        private void PopulateData()
        {
            if (this.existingItem != null)
            {
                this.nameComboBox.Text = this.existingItem.Name;
                this.currentEditor.PropertyItem = this.existingItem;
            }
            else
            {
                this.nameComboBox.SelectedIndex = 0;
            }
        }

        private void nameComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            IPropertyEditor selectedItem = (IPropertyEditor)this.nameComboBox.SelectedItem;

            // is the selection a special svn: keyword?
            if (selectedItem != null)
            {
                SetNewEditor(selectedItem);
            }   
        }

        /// <remarks>
        /// If the typed name is a built-in svn property, sets combo selection to the editor.
        /// If the typed name is a custom property, create a new <code>PlainPropertyEditor</code>
        /// </remarks>
        private void nameComboBox_TextChanged(object sender, EventArgs e)
        {
            string typedName = this.nameComboBox.Text;
            if (this.keyPropEditor.ContainsKey(typedName))
            {
                IPropertyEditor propEditor = this.keyPropEditor[typedName];
                this.nameComboBox.SelectedItem = propEditor;
            }
            else
            {
                if (this.currentEditor == null
                     || !(this.currentEditor is PlainPropertyEditor))
                {
                    this.SetNewEditor(new PlainPropertyEditor());
                }
            }
        }

        private void SetNewEditor(IPropertyEditor editor)
        {
            if (this.currentEditor != null)
            {
                //Unsubscribe the current editor from the Changed event.
                this.currentEditor.Changed -= new EventHandler(
                    this.currentEditor_Changed);
            }

            //Clear the editor panel and add the new editor.
            this.editorPanel.Controls.Clear();
            this.editorPanel.Controls.Add((Control)editor);
            ((Control)editor).Dock = DockStyle.Fill;

            //Sets the current editor to match the selected item.
            this.currentEditor = editor;
            this.currentEditor.Changed += new EventHandler(
                this.currentEditor_Changed);
        }

        /// <summary>
        /// Update the buttons when the editor value is changed.
        /// </summary>
        private void currentEditor_Changed(object sender, System.EventArgs e)
        {
            this.UpdateButtons();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            //do nothing
        }

        /// <summary>
        /// Gets the new/edited <code>PropertyItem</code>
        /// </summary>
        public PropertyItem GetPropertyItem()
        {
            PropertyItem result = this.currentEditor == null ? null : this.currentEditor.PropertyItem;
            if (result != null)
            {
                string propertyName = this.nameComboBox.Text;
                if (string.IsNullOrEmpty(propertyName))
                {
                    result = null;
                }
                else
                {
                    result.Name = propertyName;
                }
            }
            return result;
        }

        private void UpdateButtons()
        {
            this.okButton.Enabled = !string.IsNullOrEmpty(this.nameComboBox.Text)
                && this.currentEditor != null
                && this.currentEditor.Valid;
        }

    }
}
