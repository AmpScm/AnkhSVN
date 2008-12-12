// $Id$
//
// Copyright 2003-2008 The AnkhSVN Project
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

// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using SharpSvn;
using Ankh.Scc;

namespace Ankh.UI.PropertyEditors
{
    /// <summary>
    /// Dialog for editing svn properties. 
    /// </summary>
    public partial class PropertyEditorDialog : System.Windows.Forms.Form
    {
        IAnkhServiceProvider _context;
        readonly List<PropertyItem> _propItems;
        IPropertyEditor _currentEditor;
        SvnNodeKind _currentNodeKind;

        public PropertyEditorDialog(string svnItemPath)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            _propItems = new List<PropertyItem>();

            this.svnItemLabel.Text = svnItemPath == null ? "" : svnItemPath;
        }

        public PropertyEditorDialog(SvnItem svnItem) : this(svnItem.FullPath)
        {
            _currentNodeKind = svnItem.NodeKind;
        }

        public PropertyEditorDialog(SvnUriTarget target)
            : this(target.ToString())
        {
            _currentNodeKind = SvnNodeKind.None;
        }

        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; }
        }

        /// <summary>
        /// Sets and gets property items.
        /// </summary>
        public PropertyItem[] PropertyItems
        {
            get
            {
                return (PropertyItem[])
                    _propItems.ToArray();
            }
            set
            {
                _propItems.Clear();
                _propItems.AddRange(value);
                this.PopulateListView();
            }
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
        /// List the property items defined.
        /// </summary>
        private void PopulateListView()
        {
            this.propListView.Items.Clear();
            ListVisitor visitor = new ListVisitor(this.propListView);
            foreach (PropertyItem item in this._propItems)
                item.AcceptVisitor(visitor);
        }
        #region class ListVisitor
        /// <summary>
        /// Visitor that populates the list view.
        /// </summary>
        private class ListVisitor : IPropertyItemVisitor
        {
            public ListVisitor(ListView list)
            {
                this.list = list;
            }

            public void VisitTextPropertyItem(TextPropertyItem item)
            {
                // HACK: find out if all this .Replace is really what we want/need
                AddItem(new string[]{ item.Name, 
                                         item.Text.Replace("\t", "    ").Replace( "\r\n", "[NL]") }, item);
            }

            public void VisitBinaryPropertyItem(BinaryPropertyItem item)
            {
                this.AddItem(new string[] { item.Name, "<binary value>" }, item);

            }

            private void AddItem(string[] items, PropertyItem item)
            {
                ListViewItem listItem = new ListViewItem(items);
                listItem.Tag = item;
                this.list.Items.Add(listItem);
            }

            private ListView list;
        }
        #endregion

        /// <summary>
        /// Brings up the Property Dialog in edit mode.
        /// </summary>
        private void editButton_Click(object sender, System.EventArgs e)
        {
            PropertyItem item = (PropertyItem)this.propListView.SelectedItems[0].Tag;
            int index = this._propItems.IndexOf(item);

            IUIService ui = null;
            if (Context != null)
                ui = Context.GetService<IUIService>();

            using (PropertyDialog pDialog = new PropertyDialog(item, _currentNodeKind))
            {

                DialogResult dr;

                if (ui != null)
                    dr = ui.ShowDialog(pDialog);
                else
                    dr = pDialog.ShowDialog(this);

                if (dr != DialogResult.OK)
                    return;

                PropertyItem editedItem = pDialog.GetPropertyItem();
                if (editedItem != null)
                {
                    int otherIndex = -1; ;
                    if (!item.Name.Equals(editedItem.Name)
                        && ((otherIndex = TryFindItem(editedItem.Name)) > -1)
                        && otherIndex != index)
                    {
                        // there is already a property with the same name
                        // TODO
                        // Delete selected item AND replace the existing item ???
                    }
                    else
                    {
                        this._propItems[index] = editedItem;
                        this.PopulateListView();
                        this.UpdateButtons();
                    }
                }
            }
        }

        /// <summary>
        /// Delete the selected item if delete-button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, System.EventArgs e)
        {
            PropertyItem item = (PropertyItem)this.propListView.SelectedItems[0].Tag;
            this._propItems.Remove(item);
            this.PopulateListView();
            this.UpdateButtons();
        }

        /// <summary>
        /// Bring up Property Dialog in Add mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, System.EventArgs e)
        {
            using (PropertyDialog propDialog = new PropertyDialog(_currentNodeKind))
            {
                DialogResult dr;

                IUIService ui = Context != null ? Context.GetService<IUIService>() : null;

                if (ui != null)
                    dr = ui.ShowDialog(propDialog);
                else
                    dr = propDialog.ShowDialog(this);

                if (dr != DialogResult.OK)
                    return;

                PropertyItem item = propDialog.GetPropertyItem();
                if (item != null)
                {
                    int index = this.TryFindItem(item.Name);
                    item.Recursive = propDialog.ApplyRecursively();
                    if (index > -1)
                    {
                        // There is already a property with the same name.
                        // Replace the existing item with the new one
                        this._propItems[index] = item;
                    }
                    else
                    {
                        this._propItems.Add(item);
                    }
                    this.PopulateListView();
                    this.UpdateButtons();
                }
            }
        }

        private int TryFindItem(string key)
        {
            int i = 0;
            foreach (PropertyItem item in this._propItems)
            {
                if (key.Equals(item.Name))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
        /// <summary>
        /// Checks whether a predefined property is selected.
        /// If selected sets the editor to the selected item.
        /// Validate the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propListView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.UpdateButtons();
        }

        /// <summary>
        /// Validate the form if the type of editor is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void currentEditor_Changed(object sender, System.EventArgs e)
        {
            this.ValidateForm();
        }

        /// <summary>
        /// Sets a new property editor.
        /// </summary>
        /// <param name="editor"></param>
        private void SetNewEditor(IPropertyEditor editor)
        {
            if (this._currentEditor != null)
            {
                //Unsubscribe the current editor from the Changed event.
                this._currentEditor.Changed -= new EventHandler(
                    this.currentEditor_Changed);
            }

            //Sets the current editor to match the selected item.
            this._currentEditor = editor;
            this._currentEditor.Changed += new EventHandler(
                this.currentEditor_Changed);

            this.ValidateForm();
        }

        /// <summary>
        /// Validate the form.
        /// </summary>
        private void ValidateForm()
        {
        }

        private void UpdateButtons()
        {
            PropertyItem selection = null;
            if (this.propListView.SelectedItems.Count > 0)
            {
                selection = (PropertyItem)this.propListView.SelectedItems[0].Tag;
            }
            this.deleteButton.Enabled = selection != null;
            this.editButton.Enabled = selection != null;
        }
    }

    /// <summary>
    /// Represents a property item.
    /// </summary>
    public abstract class PropertyItem
    {
        protected PropertyItem()
        {
            this.name = "";
        }


        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public abstract void AcceptVisitor(IPropertyItemVisitor visitor);

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public bool Recursive
        {
            get { return this.recursive; }
            set { this.recursive = value; }
        }

        private string name;
        private bool recursive;
    }
    /// <summary>
    /// Represents a text property.
    /// </summary>
    public class TextPropertyItem : PropertyItem
    {
        public TextPropertyItem(string text)
        {
            this.text = text;
        }
        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <param name="visitor">A visitor.</param>
        public override void AcceptVisitor(IPropertyItemVisitor visitor)
        {
            visitor.VisitTextPropertyItem(this);
        }
        /// <summary>
        /// The text value of this property.
        /// </summary>
        public string Text
        {
            get { return this.text; }
        }

        private string text;
    }

    /// <summary>
    /// Represents a binary property.
    /// </summary>
    public class BinaryPropertyItem : PropertyItem
    {
        public BinaryPropertyItem(ICollection<byte> data)
        {
            this.data = data;
        }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <param name="visitor">A visitor.</param>
        public override void AcceptVisitor(IPropertyItemVisitor visitor)
        {
            visitor.VisitBinaryPropertyItem(this);
        }

        /// <summary>
        /// Binary data belonging to this property.
        /// </summary>
        public ICollection<byte> Data
        {
            get { return this.data; }
        }

        private ICollection<byte> data;
    }

    /// <summary>
    /// Visitor for visiting property items.
    /// </summary>
    public interface IPropertyItemVisitor
    {
        /// <summary>
        /// Visit a text property.
        /// </summary>
        /// <param name="item"></param>
        void VisitTextPropertyItem(TextPropertyItem item);

        /// <summary>
        /// Visit a binary property.
        /// </summary>
        /// <param name="item"></param>
        void VisitBinaryPropertyItem(BinaryPropertyItem item);
    }


}



