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
using Ankh.Collections;

namespace Ankh.UI.PendingChanges
{
    partial class PendingCommitsPage : PendingChangesPage
    {
        PendingCommitsView pendingCommits;
        IPendingChangeControl pendingChangeControl;
        IPendingChangeUI _ui;

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
            if (!VSVersion.VS2010OrLater)
#else
            if (true)
#endif
            {
                pendingCommitsControl = pendingCommits = new PendingCommitsView(this.components);
                _ui = pendingCommits;
            }
            else
            {
                // Load WPF control
                IPendingChangeControlFactory factory = Context.GetService<IPendingChangeControlFactory>();

                pendingChangeControl = factory.Create(Context, this.components);

                pendingCommitsControl = pendingChangeControl.Control;
                _ui = pendingChangeControl.UI;
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

        protected IPendingChangeUI UI
        {
            get { return _ui; }
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
                pendingCommits.KeyUp += new System.Windows.Forms.KeyEventHandler(this.pendingCommits_KeyUp);

                pendingCommits.SelectionPublishServiceProvider = Context;
                pendingCommits.Context = Context;
                pendingCommits.OpenPendingChangeOnDoubleClick = true;
                pendingCommits.HookCommands();
                pendingCommits.ColumnWidthChanged += new ColumnWidthChangedEventHandler(pendingCommits_ColumnWidthChanged);
                IDictionary<string, int> widths = ConfigurationService.GetColumnWidths(GetType());
                pendingCommits.SetColumnWidths(widths);

                logMessageEditor.PendingChangeUI = this.pendingCommits;
            }
            else
                logMessageEditor.PendingChangeUI = this.pendingChangeControl.UI;

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

            UI.Items = _manager.PendingChanges;

            if (pendingCommits != null)
            {
                if (pendingCommits.SmallImageList == null)
                {
                    IFileIconMapper mapper = Context.GetService<IFileIconMapper>();

                    pendingCommits.SmallImageList = mapper.ImageList;
                }

                _manager.IsActiveChanged += OnPendingChangesActiveChanged;
                _manager.BatchUpdateStarted += new EventHandler<BatchStartedEventArgs>(OnBatchUpdateStarted);
            }

            _manager.Changed += OnPendingChangesChanged;

            if (!_manager.IsActive)
            {
                _manager.IsActive = true;
                _manager.FullRefresh(false);
            }

            AnkhServiceEvents ev = Context.GetService<AnkhServiceEvents>();

            ev.SolutionClosed += new EventHandler(OnSolutionRefresh);
            ev.SolutionOpened += new EventHandler(OnSolutionRefresh);
            OnSolutionRefresh(this, EventArgs.Empty);
        }

        void OnBatchUpdateStarted(object sender, BatchStartedEventArgs e)
        {
            if (this.pendingCommits != null)
                e.Disposers += this.pendingCommits.BeginBatch();
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

         void OnPendingChangesActiveChanged(object sender, EventArgs e)
        {
            // Just ignore for now
            Enabled = Manager.IsActive;
        }

        void OnPendingChangesChanged(object sender, PendingChangeEventArgs e)
        {
            PendingChange pc = e.Change;

            UI.OnChange(pc.FullPath);
        }

        IAnkhSolutionSettings _settings;
        IAnkhSolutionSettings Settings
        {
            get { return _settings ?? (_settings = Context.GetService<IAnkhSolutionSettings>()); }
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
                    if (!UI.HasCheckedItems)
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
            List<PendingChange> changes = new List<PendingChange>(UI.CheckedItems);

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

            List<PendingChange> changes = new List<PendingChange>(UI.CheckedItems);

            if (Context.GetService<IPendingChangeHandler>().CreatePatch(changes, a))
            {
            }
        }

        internal bool CanCommit(bool keepingLocks)
        {
            foreach (PendingChange pc in UI.CheckedItems)
            {
                if (!keepingLocks || pc.SvnItem.IsLocked)
                    return true;
            }

            return false;
        }

        internal bool CanCreatePatch()
        {
            if (!CanCommit(false))
                return false;

            foreach (PendingChange pc in UI.CheckedItems)
            {
                if (pc.SvnItem.IsModified)
                    return true;
                else if (!pc.SvnItem.IsVersioned && pc.SvnItem.IsVersionable && pc.SvnItem.InSolution)
                    return true; // Will be added
            }

            return false;
        }

        internal bool CanApplyToWorkingCopy()
        {
            foreach (PendingChange pc in UI.CheckedItems)
            {
                if (pc.CanApply)
                    return true;
            }

            return false;
        }

        internal void ApplyToWorkingCopy()
        {
            List<PendingChange> changes = new List<PendingChange>(UI.CheckedItems);

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

        internal PendingCommitsView PendingCommitsView
        {
            get { return pendingCommits; }
        }
    }
}
