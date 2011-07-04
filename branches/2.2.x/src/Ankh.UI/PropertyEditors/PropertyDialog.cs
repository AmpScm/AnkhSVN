// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.IO;

namespace Ankh.UI.PropertyEditors
{
    public partial class PropertyDialog : VSDialogForm
    {
        private PropertyEditControl _currentEditor;
        private SvnPropertyValue _existingItem;
        private SvnNodeKind _currentNodeKind;
        private SortedList<string, PropertyEditControl> keyPropEditor = new SortedList<string, PropertyEditControl>();

        public PropertyDialog(SvnNodeKind currentNodeKind)
            : this(null, currentNodeKind)
        {
        }

        public PropertyDialog(SvnPropertyValue editItem, SvnNodeKind currentNodeKind)
        {
            InitializeComponent();
            // Dialog is in Edit mode if the "existingItem" is not null.
            this._existingItem = editItem;
            _currentNodeKind = currentNodeKind;
            InitializeEditors();
        }

        private void InitializeEditors()
        {
            AddPropertyEditor(new ExecutablePropertyEditor());
            AddPropertyEditor(new MimeTypePropertyEditor());
            AddPropertyEditor(new IgnorePropertyEditor());
            AddPropertyEditor(new KeywordsPropertyEditor());
            AddPropertyEditor(new EolStylePropertyEditor());
            AddPropertyEditor(new NeedsLockPropertyEditor());
            AddPropertyEditor(new ExternalsPropertyEditor());

            this.nameComboBox.Items.Clear();
            foreach (PropertyEditControl editor in this.keyPropEditor.Values)
            {
                this.nameComboBox.Items.Add(editor);
            }

            // Enable recursion if the current node is directory to allow applying a property recursively
            PopulateData();
        }

        /// <summary>
        /// Adds the property editor if it allows the current SvnItem's node kind
        /// </summary>
        /// <param name="propEditor">IPropertyEditor</param>
        private void AddPropertyEditor(PropertyEditControl propEditor)
        {
            if (!propEditor.AllowNodeKind(_currentNodeKind))
                return;

            // TODO: Perhaps allow applying file properties recursively on a folder

            string key = propEditor.ToString();
            this.keyPropEditor.Add(key, propEditor);
        }

        /// <remarks>
        /// Populates the field values.Uses the existing <code>PropertItem</code> in edit mode.
        /// </remarks>
        private void PopulateData()
        {
            if (this._existingItem != null)
            {
                this.nameComboBox.Text = this._existingItem.Key;
                this._currentEditor.PropertyItem = this._existingItem;
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
                if (this._currentEditor == null
                     || !(this._currentEditor is PlainPropertyEditor))
                {
                    this.SetNewEditor(new PlainPropertyEditor());
                }
            }

            if (this._currentEditor != null)
                this._currentEditor.PropertyName = this.nameComboBox.Text;
        }

        private void SetNewEditor(PropertyEditControl editor)
        {
            if (this._currentEditor != null)
            {
                //Unsubscribe the current editor from the Changed event.
                this._currentEditor.Changed -= new EventHandler(
                    this.currentEditor_Changed);
            }

            //Clear the editor panel and add the new editor.
            this.editorPanel.Controls.Clear();
            this.editorPanel.Controls.Add((Control)editor);
            ((Control)editor).Dock = DockStyle.Fill;

            //Sets the current editor to match the selected item.
            this._currentEditor = editor;
            UpdateButtons(); // allow new peoperty editor to determine the initial button states
            this._currentEditor.Changed += new EventHandler(
                this.currentEditor_Changed);
        }

        /// <summary>
        /// Update the buttons when the editor value is changed.
        /// </summary>
        private void currentEditor_Changed(object sender, System.EventArgs e)
        {
            this.UpdateButtons();
        }

        /// <summary>
        /// Gets the new/edited <code>PropertyItem</code>
        /// </summary>
        public SvnPropertyValue GetPropertyItem()
        {
            SvnPropertyValue result = this._currentEditor == null ? null : this._currentEditor.PropertyItem;
            if (result != null)
            {
                if (!_currentEditor.AllowNodeKind(_currentNodeKind))
                {
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
            return false;
        }

        private void UpdateButtons()
        {
            this.okButton.Enabled = !string.IsNullOrEmpty(this.nameComboBox.Text)
                && this._currentEditor != null
                && this._currentEditor.Valid;

            loadButton.Enabled = (_currentEditor is PlainPropertyEditor);
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            PlainPropertyEditor ppe = _currentEditor as PlainPropertyEditor;

            if (ppe == null)
                return;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*";
                dlg.CheckFileExists = true;

                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                using (Stream s = dlg.OpenFile())
                using (StreamReader sr = new StreamReader(s))
                {
                    ppe.CurrentText = sr.ReadToEnd();
                }
            }

        }
    }
}
