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

            if (pendingList.Context == null && Context != null)
            {
                pendingList.Context = Context;
                pendingList.SelectionPublishServiceProvider = Context;
            }

            _changeEnumerator = changeWalker;
            Reload();
        }

        private void Reload()
        {
            Dictionary<PendingChange, PendingCommitItem> chk = null;

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

            foreach (PendingChange pc in _changeEnumerator)
            {
                PendingCommitItem pi;
                if (chk != null && chk.TryGetValue(pc, out pi))
                {
                    pendingList.Items.Add(pi);
                    pi.RefreshText(Context);
                }
                else
                    pendingList.Items.Add(new PendingCommitItem(pendingList, pc));
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
                    if (PendingChange.IsPending(item))
                    {
                        PendingChange pc = Manager[item.FullPath];

                        if (pc == null && !_pcs.TryGetValue(item.FullPath, out pc))
                        {
                            PendingChange.CreateIfPending(_rc, item, out pc);
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

        private void pendingList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            okButton.Enabled = sender is ListView
                && ((ListView)sender).CheckedItems.Count > 0;
        }
    }
}
