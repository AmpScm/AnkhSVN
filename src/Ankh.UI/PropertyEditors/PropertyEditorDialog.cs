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
    public partial class PropertyEditorDialog : VSDialogForm
    {
        readonly SortedList<string,PropertyEditItem> _propItems;
        SvnNodeKind _currentNodeKind;
        bool _revisionProps;

        public PropertyEditorDialog(string pathText)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            _propItems = new SortedList<string, PropertyEditItem>();

            this.svnItemLabel.Text = pathText ?? "";
        }

        public PropertyEditorDialog(SvnItem svnItem) : this(svnItem.FullPath)
        {
            _currentNodeKind = svnItem.NodeKind;
        }

        public PropertyEditorDialog(SvnUriTarget target, bool revisionProps)
            : this(target.Uri, target.Revision, revisionProps)
        {
            _currentNodeKind = SvnNodeKind.None;
            _revisionProps = revisionProps;
        }

        public PropertyEditorDialog(Uri target, SvnRevision revision, bool revisionProps)
            : this(revisionProps ? string.Format(PropertyEditStrings.RevisionXPropertiesFromY, revision, target) : target.ToString())
        {
            _currentNodeKind = SvnNodeKind.None;
            _revisionProps = revisionProps;
        }

        /// <summary>
        /// Gets the list view.
        /// </summary>
        /// <value>The list view.</value>
        public Ankh.UI.VSSelectionControls.SmartListView ListView
        {
            get { return propListView; }
        }

        /// <summary>
        /// Sets and gets property items.
        /// </summary>
        public PropertyEditItem[] PropertyValues
        {
            get
            {
                PropertyEditItem[] list = new PropertyEditItem[_propItems.Count];

                _propItems.Values.CopyTo(list, 0);

                return list;
            }
            set
            {
                _propItems.Clear();
                if (value != null)
                    foreach (PropertyEditItem pv in value)
                    {
                        _propItems[pv.PropertyName] = pv;
                    }

                PopulateListView();
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

            List<PropertyEditItem> items = new List<PropertyEditItem>();

            foreach (PropertyEditItem i in _propItems.Values)
            {
                if (i.Value != null || i.BaseValue != null)
                {
                    items.Add(i);
                    i.Refresh();
                }
            }
            propListView.Items.AddRange(items.ToArray());
        }

        /// <summary>
        /// Brings up the Property Dialog in edit mode.
        /// </summary>
        private void editButton_Click(object sender, System.EventArgs e)
        {
            PropertyEditItem item = (PropertyEditItem)this.propListView.SelectedItems[0];

            using (PropertyDialog pDialog = new PropertyDialog(item.Value, _currentNodeKind))
            {
                if (pDialog.ShowDialog(Context) != DialogResult.OK)
                    return;

                SvnPropertyValue value = pDialog.GetPropertyItem();
                if (value != null)
                {
                    if (value.Key != item.PropertyName)
                    {
                        item.Value = null; // Deleted
                        item.Refresh();

                        PropertyEditItem pi;

                        if (!_propItems.TryGetValue(value.Key, out pi))
                            _propItems[value.Key] = pi = new PropertyEditItem(propListView, value.Key);

                        pi.Value = value;

                        PopulateListView(); // Add new item
                    }
                    else
                    {
                        item.Value = value;
                        item.Refresh();
                    }
                }
                else
                {
                    item.Value = null;
                    if(item.BaseValue == null)
                    {
                        _propItems.Remove(item.PropertyName);
                        propListView.Items.Remove(item);
                    }
                    else
                        item.Refresh();
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
            PropertyEditItem item = (PropertyEditItem)this.propListView.SelectedItems[0];
            item.Value = null;
            if (item.BaseValue == null)
            {
                propListView.Items.Remove(item);
            }
            else
            {
                item.Refresh();
            }
            this.UpdateButtons();
        }

        private void revertButton_Click(object sender, EventArgs e)
        {
            PropertyEditItem item = (PropertyEditItem)this.propListView.SelectedItems[0];
            item.Value = item.BaseValue;
            if (item.BaseValue == null)
            {
                propListView.Items.Remove(item);
            }
            else
            {
                item.Refresh();
            }

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
                if (propDialog.ShowDialog(Context) != DialogResult.OK)
                    return;

                SvnPropertyValue value = propDialog.GetPropertyItem();
                if (value != null)
                {
                    PropertyEditItem pi;
                    if (!_propItems.TryGetValue(value.Key, out pi))
                        _propItems[value.Key] = pi = new PropertyEditItem(propListView, value.Key);
                    
                    pi.Value = value;
                    pi.Refresh();

                    if (!propListView.Items.Contains(pi))
                        PopulateListView();
                 
                    this.UpdateButtons();
                }
            }
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
        /// Validate the form.
        /// </summary>
        private void ValidateForm()
        {
        }

        private void UpdateButtons()
        {
            PropertyEditItem selection = null;
            if (this.propListView.SelectedItems.Count > 0)
            {
                selection = (PropertyEditItem)this.propListView.SelectedItems[0];
            }

            this.deleteButton.Enabled = selection != null && selection.Value != null;
            this.editButton.Enabled = selection != null;
            this.revertButton.Enabled = selection != null && ((selection.Value != null) != (selection.BaseValue != null) || (selection.Value != null && !selection.Value.Equals(selection.BaseValue)));
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (DesignMode)
                return;

            ResizeGrid();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
                ResizeGrid();
        }

        private void ResizeGrid()
        {
            if (propListView != null)
                propListView.ResizeColumnsToFit(baseValueColumn, valueColumn);            
        }

        private void propListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hti = propListView.HitTest(e.Location);

            if (hti.Location != ListViewHitTestLocations.None && hti.Item != null && hti.Item.Selected && editButton.Enabled)
            {
                editButton_Click(sender, e);
            }
        }        
    }
}



