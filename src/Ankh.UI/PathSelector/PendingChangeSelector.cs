// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;
using Ankh.UI.PendingChanges.Commits;

namespace Ankh.UI.PathSelector
{
    public partial class PendingChangeSelector : VSContainerForm
    {
        public PendingChangeSelector()
        {
            InitializeComponent();
        }

        IEnumerable<PendingChange> _changeEnumerator;

        public void LoadChanges(IEnumerable<Ankh.Scc.PendingChange> changeWalker)
        {
            if (changeWalker == null)
                throw new ArgumentNullException("changeWalker");

            _changeEnumerator = changeWalker;

            if (IsHandleCreated)
                Reload();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (pendingList.Context == null && Context != null)
            {
                pendingList.Context = Context;
                pendingList.SelectionPublishServiceProvider = Context;
            }

            Reload();
        }

        IEnumerable<SvnItem> _allItems;
        Predicate<SvnItem> _filter;
        Predicate<SvnItem> _checkedFilter;

        private void Reload()
        {
            PendingChange.RefreshContext rc = new PendingChange.RefreshContext(Context);
            Dictionary<string, bool> checkedCache = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            foreach (PendingCommitItem pci in pendingList.Items)
            {
                checkedCache[pci.FullPath] = pci.Checked;
            }

            pendingList.ClearItems();
            foreach (SvnItem i in _allItems)
            {
                if (_filter != null && !_filter(i))
                    continue;

                PendingChange pc = new PendingChange(rc, i);
                PendingCommitItem pci = new PendingCommitItem(pendingList, pc);

                bool chk;
                if (checkedCache.TryGetValue(i.FullPath, out chk))
                    pci.Checked = chk;
                else if (_checkedFilter != null && !_checkedFilter(i))
                    pci.Checked = false;

                pendingList.Items.Add(pci);
            }
        }

        /// <summary>
        /// Processes a command key.
        /// </summary>
        /// <param name="msg">A <see cref="T:System.Windows.Forms.Message"/>, passed by reference, that represents the Win32 message to process.</param>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values that represents the key to process.</param>
        /// <returns>
        /// true if the keystroke was processed and consumed by the control; otherwise, false to allow further processing.
        /// </returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Return | Keys.Control)
                && okButton.Enabled)
            {
                DialogResult = DialogResult.OK;
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public IEnumerable<PendingChange> GetSelection()
        {
            foreach (PendingCommitItem it in pendingList.Items)
            {
                if (it.Checked)
                    yield return it.PendingChange;
            }
        }

        public IEnumerable<SvnItem> GetSelectedItems()
        {
            foreach (PendingChange pc in GetSelection())
            {
                yield return pc.SvnItem;
            }
        }

        public void LoadItems(IEnumerable<SvnItem> allItems, Predicate<SvnItem> checkedFilter, Predicate<SvnItem> visibleFilter)
        {
            _allItems = allItems;
            _checkedFilter = checkedFilter;
        }

        public void LoadItems(IEnumerable<SvnItem> allItems)
        {
            LoadItems(allItems, PendingChange.IsPending, null);
        }

        private void pendingList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            okButton.Enabled = sender is ListView
                && ((ListView)sender).CheckedItems.Count > 0;
        }
    }
}
