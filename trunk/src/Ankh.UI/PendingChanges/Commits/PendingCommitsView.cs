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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using Ankh.Commands;
using Ankh.Configuration;
using Ankh.Scc;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.UI.PendingChanges.Commits
{
    public interface IPendingChangeSource
    {
        bool HasPendingChanges { get; }
        IEnumerable<PendingChange> PendingChanges { get; }
    }

    class PendingCommitsView : ListViewWithSelection<PendingCommitItem>, IPendingChangeSource
    {
        public PendingCommitsView()
        {
            // TODO initialize to false for now, until all the usage paths are verified
            OpenPendingChangeOnDoubleClick = false;

            StrictCheckboxesClick = true;
            FullRowSelect = true;
            HideSelection = false;
            AllowColumnReorder = true;
            CheckBoxes = true;
            Sorting = SortOrder.Ascending;
            Initialize();
        }

        public PendingCommitsView(IContainer container)
            : this()
        {
            container.Add(this);
        }

        public void Initialize()
        {
            SmartColumn path = new SmartColumn(this, PCResources.PathColumn, 288, "Path");
            SmartColumn project = new SmartColumn(this, PCResources.ProjectColumn, 76, "Project");
            SmartColumn change = new SmartColumn(this, PCResources.ChangeColumn, 76, "Change");
            SmartColumn fullPath = new SmartColumn(this, PCResources.FullPathColumn, 327, "FullPath");

            SmartColumn changeList = new SmartColumn(this, PCResources.ChangeListColumn, 76, "ChangeList");
            SmartColumn folder = new SmartColumn(this, PCResources.FolderColumn, 196, "Folder");
            SmartColumn locked = new SmartColumn(this, PCResources.LockedColumn, 38, "Locked");
            SmartColumn modified = new SmartColumn(this, PCResources.ModifiedColumn, 76, "Modified");
            SmartColumn name = new SmartColumn(this, PCResources.NameColumn, 76, "Name");
            SmartColumn revision = new SmartColumn(this, PCResources.RevisionColumn, 38, "Revision");
            SmartColumn type = new SmartColumn(this, PCResources.TypeColumn, 76, "Type");
            SmartColumn workingCopy = new SmartColumn(this, PCResources.WorkingCopyColumn, 76, "WorkingCopy");

            Columns.AddRange(new ColumnHeader[]
            {
                path,
                project,
                change,
                fullPath
            });

            modified.Sorter = new SortWrapper(
                delegate(PendingCommitItem x, PendingCommitItem y)
                {
                    return x.PendingChange.SvnItem.Modified.CompareTo(y.PendingChange.SvnItem.Modified);
                });

            revision.Sorter = new SortWrapper(
                delegate(PendingCommitItem x, PendingCommitItem y)
                {
                    long? xRev, yRev;
                    xRev = x.PendingChange.Revision;
                    yRev = y.PendingChange.Revision;

                    if (xRev.HasValue && yRev.HasValue)
                        return xRev.Value.CompareTo(yRev.Value);
                    else if (!xRev.HasValue)
                        return yRev.HasValue ? 1 : 0;
                    else
                        return -1;
                });

            change.Groupable = true;
            changeList.Groupable = true;
            folder.Groupable = true;
            locked.Groupable = true;
            project.Groupable = true;
            type.Groupable = true;
            workingCopy.Groupable = true;

            path.Hideable = false;

            AllColumns.Add(change);
            AllColumns.Add(changeList);
            AllColumns.Add(folder);
            AllColumns.Add(fullPath);
            AllColumns.Add(locked);
            AllColumns.Add(modified);
            AllColumns.Add(name);
            AllColumns.Add(path);
            AllColumns.Add(project);
            AllColumns.Add(revision);
            AllColumns.Add(type);
            AllColumns.Add(workingCopy);

            SortColumns.Add(path);
            GroupColumns.Add(changeList);

            FinalSortColumn = path;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            HookCommands();
        }

        bool _hooked;
        public void HookCommands()
        {
            if (_hooked)
                return;

            if (Context != null)
            {
                _hooked = true;
                VSCommandHandler.Install(Context, this,
                    new CommandID(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.SelectAll),
                    OnSelectAll);
            }
        }

        void OnSelectAll(object sender, CommandEventArgs e)
        {
            SelectAllItems();
        }

        bool _openPCOnDoubleClick;

        /// <summary>
        /// Gets or Sets the flag to open pending changes when double-clicked.
        /// </summary>
        [DefaultValue(false)]
        public bool OpenPendingChangeOnDoubleClick
        {
            get { return _openPCOnDoubleClick; }
            set { _openPCOnDoubleClick = value; }
        }

        bool IPendingChangeSource.HasPendingChanges
        {
            get { return CheckedIndices.Count > 0; }
        }

        IEnumerable<PendingChange> IPendingChangeSource.PendingChanges
        {
            get
            {
                List<ListViewItem> list = new List<ListViewItem>();
                foreach (PendingCommitItem pi in CheckedItems)
                {
                    list.Add(pi);
                }

                IComparer<ListViewItem> sorter = ListViewItemSorter as IComparer<ListViewItem>;

                if (sorter != null)
                    list.Sort(sorter);

                foreach (PendingCommitItem pi in list)
                    yield return pi.PendingChange;
            }
        }

        IAnkhServiceProvider _context;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;
            }
        }

        SelectionItemMap _map;
        internal override SelectionItemMap SelectionMap
        {
            get { return _map ?? (_map = SelectionItemMap.Create(this)); }
        }

        protected override string GetCanonicalName(PendingCommitItem item)
        {
            return item.FullPath;
        }

        protected override void OnRetrieveSelection(ListViewWithSelection<PendingCommitItem>.RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = e.Item.PendingChange;
        }

        public override void OnShowContextMenu(MouseEventArgs e)
        {
            base.OnShowContextMenu(e);

            Point p = e.Location;
            bool showSort = false;
            if (p != new Point(-1, -1))
            {
                // Mouse context menu
                if (PointToClient(p).Y < HeaderHeight)
                    showSort = true;
            }
            else
            {
                ListViewItem fi = FocusedItem;

                if (fi != null)
                    p = PointToScreen(fi.Position);
            }

            IAnkhCommandService mcs = Context.GetService<IAnkhCommandService>();
            if (mcs != null)
            {
                if (showSort)
                    mcs.ShowContextMenu(AnkhCommandMenu.PendingCommitsHeaderContextMenu, p);
                else
                    mcs.ShowContextMenu(AnkhCommandMenu.PendingCommitsContextMenu, p);
            }
        }

        IVsUIShell _shell;
        protected override void OnItemChecked(ItemCheckedEventArgs e)
        {
            base.OnItemChecked(e);

            if (_shell == null)
            {

                IAnkhServiceProvider sps = SelectionPublishServiceProvider;
                if (sps != null)
                {
                    _shell = sps.GetService<IVsUIShell>(typeof(SVsUIShell));
                }
            }
            if (_shell != null)
                _shell.UpdateCommandUI(0); // Make sure the toolbar is updated on check actions
        }

        protected override bool IsPartOfSelectAll(ListViewItem i)
        {
            PendingCommitItem pci = i as PendingCommitItem;

            return pci != null &&
                !PendingChange.IsIgnoreOnCommitChangeList(pci.PendingChange.ChangeList);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (OpenPendingChangeOnDoubleClick)
            {
                ListViewHitTestInfo info = HitTest(e.X, e.Y);

                if (info == null || info.Location == ListViewHitTestLocations.None)
                    return;

                if (info.Location == ListViewHitTestLocations.StateImage)
                    return; // Just check the item

                if (CommandService != null)
                    CommandService.ExecCommand(Config.PCDoubleClickShowsChanges
                        ? AnkhCommand.ItemShowChanges : AnkhCommand.ItemOpenVisualStudio, true);
            }
        }

        IAnkhConfigurationService _configurationService;
        protected IAnkhConfigurationService ConfigurationService
        {
            get { return _configurationService ?? (_configurationService = Context.GetService<IAnkhConfigurationService>()); }
        }

        AnkhConfig Config
        {
            get { return ConfigurationService.Instance; }
        }

        IAnkhCommandService CommandService
        {
            get { return Context == null ? null : Context.GetService<IAnkhCommandService>(); }
        }

        public override void OnThemeChange(IAnkhServiceProvider sender, CancelEventArgs e)
        {
            IAnkhCommandStates states;

            if (!VSVersion.SupportsTheming
                || null == (states = sender.GetService<IAnkhCommandStates>())
                || !states.ThemeLight)
            {
                e.Cancel = true; // Don't ask VS to theme the header
                base.OnThemeChange(sender, e); /* Recreate handle while keeping state lists valid */

                ForeColor = Parent.ForeColor;
                BackColor = Parent.BackColor;

                // Re-enable after undoing theming
                ShowSelectAllCheckBox = true;

                return;
            }

            base.OnThemeChange(sender, e);
        }
    }
}
