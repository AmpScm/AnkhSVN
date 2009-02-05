// $Id$
//
// Copyright 2004-2009 The AnkhSVN Project
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
using SharpSvn;
using System.Collections.Generic;
using Ankh.Scc;

namespace Ankh.UI.PathSelector
{
    public enum PathSelectorOptions
    {
        NoRevision,
        DisplaySingleRevision,
        DisplayRevisionRange
    }
    /// <summary>
    /// Summary description for PathSelector.
    /// </summary>
    public partial class PathSelector : VSDialogForm
    {
        PathSelectorInfo _info;
        PathSelectorOptions _options;
        
        protected PathSelector()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            
            this.Options = PathSelectorOptions.NoRevision;
        }

        int _revStartOffset;
        int _revEndOffset;
        int _suppressOffset;
        int _buttonOffset;
        int _bottomSpacing;

        private void SaveSizes()
        {
            _revStartOffset = fromPanel.Top - pathSelectionTreeView.Bottom;
            _revEndOffset = toPanel.Top - fromPanel.Bottom;
            _suppressOffset = suppressGroupBox.Top - toPanel.Bottom;
            _buttonOffset = bottomPanel.Top - suppressGroupBox.Bottom;
            _bottomSpacing = ClientSize.Height - bottomPanel.Bottom;
        }

        public PathSelector(PathSelectorInfo info)
            : this()
        {
            _info = info;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                SaveSizes();

                EnsureSelection();
                UpdateLayout();                
            }
        }

        void EnsureSelection()
        {
            EnableRecursive = _info.EnableRecursive;
            Items = _info.VisibleItems;
            //selector.CheckedFilter = _info.CheckedFilter;
            Recursive = _info.Depth == SvnDepth.Infinity;
            SingleSelection = _info.SingleSelection;
            Caption = _info.Caption;

            // do we need go get a revision range?
            if (_info.RevisionStart == null && _info.RevisionEnd == null)
            {
                Options = PathSelectorOptions.NoRevision;
            }
            else if (_info.RevisionEnd == null)
            {
                RevisionStart = _info.RevisionStart;
                Options = PathSelectorOptions.DisplaySingleRevision;
            }
            else
            {
                RevisionStart = _info.RevisionStart;
                RevisionEnd = _info.RevisionEnd;
                Options = PathSelectorOptions.DisplayRevisionRange;
            }
            pathSelectionTreeView.CheckedFilter += _info.EvaluateChecked;
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);

            revisionPickerStart.Context = Context;
            revisionPickerEnd.Context = Context;
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
            set
            {
                this.pathSelectionTreeView.Items = value;
                if (value != null)
                {
                    SvnItem parent = SvnItem.GetCommonParent(value);

                    if (parent != null && parent.IsVersioned)
                        revisionPickerEnd.SvnOrigin = revisionPickerStart.SvnOrigin = new SvnOrigin(parent);
                }
            }
        }

        /// <summary>
        /// The items checked in the treeview.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<SvnItem> CheckedItems
        {
            get { return this.pathSelectionTreeView.CheckedItems; }
        }

        /// <summary>
        /// Whether the "Recursive" checkbox should be enabled
        /// </summary>
        public bool EnableRecursive
        {
            get { return this.recursiveCheckBox.Enabled; }
            set { this.recursiveCheckBox.Visible = this.recursiveCheckBox.Enabled = value; }
        }

        /// <summary>
        /// Whether only a single item can be checked.
        /// </summary>
        public bool SingleSelection
        {
            get { return this.pathSelectionTreeView.SingleCheck; }
            set { this.pathSelectionTreeView.SingleCheck = value; }
        }

        /// <summary>
        /// Whether the selection in the treeview is recursive.
        /// </summary>
        public bool Recursive
        {
            get { return this.recursiveCheckBox.Checked; }
            set
            {
                this.recursiveCheckBox.Checked = value;
                this.pathSelectionTreeView.Recursive = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnRevision RevisionStart
        {
            get { return this.revisionPickerStart.Revision; }
            set { this.revisionPickerStart.Revision = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnRevision RevisionEnd
        {
            get { return this.revisionPickerEnd.Revision; }
            set { this.revisionPickerEnd.Revision = value; }
        }


        public PathSelectorOptions Options
        {
            get { return this._options; }
            set
            {
                _options = value;
                switch (value)
                {
                    case PathSelectorOptions.NoRevision:
                        toPanel.Visible = toPanel.Enabled = false;
                        fromPanel.Visible = fromPanel.Enabled = false;
                        break;
                    case PathSelectorOptions.DisplaySingleRevision:
                        fromPanel.Visible = fromPanel.Enabled = true;
                        toPanel.Visible = toPanel.Enabled = false;
                        break;
                    case PathSelectorOptions.DisplayRevisionRange:
                        fromPanel.Visible = fromPanel.Enabled = true;
                        toPanel.Visible = toPanel.Enabled = true;
                        break;
                    default:
                        throw new ArgumentException("Invalid value for Options");
                }

                if (IsHandleCreated)
                    UpdateLayout();
            }
        }

        void UpdateLayout()
        {
            int y = ClientSize.Height - _bottomSpacing;

            if (bottomPanel.Visible)
            {
                if (y != bottomPanel.Bottom)
                    bottomPanel.Top = y - bottomPanel.Height;

                y = bottomPanel.Top - _bottomSpacing;
            }

            if (suppressGroupBox.Visible)
            {
                if (y != suppressGroupBox.Bottom)
                    suppressGroupBox.Top = y - suppressGroupBox.Height;

                y = suppressGroupBox.Top - _suppressOffset;
            }

            if (toPanel.Visible)
            {
                if (y != toPanel.Bottom)
                    toPanel.Top = y - toPanel.Height;

                y = toPanel.Top - _revEndOffset;
            }

            if (fromPanel.Visible)
            {
                if (y != fromPanel.Bottom)
                    fromPanel.Top = y - fromPanel.Height;

                y = fromPanel.Top - _revStartOffset;
            }

            y -= _revStartOffset;

            if (y != pathSelectionTreeView.Bottom)
            {
                int n = pathSelectionTreeView.Bottom - y;
                pathSelectionTreeView.Height -= n;                

                if (n < 0)
                {
                    Height += n;
                }
            }

            int nv = pathSelectionTreeView.VisibleCount;

            if(nv > 5 && nv > _info.VisibleItems.Count * 2)
            {
                int height = (pathSelectionTreeView.Height * 3) / 2  / nv;

                height = Math.Max(5, _info.VisibleItems.Count+3) * height;

                if(height < pathSelectionTreeView.Height)
                    Height -= pathSelectionTreeView.Height - height;
            }
        }

        internal PathSelectionTreeView TreeView
        {
            get { return this.pathSelectionTreeView; }
        }

        protected Button OkButton
        {
            get { return this.okButton; }
        }

        protected Button DoCancelButton
        {
            get { return this.cancelButton; }
        }

        protected VersionSelector RevisionPickerStart
        {
            get { return this.revisionPickerStart; }
        }

        protected VersionSelector RevisionPickerEnd
        {
            get { return this.revisionPickerEnd; }
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

        private void RecursiveCheckedChanged(object sender, System.EventArgs e)
        {
            this.pathSelectionTreeView.Recursive = this.recursiveCheckBox.Checked;
        }        
    }
}
