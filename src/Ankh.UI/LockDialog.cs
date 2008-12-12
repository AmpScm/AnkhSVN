// $Id$
//
// Copyright 2005-2008 The AnkhSVN Project
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
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Ankh.UI
{
    /// <summary>
    /// The dialog to lock SVN items.
    /// </summary>
    public partial class LockDialog : VSContainerForm
    {
        PathSelectorInfo _info;

        public LockDialog()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
        }

        public LockDialog(PathSelectorInfo info)
            : this()
        {
            this._info = info;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                EnsureSelection();
            }
            Message = _originalText;
        }

        void EnsureSelection()
        {
            Items = _info.VisibleItems;
            //selector.CheckedFilter = _info.CheckedFilter;
            Caption = _info.Caption;
            pathSelectionTreeView.CheckedFilter += _info.EvaluateChecked;
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);

            pathSelectionTreeView.Context = Context;
        }

        /// <summary>
        /// The text to display in the label area.
        /// </summary>
        public string Caption
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        /// <summary>
        /// The items to put in the treeview.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICollection<SvnItem> Items
        {
            get { return this.pathSelectionTreeView.Items; }
            set { this.pathSelectionTreeView.Items = value; }
        }

        /// <summary>
        /// The items checked in the treeview.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<SvnItem> CheckedItems
        {
            get { return this.pathSelectionTreeView.CheckedItems; }
        }

        string _originalText;
        public string Message
        {
            get { return this.logMessageEditor.Text; }
            set { this.logMessageEditor.Text = _originalText = value; }
        }

        public bool StealLocks
        {
            get { return this.stealLocksCheckBox.Checked; }
            set { this.stealLocksCheckBox.Checked = value; }
        }

        protected PathSelectionTreeView TreeView
        {
            get { return this.pathSelectionTreeView; }
        }

        protected Button DoCancelButton
        {
            get { return this.btnCancel; }
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

        private void pathSelectionTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            bool result = false;
            foreach (SvnItem item in this.CheckedItems)
            {
                // enable when a checked file is discovered.
                if (item.IsFile)
                {
                    result = true;
                    break;
                }
            }
            okButton.Enabled = result;
        }
    }
}

