// Copyright 2008 The AnkhSVN Project
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
using System.Drawing;
using System.Windows.Forms;
using Ankh.Collections;
using Ankh.Commands;
using Ankh.Scc;
using Ankh.UI.PendingChanges.Conflicts;
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;

namespace Ankh.UI.PendingChanges
{
    partial class PendingConflictsPage : PendingChangesPage
    {
        IPendingChangesManager _manager;
        IAnkhCommandService _commandService;
        bool _loaded;
        bool _refreshingList;

        public PendingConflictsPage()
        {
            InitializeComponent();
            SetupResolutionPanel();
            conflictHeader.Text = PendingConflictLogic.FormatHeader(0, 0);
            conflictEditSplitter.Panel2Collapsed = true;
            Disposed += delegate { UnhookManager(); };
        }

        protected override Type PageType
        {
            get { return typeof(PendingConflictsPage); }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            conflictView.Context = Context;
            conflictView.SelectedIndexChanged += conflictView_SelectedIndexChanged;
            conflictView.DoubleClick += conflictView_DoubleClick;

            ApplyPageTheme();

            conflictView.ColumnWidthChanged += conflictView_ColumnWidthChanged;
            IDictionary<string, int> widths = ConfigurationService.GetColumnWidths(GetType());
            conflictView.SetColumnWidths(widths);

            HookManager();
            RefreshConflictList();

            _loaded = true;
        }

        void SetupResolutionPanel()
        {
            Button[] buttons =
            {
                resolveButton0,
                resolveButton1,
                resolveButton2,
                resolveButton3,
                resolveButton4,
                resolveButton5,
                resolveButton6,
                resolveButton7
            };
            string[] captions =
            {
                "Edit Conflict...",
                "Mark Resolved",
                "Working Copy",
                "Original",
                "Mine",
                "Theirs",
                "Mine on Conflicts",
                "Theirs on Conflicts"
            };
            AnkhCommand[] commands =
            {
                AnkhCommand.ItemConflictEdit,
                AnkhCommand.ItemResolveMerge,
                AnkhCommand.ItemResolveWorking,
                AnkhCommand.ItemResolveBase,
                AnkhCommand.ItemResolveMineFull,
                AnkhCommand.ItemResolveTheirsFull,
                AnkhCommand.ItemResolveMineConflict,
                AnkhCommand.ItemResolveTheirsConflict
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Text = captions[i];
                buttons[i].Tag = commands[i];
                buttons[i].MinimumSize = new Size(75, 23);
                buttons[i].Enabled = false;
                buttons[i].Click += resolutionButton_Click;
            }

            // The original designer scaffold placed the empty buttons first and
            // the placeholder labels afterward. Present the panel as an actual
            // conflict action area: selection, explanation, actions, refresh.
            resolvePanel.Controls.SetChildIndex(resolveTopLabel, 0);
            resolvePanel.Controls.SetChildIndex(resolveBottomLabel, 1);
            resolvePanel.SetFlowBreak(resolveTopLabel, true);
            resolvePanel.SetFlowBreak(resolveBottomLabel, true);
            resolvePanel.SetFlowBreak(resolveButton7, true);

            resolveTopLabel.Text = "Conflict";
            resolveTopLabel.UseMnemonic = false;
            resolveBottomLabel.Text = "Choose a resolution strategy. Actions apply immediately.";
            resolveLinkLabel.Text = "Refresh conflicts";
            resolveLinkLabel.LinkClicked += resolveLinkLabel_LinkClicked;
        }

        void HookManager()
        {
            if (_manager != null || Context == null)
                return;

            _manager = Context.GetService<IPendingChangesManager>();
            if (_manager == null)
                return;

            _manager.PendingChanges.CollectionChanged += PendingChanges_CollectionChanged;
            _manager.PendingChanges.ItemChanged += PendingChanges_ItemChanged;
            _manager.IsActiveChanged += Manager_IsActiveChanged;

            if (!_manager.IsActive)
            {
                _manager.IsActive = true;
                _manager.FullRefresh(false);
            }
        }

        void UnhookManager()
        {
            if (_manager == null)
                return;

            _manager.PendingChanges.CollectionChanged -= PendingChanges_CollectionChanged;
            _manager.PendingChanges.ItemChanged -= PendingChanges_ItemChanged;
            _manager.IsActiveChanged -= Manager_IsActiveChanged;
            _manager = null;
        }

        void PendingChanges_CollectionChanged(object sender, CollectionChangedEventArgs<PendingChange> e)
        {
            RefreshConflictList();
        }

        void PendingChanges_ItemChanged(object sender, ItemChangedEventArgs<PendingChange> e)
        {
            RefreshConflictList();
        }

        void Manager_IsActiveChanged(object sender, EventArgs e)
        {
            Enabled = _manager != null && _manager.IsActive;
        }

        void RefreshConflictList()
        {
            if (_manager == null || Context == null || _refreshingList)
                return;

            _refreshingList = true;
            try
            {
                HashSet<string> selectedPaths =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (ListViewItem selected in conflictView.SelectedItems)
                {
                    ConflictListItem conflict = selected as ConflictListItem;
                    if (conflict != null)
                        selectedPaths.Add(conflict.FullPath);
                }

                int versionConflicts = 0;
                int treeConflicts = 0;

                conflictView.BeginUpdate();
                try
                {
                    conflictView.Items.Clear();

                    foreach (PendingChange change in _manager.PendingChanges)
                    {
                        if (!PendingConflictLogic.IsConflict(change.Kind))
                            continue;

                        if (change.Kind == PendingChangeKind.TreeConflict)
                            treeConflicts++;
                        else
                            versionConflicts++;

                        ConflictListItem item = new ConflictListItem(conflictView, change);
                        conflictView.Items.Add(item);

                        if (selectedPaths.Contains(item.FullPath))
                            item.Selected = true;
                    }

                    if (conflictView.SelectedItems.Count == 0 && conflictView.Items.Count > 0)
                    {
                        conflictView.Items[0].Selected = true;
                        conflictView.Items[0].Focused = true;
                    }
                }
                finally
                {
                    conflictView.EndUpdate();
                }

                conflictHeader.Text =
                    PendingConflictLogic.FormatHeader(versionConflicts, treeConflicts);
            }
            finally
            {
                _refreshingList = false;
            }

            UpdateResolutionPanel();
        }

        List<PendingChange> GetSelectedConflicts()
        {
            List<PendingChange> selected = new List<PendingChange>();

            foreach (ListViewItem item in conflictView.SelectedItems)
            {
                ConflictListItem conflict = item as ConflictListItem;
                if (conflict != null)
                    selected.Add(conflict.PendingChange);
            }

            return selected;
        }

        void UpdateResolutionPanel()
        {
            if (_refreshingList)
                return;

            List<PendingChange> selected = GetSelectedConflicts();
            bool hasSelection = selected.Count > 0;

            conflictEditSplitter.Panel2Collapsed = !hasSelection;
            if (!hasSelection)
                return;

            bool anyTreeConflict = false;
            bool allTextFiles = true;
            bool singleTextConflict = selected.Count == 1;

            foreach (PendingChange change in selected)
            {
                SvnItem item = change.SvnItem;
                bool isTree = change.Kind == PendingChangeKind.TreeConflict
                    || (item != null && item.IsTreeConflicted);

                anyTreeConflict |= isTree;
                allTextFiles &= item != null && item.IsTextFile;

                if (singleTextConflict)
                {
                    singleTextConflict =
                        !isTree
                        && item != null
                        && item.Status != null
                        && item.Status.LocalTextStatus == SvnStatus.Conflicted;
                }
            }

            resolveTopLabel.Text = selected.Count == 1
                ? "Resolve: " + (string.IsNullOrEmpty(selected[0].RelativePath)
                    ? selected[0].Name
                    : selected[0].RelativePath)
                : "Resolve " + selected.Count + " selected conflicts";

            foreach (Control control in resolvePanel.Controls)
            {
                Button button = control as Button;
                if (button == null || !(button.Tag is AnkhCommand))
                    continue;

                button.Enabled = PendingConflictLogic.CanExecuteResolution(
                    (AnkhCommand)button.Tag,
                    hasSelection,
                    anyTreeConflict,
                    allTextFiles,
                    singleTextConflict);
            }

            resolvePanel.PerformLayout();
            ResizeToFit();
        }

        void conflictView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateResolutionPanel();
        }

        void conflictView_DoubleClick(object sender, EventArgs e)
        {
            if (GetSelectedConflicts().Count == 1)
                ExecuteResolution(AnkhCommand.ItemConflictEdit);
        }

        void resolutionButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null || !(button.Tag is AnkhCommand))
                return;

            ExecuteResolution((AnkhCommand)button.Tag);
        }

        void ExecuteResolution(AnkhCommand command)
        {
            if (Context == null)
                return;

            List<PendingChange> selected = GetSelectedConflicts();
            if (selected.Count == 0)
                return;

            List<string> paths = new List<string>(selected.Count);
            foreach (PendingChange change in selected)
                paths.Add(change.FullPath);

            if (_commandService == null)
                _commandService = Context.GetService<IAnkhCommandService>();

            if (_commandService == null)
                return;

            // Keep the conflict list as the active selection source while a
            // button is clicked; the existing resolve command handlers then
            // operate on the same PendingChange/SvnItem selection as menus do.
            conflictView.Select();
            conflictView.NotifySelectionUpdated();

            CommandResult result = _commandService.ExecCommand(command, true);
            if (result && _manager != null)
                _manager.Refresh(paths);
        }

        void resolveLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RefreshList();
        }

        protected void conflictView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            IDictionary<string, int> widths = conflictView.GetColumnWidths();
            ConfigurationService.SaveColumnsWidths(GetType(), widths);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_loaded)
                ResizeToFit();
        }

        public override void OnThemeChanged(EventArgs e)
        {
            base.OnThemeChanged(e);

            ApplyPageTheme();

            if (VSVersion.VS2012OrLater)
            {
                borderPanel.BorderStyle = BorderStyle.None;
                conflictView.BorderStyle = BorderStyle.None;
            }

            conflictView.Invalidate();
            resolvePanel.Invalidate(true);
        }

        void ApplyPageTheme()
        {
            if (Context == null)
                return;

            // Use the same semantic WinForms palette as the rest of Pending
            // Changes. In particular, do not treat the resolution surface as
            // an inactive title bar; doing so produced mismatched buttons and
            // stale colors after a Visual Studio theme switch.
            if (VSVersion.VS2012OrLater)
            {
                IWinFormsThemingService theming =
                    Context.GetService<IWinFormsThemingService>();
                if (theming != null)
                    theming.ThemeRecursive(this, false);
            }

            ApplyConflictHeaderTheme();
        }

        void ApplyConflictHeaderTheme()
        {
            if (Context == null || SystemInformation.HighContrast)
            {
                conflictHeader.BackColor = SystemColors.Info;
                conflictHeader.ForeColor = SystemColors.InfoText;
                return;
            }

            IAnkhVSColor colors = Context.GetService<IAnkhVSColor>();
            Color color;

            if (colors != null
                && colors.TryGetColor(
                    (__VSSYSCOLOREX)(-207) /* VS2010: VSCOLOR_INFOBACKGROUND */,
                    out color))
            {
                conflictHeader.BackColor = color;
            }
            else
                conflictHeader.BackColor = SystemColors.Info;

            if (colors != null
                && colors.TryGetColor(
                    (__VSSYSCOLOREX)(-208) /* VS2010: VSCOLOR_INFOTEXT */,
                    out color))
            {
                conflictHeader.ForeColor = color;
            }
            else
                conflictHeader.ForeColor = SystemColors.InfoText;
        }

        public override bool CanRefreshList
        {
            get { return true; }
        }

        public override void RefreshList()
        {
            HookManager();
            if (_manager == null)
                return;

            ISvnStatusCache cache = Context.GetService<ISvnStatusCache>();
            if (cache != null)
                cache.ClearCache();

            _manager.FullRefresh(true);
        }

        private void ResizeToFit()
        {
            if (conflictEditSplitter.Panel2Collapsed)
                return;

            int desiredBottom = resolveLinkLabel.Bottom + resolveLinkLabel.Margin.Bottom;
            int delta = conflictEditSplitter.Panel2.Height - desiredBottom;
            if (delta != 0)
            {
                int distance = conflictEditSplitter.SplitterDistance + delta;
                int maximum = Math.Max(
                    conflictEditSplitter.Panel1MinSize,
                    conflictEditSplitter.Height - conflictEditSplitter.Panel2MinSize
                        - conflictEditSplitter.SplitterWidth);

                conflictEditSplitter.SplitterDistance =
                    Math.Max(conflictEditSplitter.Panel1MinSize, Math.Min(distance, maximum));
            }
        }
    }
}
