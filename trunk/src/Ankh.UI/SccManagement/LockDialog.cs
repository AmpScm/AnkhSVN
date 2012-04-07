// $Id$
//
// Copyright 2005-2009 The AnkhSVN Project
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
using System.ComponentModel.Design;
using System.Windows.Forms;

using Microsoft.VisualStudio;

using Ankh.Commands;
using Ankh.UI.PathSelector;
using Ankh.Scc;
using Ankh.UI.PendingChanges.Commits;

namespace Ankh.UI.SccManagement
{
    /// <summary>
    /// The dialog to lock SVN items.
    /// </summary>
    public partial class LockDialog : VSContainerForm
    {
        public LockDialog()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            logMessage.Select();
            Message = _originalText;

            VSCommandHandler.Install(Context, logMessage, new CommandID(VSConstants.VSStd2K, (int)VSConstants.VSStd2KCmdID.OPENLINEABOVE), new EventHandler<CommandEventArgs>(OnLock));
        }

        void OnLock(object sender, CommandEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public IEnumerable<SvnItem> GetCheckedItems()
        {
            foreach (PendingCommitItem pci in pendingList.CheckedItems)
            {
                yield return pci.PendingChange.SvnItem;
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

            UpdateOkButton();
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);

            if (pendingList.Context != null && Context != null)
            {
                pendingList.Context = Context;
                pendingList.SelectionPublishServiceProvider = Context;
            }
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

        string _originalText;
        private IEnumerable<Scc.PendingChange> _changeEnumerator;
        public string Message
        {
            get { return this.logMessage.Text; }
            set { this.logMessage.Text = _originalText = value; }
        }

        public bool StealLocks
        {
            get { return this.stealLocksCheckBox.Checked; }
            set { this.stealLocksCheckBox.Checked = value; }
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
            UpdateOkButton();
        }

        void UpdateOkButton()
        {
            okButton.Enabled = (pendingList.CheckedItems.Count > 0);
        }
    }
}

