// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.UI.PropertyEditors
{
    public partial class PropertyDialog : VSDialogForm
    {
        private PropertyEditControl currentEditor;
        private SvnPropertyValue existingItem;
        private SvnNodeKind _currentNodeKind;
        private Dictionary<string, PropertyEditControl> keyPropEditor = new Dictionary<string, PropertyEditControl>();

        public PropertyDialog(SvnNodeKind currentNodeKind)
            : this(null, currentNodeKind)
        {
        }

        public PropertyDialog(SvnPropertyValue editItem, SvnNodeKind currentNodeKind)
        {
            InitializeComponent();
            // Dialog is in Edit mode if the "existingItem" is not null.
            this.existingItem = editItem;
            _currentNodeKind = currentNodeKind;
            InitializeEditors();
        }

        private void InitializeEditors()
        {
            PropertyEditControl propEditor = new ExecutablePropertyEditor();
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
            foreach (PropertyEditControl editor in this.keyPropEditor.Values)
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
        private void AddPropertyEditor(PropertyEditControl propEditor)
        {
            SvnNodeKind allowed = propEditor.GetAllowedNodeKind();
            SvnNodeKind result =  allowed & _currentNodeKind;
            bool doAdd = ((result == SvnNodeKind.None)
                            && (allowed != SvnNodeKind.None)) ?
                            false
                            : result == _currentNodeKind;
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
        /// Populates the field values.Uses the existing <code>PropertItem</code> in edit mode.
        /// </remarks>
        private void PopulateData()
        {
            if (this.existingItem != null)
            {
                this.nameComboBox.Text = this.existingItem.Key;
                this.currentEditor.PropertyItem = this.existingItem;
                if (this.recursiveCheckBox.Enabled)
                {
                    this.recursiveCheckBox.Checked = false;// this.existingItem.Recursive;
                }
            }
            else
            {
                if (this.nameComboBox.Items.Count > 0)
                {
                    this.nameComboBox.SelectedIndex = 0;
                }
            }
        }

        private void nameComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            PropertyEditControl selectedItem = (PropertyEditControl)this.nameComboBox.SelectedItem;

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
                PropertyEditControl propEditor = this.keyPropEditor[typedName];
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

            if (this.currentEditor != null)
                this.currentEditor.PropertyName = this.nameComboBox.Text;
        }

        private void SetNewEditor(PropertyEditControl editor)
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
        public SvnPropertyValue GetPropertyItem()
        {
            SvnPropertyValue result = this.currentEditor == null ? null : this.currentEditor.PropertyItem;
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
                    if (result.Key != propertyName)
                        result = new SvnPropertyValue(propertyName, (byte[])result.RawValue);
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
