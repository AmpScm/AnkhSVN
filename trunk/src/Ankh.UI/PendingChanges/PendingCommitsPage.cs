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
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio;

using SharpSvn;

using Ankh.Commands;
using Ankh.Configuration;
using Ankh.Scc;
using Ankh.UI.PendingChanges.Commits;
using Ankh.VS;

namespace Ankh.UI.PendingChanges
{
    partial class PendingCommitsPage : PendingChangesPage
    {
        PendingCommitsView pendingCommits;
        IPendingChangeControl pendingChangeControl;

        public PendingCommitsPage()
        {
            InitializeComponent();
        }

        bool _insertedPendingChanges;
        void InsertPendingChanges()
        {
            if (_insertedPendingChanges)
                return;
            _insertedPendingChanges = true;
            splitContainer.SuspendLayout();
            splitContainer.Panel2.Controls.Clear();

            Control pendingCommitsControl;
#if DEBUG
            if (!VSVersion.VS2012OrLater) /// Enable for VS2010 when theme proxy supports this
#else
            if (true)
#endif
            {
                pendingCommitsControl = pendingCommits = new PendingCommitsView(this.components);
            }
            else
            {
                // Load WPF control
                IPendingChangeControlFactory factory = Context.GetService<IPendingChangeControlFactory>();

                pendingChangeControl = factory.Create(Context, this.components);

                pendingCommitsControl = pendingChangeControl.Control;
            }

            splitContainer.Panel2.Controls.Add(pendingCommitsControl);

            pendingCommitsControl.Dock = changeListDummy.Dock;
            pendingCommitsControl.Location = changeListDummy.Location;
            pendingCommitsControl.Size = changeListDummy.Size;

            changeListDummy.Dispose();
            changeListDummy = null;
            this.splitContainer.ResumeLayout(true);
            
            if (VSVersion.VS2012OrLater)
            {
                if (pendingCommits != null)
                    pendingCommits.BorderStyle = BorderStyle.None;
                borderPanel.BorderStyle = BorderStyle.None;
            }
            else
            {

            }
        }

        AnkhConfig Config
        {
            get { return ConfigurationService.Instance; }
        }

        IAnkhCommandService _commandService;
        IAnkhCommandService CommandService
        {
            get { return _commandService ?? (_commandService = Context.GetService<IAnkhCommandService>()); }
        }

        protected override void OnLoad(EventArgs e)
        {
            InsertPendingChanges();

            base.OnLoad(e);

            if (pendingCommits != null)
            {
                pendingCommits.AllowColumnReorder = true;
                pendingCommits.CheckBoxes = true;
                pendingCommits.HideSelection = false;
                pendingCommits.Name = "pendingCommits";
                pendingCommits.ShowItemToolTips = true;
                pendingCommits.ShowSelectAllCheckBox = true;
                pendingCommits.ResolveItem += new System.EventHandler<Ankh.UI.VSSelectionControls.ListViewWithSelection<Ankh.UI.PendingChanges.Commits.PendingCommitItem>.ResolveItemEventArgs>(this.pendingCommits_ResolveItem);
                pendingCommits.KeyUp += new System.Windows.Forms.KeyEventHandler(this.pendingCommits_KeyUp);

                pendingCommits.SelectionPublishServiceProvider = Context;
                pendingCommits.Context = Context;
                pendingCommits.OpenPendingChangeOnDoubleClick = true;
                pendingCommits.HookCommands();
                pendingCommits.ColumnWidthChanged += new ColumnWidthChangedEventHandler(pendingCommits_ColumnWidthChanged);
                IDictionary<string, int> widths = ConfigurationService.GetColumnWidths(GetType());
                pendingCommits.SetColumnWidths(widths);

                logMessageEditor.PasteSource = this.pendingCommits;
            }
            else
                logMessageEditor.PasteSource = this.pendingChangeControl.PendingChangeSource;

            Context.GetService<AnkhServiceEvents>().LastChanged += OnLastChanged;

            VSCommandHandler.Install(Context, this, new CommandID(VSConstants.VSStd2K, 1635 /* cmdidExploreFolderInWindows */), OnOpenFolder, OnUpdateOpenFolder);

            HookList();
        }

        private void OnUpdateOpenFolder(object sender, CommandUpdateEventArgs e)
        {
            SvnItem one = EnumTools.GetSingle(e.Selection.GetSelectedSvnItems(false));

            if (one == null || !one.Exists)
                e.Enabled = false;
        }

        private void OnOpenFolder(object sender, CommandEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.Exists)
                    System.Diagnostics.Process.Start(item.IsDirectory ? item.FullPath : SvnTools.GetNormalizedDirectoryName(item.FullPath));
            }
        }

        protected void pendingCommits_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            IDictionary<string, int> widths = pendingCommits.GetColumnWidths();
            ConfigurationService.SaveColumnsWidths(GetType(), widths);
        }

        IPendingChangesManager _manager;
        private void HookList()
        {
            if (_manager != null || Context == null)
                return;

            _manager = Context.GetService<IPendingChangesManager>();

            if (_manager == null)
                return;

            if (pendingCommits != null)
            {
                if (pendingCommits.SmallImageList == null)
                {
                    IFileIconMapper mapper = Context.GetService<IFileIconMapper>();

                    pendingCommits.SmallImageList = mapper.ImageList;
                }

                _manager.PendingChanges.CollectionChanged += OnPendingChangesChanged;
                _manager.Changed += new EventHandler<PendingChangeEventArgs>(OnPendingChangesChanged);
                _manager.IsActiveChanged += new EventHandler<PendingChangeEventArgs>(OnPendingChangesActiveChanged);
                _manager.BatchUpdateStarted += new EventHandler<BatchStartedEventArgs>(OnBatchUpdateStarted);
            }
            else if (pendingChangeControl != null)
            {
                pendingChangeControl.PendingChanges = _manager.PendingChanges;
            }

            if (!_manager.IsActive)
            {
                _manager.IsActive = true;
                _manager.FullRefresh(false);
            }
            else if (pendingCommits != null)
                PerformInitialUpdate(_manager);

            AnkhServiceEvents ev = Context.GetService<AnkhServiceEvents>();

            ev.SolutionClosed += new EventHandler(OnSolutionRefresh);
            ev.SolutionOpened += new EventHandler(OnSolutionRefresh);
            OnSolutionRefresh(this, EventArgs.Empty);
        }

        private void OnPendingChangesChanged(object sender, CollectionChangedEventArgs<PendingChange> e)
        {
            if (!IsHandleCreated)
                return;

            switch (e.Action)
            {
                case CollectionChange.Add:
                    foreach(PendingChange pc in e.NewItems)
                        OnPendingChangeAdded(pc);
                    break;
                case CollectionChange.Remove:
                    foreach(PendingChange pc in e.OldItems)
                        OnPendingChangeRemoved(pc);
                    break;
                case CollectionChange.Reset:
                    OnPendingChangesListFlushed();
                    PerformInitialUpdate(_manager);
                    break;
            }
        }

        int _inBatchUpdate;
        List<PendingCommitItem> _toAdd;
        void OnBatchUpdateStarted(object sender, BatchStartedEventArgs e)
        {
            pendingCommits.BeginUpdate();
            _inBatchUpdate++;
            e.Disposers += OnBatchEnd;
        }

        void OnBatchEnd()
        {
            try
            {
                if (--_inBatchUpdate == 0)
                {
                    if (_toAdd != null)
                    {
                        PendingCommitItem[] toAdd = _toAdd.ToArray();
                        _toAdd = null;
                        pendingCommits.Items.AddRange(toAdd);
                    }
                }
            }
            finally
            {
                pendingCommits.EndUpdate();
            }
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
                        issueLabel.Text = pcs.IssueLabel ?? PCResources.IssueLabelText;
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

        void OnPendingChangeAdded(PendingChange pc)
        {
            PendingCommitItem pci;

            string path = pc.FullPath;

            if (_listItems.TryGetValue(path, out pci))
            {
                // Should never happend; will refresh checkbox, etc.
                _listItems.Remove(path);
                pci.Remove();
            }

            pci = new PendingCommitItem(pendingCommits, pc);
            _listItems.Add(path, pci);
            if (_inBatchUpdate > 0)
            {
                if (_toAdd == null)
                    _toAdd = new List<PendingCommitItem>();
                _toAdd.Add(pci);
            }
            else
                pendingCommits.Items.Add(pci);

            // TODO: Maybe add something like
            //pendingCommits.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);

            OnPendingChangeActivity(pc);
        }

        HybridCollection<string> _checkedItems;
        void OnPendingChangesListFlushed()
        {
            if (_listItems.Count > 0)
            {
                _checkedItems = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                foreach (PendingCommitItem pci in _listItems.Values)
                {
                    if (pci.Checked && !_checkedItems.Contains(pci.FullPath))
                        _checkedItems.Add(pci.FullPath);
                }
                _listItems.Clear();
                pendingCommits.ClearItems();
                _toAdd = null;
            }
        }

        void OnPendingChangesActiveChanged(object sender, PendingChangeEventArgs e)
        {
            // Just ignore for now
            Enabled = e.Manager.IsActive;
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
                List<ListViewItem> newItems = new List<ListViewItem>();
                foreach (PendingChange pc in new List<PendingChange>(manager.PendingChanges))
                {
                    PendingCommitItem pi = new PendingCommitItem(pendingCommits, pc);
                    _listItems.Add(pc.FullPath, pi);

                    /* Store the old checked state (checked or unchecked) in pi */
                    if (_checkedItems != null)
                        pi.Checked = _checkedItems.Contains(pc.FullPath);

                    newItems.Add(pi);
                }

                if (newItems.Count > 0)
                    pendingCommits.Items.AddRange(newItems.ToArray());
            }
            finally
            {
                _checkedItems = null;
                pendingCommits.EndUpdate();
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
            OnPendingChangeActivity(e.Change);
        }

        void OnPendingChangeRemoved(PendingChange pc)
        {
            PendingCommitItem pci;

            string path = pc.FullPath;

            if (_listItems.TryGetValue(path, out pci))
            {
                _listItems.Remove(path);
                pci.Remove();

                // Currently probably not necessary, but just to be sure
                if (_toAdd != null)
                    _toAdd.Remove(pci);

                pendingCommits.RefreshGroupsAvailable();
            }
            OnPendingChangeActivity(pc);
        }

        void OnPendingChangeActivity(PendingChange pc)
        {
            IAnkhSolutionSettings settings = null;
            if (true
                && pc != null
                && (settings = Context.GetService<IAnkhSolutionSettings>()) != null
                && string.Equals(settings.ProjectRoot, pc.FullPath)
                // TODO add filter for property changes
                )
            {
                IAnkhIssueService iService = Context.GetService<IAnkhIssueService>();
                if (iService != null)
                {
                    iService.MarkDirty();
                }
            }
        }

        public override bool CanRefreshList
        {
            get { return true; }
        }

        public override void RefreshList()
        {
            Context.GetService<ISvnStatusCache>().ClearCache();

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

        private void pendingCommits_KeyUp(object sender, KeyEventArgs e)
        {
            // TODO: Replace with VS command handling, instead of hooking it with Winforms
            if (e.KeyCode == Keys.Enter)
            {
                // TODO: We should probably open just the focused file instead of the selection in the ItemOpenVisualStudio case to make it more deterministic what file is active after opening
                if (CommandService != null)
                    CommandService.ExecCommand(Config.PCDoubleClickShowsChanges
                        ? AnkhCommand.ItemShowChanges : AnkhCommand.ItemOpenVisualStudio, true);
            }
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

        void OnLastChanged(object sender, LastChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Caption))
                lastRevBox.Enabled = lastRevBox.Visible = lastRevLabel.Enabled = lastRevLabel.Visible = false;
            else
            {
                lastRevLabel.Text = e.Caption ?? "";
                lastRevBox.Text = e.Value ?? "";

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
                logMessageEditor.Clear(true);
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

                if (!keepingLocks || pci.PendingChange.SvnItem.IsLocked)
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

                if (pc.SvnItem.IsModified)
                    return true;
                else if (!pc.SvnItem.IsVersioned && pc.SvnItem.IsVersionable && pc.SvnItem.InSolution)
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

        internal Microsoft.VisualStudio.TextManager.Interop.IVsTextView TextView
        {
            get
            {
                IAnkhHasVsTextView tv = logMessageEditor.ActiveControl as IAnkhHasVsTextView;

                if (tv != null)
                    return tv.TextView;

                return null;
            }
        }

        internal Microsoft.VisualStudio.TextManager.Interop.IVsFindTarget FindTarget
        {
            get
            {
                IAnkhHasVsTextView tv = logMessageEditor.ActiveControl as IAnkhHasVsTextView;

                if (tv != null)
                    return tv.FindTarget;

                return null;
            }
        }
    }
}
