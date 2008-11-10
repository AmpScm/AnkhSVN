using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.UI.PropertyEditors
{
    public partial class PropertyDialog : Form
    {
        private IPropertyEditor currentEditor;
        private PropertyItem existingItem;
        private SvnNodeKind _currentNodeKind;
        private Dictionary<string, IPropertyEditor> keyPropEditor = new Dictionary<string, IPropertyEditor>();

        public PropertyDialog(SvnNodeKind currentNodeKind)
            : this(null, currentNodeKind)
        {
        }

        public PropertyDialog(PropertyItem editItem, SvnNodeKind currentNodeKind)
        {
            InitializeComponent();
            // Dialog is in Edit mode if the "existingItem" is not null.
            this.existingItem = editItem;
            _currentNodeKind = currentNodeKind;
            InitializeEditors();
        }

        private void InitializeEditors()
        {
            IPropertyEditor propEditor = new ExecutablePropertyEditor();
            AddPropertyEditor(propEditor);

            propEditor = new MimeTypePropertyEditor();
            AddPropertyEditor(propEditor);

            propEditor = new IgnorePropertyEditor();
            AddPropertyEditor(propEditor);
            
            propEditor = new KeywordsPropertyEditor();
            AddPropertyEditor(propEditor);
            
            propEditor = new EolStylePropertyEditor();
            AddPropertyEditor(propEditor);

            propEditor = new NeedsLockPropertyEditor();
            AddPropertyEditor(propEditor);

            propEditor = new ExternalsPropertyEditor();
            AddPropertyEditor(propEditor);

            this.nameComboBox.Items.Clear();
            foreach (IPropertyEditor editor in this.keyPropEditor.Values)
            {
                this.nameComboBox.Items.Add(editor);
            }

            // Enable recursion if the current node is directory to allow applying a property recursively
            this.recursiveCheckBox.Enabled = _currentNodeKind == SvnNodeKind.Directory;
            PopulateData();
        }

        /// <summary>
        /// Adds the property editor if it allows the current SvnItem's node kind
        /// </summary>
        /// <param name="propEditor">IPropertyEditor</param>
        private void AddPropertyEditor(IPropertyEditor propEditor)
        {
            SvnNodeKind allowed = propEditor.GetAllowedNodeKind();
            SvnNodeKind result =  allowed & _currentNodeKind;
            bool doAdd = (result == _currentNodeKind);
            if (!doAdd)
            {
                // Enable file properties on directories
                // to support applying properties recursively
                doAdd = (allowed | SvnNodeKind.File) == SvnNodeKind.File 
                    && _currentNodeKind == SvnNodeKind.Directory;
            }
            if (doAdd)
            {
                string key = propEditor.ToString();
                this.keyPropEditor.Add(key, propEditor);
            }
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
                if (this.recursiveCheckBox.Enabled)
                {
                    this.recursiveCheckBox.Checked = this.existingItem.Recursive;
                }
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
            UpdateButtons(); // allow new peoperty editor to determine the initial button states
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
                if (_currentNodeKind == SvnNodeKind.Directory
                    && (this.currentEditor.GetAllowedNodeKind() | SvnNodeKind.File) == SvnNodeKind.File // only files
                    && !ApplyRecursively()
                    )
                {
                    // Do not accept a file property on a directory
                    // unless "Apply recursively" is checked.
                    MessageBox.Show(
                        "Can not set a file property on a directory. ",
                        "Invalid Directory Property", MessageBoxButtons.OK,
                        MessageBoxIcon.Hand);
                    return null;
                }
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

        /// <summary>
        /// Gets if the property is going to be applied recursively.
        /// </summary>
        /// <returns></returns>
        public bool ApplyRecursively()
        {
            return this.recursiveCheckBox.Checked;
        }

        private void UpdateButtons()
        {
            this.okButton.Enabled = !string.IsNullOrEmpty(this.nameComboBox.Text)
                && this.currentEditor != null
                && this.currentEditor.Valid;
        }

    }
}
