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
using Ankh.UI.PendingChanges.Commits;

namespace Ankh.UI.PathSelector
{
    enum PathSelectorOptions
    {
        NoRevision,
        DisplaySingleRevision,
        DisplayRevisionRange
    }

    /// <summary>
    /// Summary description for PathSelector.
    /// </summary>
    public partial class CommonFileSelectorDialog : VSDialogForm
    {
        PathSelectorOptions _options;
        
        public CommonFileSelectorDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            
            this.Options = PathSelectorOptions.NoRevision;
        }

        int _revStartOffset;
        int _revEndOffset;
        int _buttonOffset;
        int _bottomSpacing;

        private void SaveSizes()
        {
            _revStartOffset = fromPanel.Top - pendingList.Bottom;
            _revEndOffset = toPanel.Top - fromPanel.Bottom;
            _buttonOffset = bottomPanel.Top - toPanel.Bottom;
            _bottomSpacing = ClientSize.Height - bottomPanel.Bottom;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;
            
            SaveSizes();

            EnsureSelection();
            UpdateLayout();

            LoadItems(_items);

            UpdateOkButton();
        }

        private void UpdateOkButton()
        {
            okButton.Enabled = pendingList.HasCheckedItems;
        }

        class ItemLister : AnkhService, IEnumerable<PendingChange>
        {
            readonly IEnumerable<SvnItem> _items;

            IPendingChangesManager _pcm;
            readonly PendingChange.RefreshContext _rc;
            public ItemLister(IAnkhServiceProvider context, IEnumerable<SvnItem> items)
                : base(context)
            {
                if (items == null)
                    throw new ArgumentNullException("items");
                _items = items;
                _rc = new PendingChange.RefreshContext(context);
            }

            #region IEnumerable<PendingChange> Members

            IPendingChangesManager Manager
            {
                get { return _pcm ?? (_pcm = GetService<IPendingChangesManager>()); }
            }

            readonly Dictionary<string, PendingChange> _pcs = new Dictionary<string, PendingChange>(StringComparer.OrdinalIgnoreCase);

            public IEnumerator<PendingChange> GetEnumerator()
            {
                foreach (SvnItem item in _items)
                {
                    if (item.IsFile && !item.IsLocked)
                    {
                        PendingChange pc = Manager[item.FullPath];

                        if (pc == null && !_pcs.TryGetValue(item.FullPath, out pc))
                        {
                            pc = new PendingChange(_rc, item);
                        }

                        if (pc == null)
                            yield break; // Not a pending change

                        _pcs[item.FullPath] = pc;

                        yield return pc;
                    }
                }
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        public void LoadItems(IEnumerable<SvnItem> iEnumerable)
        {
            LoadChanges(new ItemLister(Context, iEnumerable));
        }

        public void LoadChanges(IEnumerable<Ankh.Scc.PendingChange> changeWalker)
        {
            if (changeWalker == null)
                throw new ArgumentNullException("changeWalker");

            if (pendingList.Context == null && Context != null)
            {
                pendingList.Context = Context;
                pendingList.SelectionPublishServiceProvider = Context;
                pendingList.OpenPendingChangeOnDoubleClick = true;
            }

            _changeEnumerator = changeWalker;
            Reload();
        }

        bool _loading;
        private void Reload()
        {
            Dictionary<PendingChange, PendingCommitItem> chk = null;
            _loading = true;
            if (pendingList.Items.Count > 0)
            {
                chk = new Dictionary<PendingChange, PendingCommitItem>();

                foreach (PendingCommitItem it in pendingList.Items)
                {
                    chk.Add(it.PendingChange, it);
                    it.Group = null;
                }
            }

            pendingList.ClearItems();

            pendingList.BeginUpdate();
            List<PendingCommitItem> newItems = new List<PendingCommitItem>();
            foreach (PendingChange pc in _changeEnumerator)
            {
                PendingCommitItem pi;
                if (chk != null && chk.TryGetValue(pc, out pi))
                {
                    pendingList.Items.Add(pi);
                    pi.RefreshText(Context);
                }
                else
                    newItems.Add(new PendingCommitItem(pendingList, pc));
            }
            pendingList.Items.AddRange(newItems.ToArray());

            _loading = false;
            pendingList.EndUpdate();

            EnsureSelection();
            UpdateCheckedItems();
        }

        void EnsureSelection()
        {
            //selector.CheckedFilter = _info.CheckedFilter;
            // do we need go get a revision range?
            if (RevisionStart == null && RevisionEnd == null)
            {
                Options = PathSelectorOptions.NoRevision;
            }
            else if (RevisionEnd == null)
            {
                Options = PathSelectorOptions.DisplaySingleRevision;
            }
            else
            {
                Options = PathSelectorOptions.DisplayRevisionRange;
            }
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);

            revisionPickerStart.Context = Context;
            revisionPickerEnd.Context = Context;
            pendingList.Context = Context;
            pendingList.SelectionPublishServiceProvider = Context;
        }

        List<SvnItem> _items = new List<SvnItem>();
        private IEnumerable<PendingChange> _changeEnumerator;

        /// <summary>
        /// The items to put in the treeview.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICollection<SvnItem> Items
        {
            get { return _items; }
            set
            {
                _items.Clear();
                if (value != null)
                    _items.AddRange(value);
                // TODO: Update the revisionpicker origin

                if (value != null)
                {
                    SvnItem parent = SvnItem.GetCommonParent(_items);

                    if (parent != null && parent.IsVersioned)
                        revisionPickerEnd.SvnOrigin = revisionPickerStart.SvnOrigin = new SvnOrigin(parent);
                }
            }
        }

        /// <summary>
        /// The items checked in the treeview.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<SvnItem> GetCheckedItems()
        {
            foreach (PendingCommitItem pci in pendingList.CheckedItems)
            {
                yield return pci.PendingChange.SvnItem;
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


        internal PathSelectorOptions Options
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
                        fromLabel.Visible = fromLabel.Enabled = false;
                        break;
                    case PathSelectorOptions.DisplayRevisionRange:
                        fromPanel.Visible = fromPanel.Enabled = true;
                        toPanel.Visible = toPanel.Enabled = true;
                        fromLabel.Visible = fromLabel.Enabled = true;
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

            pendingList.Height = y - pendingList.Top;
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

        private void pendingList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_loading)
                return;
            UpdateCheckedItems();
        }

        void UpdateCheckedItems()
        {
            if (pendingList.HasCheckedItems)
            {
                SvnItem parent = SvnItem.GetCommonParent(GetCheckedItems());

                if (parent != null && parent.IsVersioned)
                    revisionPickerEnd.SvnOrigin = revisionPickerStart.SvnOrigin = new SvnOrigin(parent);
            }
            UpdateOkButton();
        }

        private void suppressLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
