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
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using Ankh.Scc;
using Ankh.Commands;
using Ankh.VS;
using Ankh.UI.PendingChanges.Commits;

namespace Ankh.UI.PendingChanges
{
    partial class PendingCommitsPage : PendingChangesPage, ILastChangeInfo
    {
        public PendingCommitsPage()
        {
            InitializeComponent();
            logMessageEditor.ShowHorizontalScrollBar = false;
            logMessageEditor.PasteSource = pendingCommits;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (pendingCommits != null)
            {
                pendingCommits.SelectionPublishServiceProvider = Context;
                pendingCommits.Context = Context;
                pendingCommits.HookCommands();
            }

            Context.GetService<IServiceContainer>().AddService(typeof(ILastChangeInfo), this);

            HookList();
        }

        IPendingChangesManager _manager;
        private void HookList()
        {
            if (_manager != null || Context == null)
                return;

            if (pendingCommits.SmallImageList == null)
            {
                IFileIconMapper mapper = Context.GetService<IFileIconMapper>();

                pendingCommits.SmallImageList = mapper.ImageList;
            }

            _manager = Context.GetService<IPendingChangesManager>();

            if (_manager == null)
                return;

            _manager.Added += new EventHandler<PendingChangeEventArgs>(OnPendingChangeAdded);
            _manager.Removed += new EventHandler<PendingChangeEventArgs>(OnPendingChangeRemoved);
            _manager.Changed += new EventHandler<PendingChangeEventArgs>(OnPendingChangesChanged);
            _manager.InitialUpdate += new EventHandler<PendingChangeEventArgs>(OnPendingChangesInitialUpdate);
            _manager.IsActiveChanged += new EventHandler<PendingChangeEventArgs>(OnPendingChangesActiveChanged);
            _manager.ListFlushed += new EventHandler<PendingChangeEventArgs>(OnPendingChangesListFlushed);

            if (!_manager.IsActive)
            {
                _manager.IsActive = true;
                _manager.FullRefresh(false);
            }
            else
                PerformInitialUpdate(_manager);

            AnkhServiceEvents ev = Context.GetService<AnkhServiceEvents>();

            ev.SolutionClosed += new EventHandler(OnSolutionRefresh);
            ev.SolutionOpened += new EventHandler(OnSolutionRefresh);
            OnSolutionRefresh(this, EventArgs.Empty);
        }

        void OnSolutionRefresh(object sender, EventArgs e)
        {
            bool showIssueBox = false;

            if (Context != null)
            {
                IProjectCommitSettings pcs = Context.GetService<IProjectCommitSettings>();

                if (pcs != null)
                {
                    showIssueBox = pcs.ShowIssueBox;

                    if (showIssueBox)
                    {
                        issueLabel.Text = pcs.IssueLabel ?? PCStrings.IssueLabelText;
                    }

                    _issueNummeric = pcs.NummericIssueIds;
                }
            }

            if (showIssueBox != issueNumberBox.Visible)
            {
                issueNumberBox.Enabled = issueNumberBox.Visible =
                    issueLabel.Enabled = issueLabel.Visible = showIssueBox;
            }
        }

        protected IPendingChangesManager Manager
        {
            get
            {
                if (_manager == null)
                    HookList();

                return _manager;
            }
        }

        protected override Type PageType
        {
            get
            {
                return typeof(PendingCommitsPage);
            }
        }

        public bool LogMessageVisible
        {
            get { return !splitContainer.Panel1Collapsed; }
            set { splitContainer.Panel1Collapsed = !value; }
        }

        readonly Dictionary<string, PendingCommitItem> _listItems = new Dictionary<string, PendingCommitItem>(StringComparer.OrdinalIgnoreCase);

        void OnPendingChangeAdded(object sender, PendingChangeEventArgs e)
        {
            PendingCommitItem pci;

            string path = e.Change.FullPath;

            if (_listItems.TryGetValue(path, out pci))
            {
                // Should never happend; will refresh checkbox, etc.
                _listItems.Remove(path);
                pci.Remove();
            }

            pci = new PendingCommitItem(pendingCommits, e.Change);
            _listItems.Add(path, pci);
            pendingCommits.Items.Add(pci);

            // TODO: Maybe add something like
            //pendingCommits.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        HybridCollection<string> _checkedItems;
        void OnPendingChangesListFlushed(object sender, PendingChangeEventArgs e)
        {
            if (_listItems.Count > 0)
            {
                _checkedItems = new HybridCollection<string>();
                foreach (PendingCommitItem pci in _listItems.Values)
                {
                    if (pci.Checked && !_checkedItems.Contains(pci.FullPath))
                        _checkedItems.Add(pci.FullPath);
                }
                _listItems.Clear();
                pendingCommits.ClearItems();
            }
        }

        void OnPendingChangesActiveChanged(object sender, PendingChangeEventArgs e)
        {
            // Just ignore for now
            Enabled = e.Manager.IsActive;
        }

        void OnPendingChangesInitialUpdate(object sender, PendingChangeEventArgs e)
        {
            PerformInitialUpdate(e.Manager);
        }

        void PerformInitialUpdate(IPendingChangesManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");

            pendingCommits.BeginUpdate();
            _listItems.Clear(); // Make sure we are clear
            pendingCommits.ClearItems();
            try
            {
                foreach (PendingChange pc in manager.GetAll())
                {
                    PendingCommitItem pi = new PendingCommitItem(pendingCommits, pc);
                    _listItems.Add(pc.FullPath, pi);

                    if (_checkedItems != null)
                        pi.Checked = _checkedItems.Contains(pc.FullPath);

                    pendingCommits.Items.Add(pi);
                }

                _checkedItems = null;
            }
            finally
            {
                pendingCommits.EndUpdate();
                pendingCommits.Invalidate();
            }
        }

        void OnPendingChangesChanged(object sender, PendingChangeEventArgs e)
        {
            PendingCommitItem pci;

            string path = e.Change.FullPath;

            if (!_listItems.TryGetValue(path, out pci))
            {
                pci = new PendingCommitItem(pendingCommits, e.Change);
                _listItems.Add(path, pci);
                pendingCommits.Items.Add(pci);
            }
            else
            {
                if (PendingChange.IsIgnoreOnCommitChangeList(pci.PendingChange.ChangeList)
                    && pci.Checked)
                {
                    // Uncheck items that were moved to the ignore list
                    if (!PendingChange.IsIgnoreOnCommitChangeList(pci.LastChangeList))
                        pci.Checked = false; // Uncheck items that weren't on the ignore list before

                    // Note: We don't check items that were previously ignored, as the user didn't
                    // ask us to do that.
                }

                pci.RefreshText(Context);
            }
        }

        void OnPendingChangeRemoved(object sender, PendingChangeEventArgs e)
        {
            PendingCommitItem pci;

            string path = e.Change.FullPath;

            if (_listItems.TryGetValue(path, out pci))
            {
                _listItems.Remove(path);
                pci.Remove();
                pendingCommits.RefreshGroupsAvailable();
            }
        }

        public override bool CanRefreshList
        {
            get { return true; }
        }

        public override void RefreshList()
        {
            Context.GetService<IFileStatusCache>().ClearCache();

            IAnkhOpenDocumentTracker dt = Context.GetService<IAnkhOpenDocumentTracker>();

            if (dt != null)
                dt.RefreshDirtyState();

            Manager.FullRefresh(true);
        }

        private void pendingCommits_ResolveItem(object sender, PendingCommitsView.ResolveItemEventArgs e)
        {
            PendingChange pc = e.SelectionItem as PendingChange;

            PendingCommitItem pci;
            if (pc != null && this._listItems.TryGetValue(pc.FullPath, out pci))
            {
                e.Item = pci;
            }
        }

        private void pendingCommits_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = pendingCommits.HitTest(e.X, e.Y);

            if (info == null || info.Location == ListViewHitTestLocations.None)
                return;

            if (info.Location == ListViewHitTestLocations.StateImage)
                return; // Just check the item

            IAnkhCommandService cmd = Context.GetService<IAnkhCommandService>();

            if (cmd != null)
                cmd.ExecCommand(Control.ModifierKeys == Keys.Control
                    ? AnkhCommand.ItemShowChanges : AnkhCommand.ItemOpenVisualStudio, true);
        }
        internal void OnUpdate(Ankh.Commands.CommandUpdateEventArgs e)
        {
            switch (e.Command)
            {
                case AnkhCommand.PcLogEditorPasteFileList:
                    foreach (PendingCommitItem pci in _listItems.Values)
                    {
                        if (pci.Checked)
                            return;
                    }
                    e.Enabled = false;
                    return;
                case AnkhCommand.PcLogEditorPasteRecentLog:
                    return;
            }
        }

        internal void OnExecute(Ankh.Commands.CommandEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            switch (e.Command)
            {
                case AnkhCommand.PcLogEditorPasteRecentLog:
                    break;
            }
            if (sb.Length > 0)
                logMessageEditor.PasteText(sb.ToString());
        }

        #region ILastChangeInfo Members

        void ILastChangeInfo.SetLastChange(string caption, string value)
        {
            if (string.IsNullOrEmpty(caption))
                lastRevBox.Enabled = lastRevBox.Visible = lastRevLabel.Enabled = lastRevLabel.Visible = false;
            else
            {
                lastRevLabel.Text = caption ?? "";
                lastRevBox.Text = value ?? "";

                lastRevBox.Enabled = lastRevBox.Visible = lastRevLabel.Enabled = lastRevLabel.Visible = true;
            }
        }

        #endregion

        public void DoCommit(bool keepLocks)
        {
            List<PendingChange> changes = new List<PendingChange>();

            foreach (PendingCommitItem pci in _listItems.Values)
            {
                if (pci.Checked)
                {
                    changes.Add(pci.PendingChange);
                }
            }

            IPendingChangeHandler pch = Context.GetService<IPendingChangeHandler>();

            PendingChangeCommitArgs a = new PendingChangeCommitArgs();
            a.LogMessage = logMessageEditor.Text;
            a.KeepLocks = keepLocks;

            if (issueNumberBox.Visible)
                a.IssueText = issueNumberBox.Text; // The pc handler verifies if it should be used            

            if (pch.Commit(changes, a))
            {
                logMessageEditor.Text = "";
                issueNumberBox.Text = "";
            }
        }

        public void DoCreatePatch(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            PendingChangeCreatePatchArgs a = new PendingChangeCreatePatchArgs();
            a.FileName = fileName;

            IAnkhSolutionSettings ss = Context.GetService<IAnkhSolutionSettings>();
            a.RelativeToPath = ss.ProjectRoot;
            a.AddUnversionedFiles = true;

            List<PendingChange> changes = new List<PendingChange>();

            foreach (PendingCommitItem pci in _listItems.Values)
            {
                if (pci.Checked)
                {
                    changes.Add(pci.PendingChange);
                }
            }

            if (Context.GetService<IPendingChangeHandler>().CreatePatch(changes, a))
            {
            }
        }

        internal bool CanCommit(bool keepingLocks)
        {
            if (_listItems.Count == 0)
                return false;

            foreach (PendingCommitItem pci in _listItems.Values)
            {
                if (!pci.Checked)
                    continue;

                if (!keepingLocks || pci.PendingChange.Item.IsLocked)
                    return true;
            }

            return false;
        }

        internal bool CanCreatePatch()
        {
            if (!CanCommit(false))
                return false;

            foreach (PendingCommitItem pci in _listItems.Values)
            {
                if (!pci.Checked)
                    continue;
                PendingChange pc = pci.PendingChange;

                if (pc.Item.IsModified)
                    return true;
                else if (!pc.Item.IsVersioned && pc.Item.IsVersionable && pc.Item.InSolution)
                    return true; // Will be added                
            }

            return false;
        }

        internal bool CanApplyToWorkingCopy()
        {
            foreach (PendingCommitItem pci in _listItems.Values)
            {
                if (!pci.Checked)
                    continue;

                if (pci.PendingChange.CanApply)
                    return true;
            }

            return false;
        }

        internal void ApplyToWorkingCopy()
        {
            List<PendingChange> changes = new List<PendingChange>();

            foreach (PendingCommitItem pci in _listItems.Values)
            {
                if (!pci.Checked)
                    continue;

                changes.Add(pci.PendingChange);
            }

            PendingChangeApplyArgs args = new PendingChangeApplyArgs();

            if (Context.GetService<IPendingChangeHandler>().ApplyChanges(changes, args))
            {
            }
        }

        bool _issueNummeric;
        private void issueNumberBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_issueNummeric)
            {
                if (!char.IsNumber(e.KeyChar) && e.KeyChar != ',' && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
        }

        private void issueNumberBox_TextChanged(object sender, EventArgs e)
        {
            if (_issueNummeric)
            {
                bool replace = false;
                string txt = issueNumberBox.Text;

                for (int i = 0; i < txt.Length; i++)
                {
                    if (!char.IsNumber(txt, i) && txt[i] != ',')
                    {
                        txt = txt.Remove(i, 1);
                        replace = true;
                    }
                }

                if (replace)
                    issueNumberBox.Text = txt;
            }
        }
    }
}
