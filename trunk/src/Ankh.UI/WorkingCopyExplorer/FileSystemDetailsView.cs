// $Id$
//
// Copyright 2006-2009 The AnkhSVN Project
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
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using Ankh.Scc;
using System.IO;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.UI.WorkingCopyExplorer.Nodes;

namespace Ankh.UI.WorkingCopyExplorer
{
    class FileSystemDetailsView : ListViewWithSelection<FileSystemListViewItem>
    {
        public FileSystemDetailsView()
        {
            View = View.Details;
            HideSelection = false;
            FullRowSelect = true;
            AllowColumnReorder = true;
        }

        IAnkhServiceProvider _context;
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                if (_context != value)
                {
                    _context = value;

                    OnContextChanged(EventArgs.Empty);
                }
            }
        }

        bool _initialized;
        void TryInitialize()
        {
            if (!_initialized)
            {
                _initialized = true;
                InitializeCharacterWidth();
                InitializeColumns();
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (IsHandleCreated)
            {
                TryInitialize();
            }
        }

        IFileIconMapper _mapper;
        IStatusImageMapper _statusMapper;

        internal IFileIconMapper IconMapper
        {
            get { return _mapper ?? (_mapper = Context.GetService<IFileIconMapper>()); }
        }

        internal IStatusImageMapper StatusMapper
        {
            get { return _statusMapper ?? (_statusMapper = Context.GetService<IStatusImageMapper>()); }
        }

        protected virtual void OnContextChanged(EventArgs e)
        {
            if (SmallImageList == null)
                SmallImageList = IconMapper.ImageList;

            if(StateImageList == null)
                StateImageList = StatusMapper.StatusImageList;

            SelectionPublishServiceProvider = Context;
        }

        public void SetDirectory(WCTreeNode directory)
        {
            TryInitialize();

            AddChildren(directory);
        }



        public WCTreeNode CurrentDirectory
        {
            get { return this.currentDirectory; }
        }

        public System.Drawing.Point GetSelectionPoint()
        {
            if (this.SelectedItems.Count > 0)
            {
                ListViewItem item = this.SelectedItems[0];
                int offset = item.Bounds.Height / 3;
                return this.PointToScreen(new Point(item.Bounds.X + offset + this.StateImageList.ImageSize.Width,
                    item.Bounds.Y + offset));
            }
            else
            {
                return Point.Empty;
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            ListViewHitTestInfo ht = HitTest(PointToClient(MousePosition));

            FileSystemListViewItem li = ht.Item as FileSystemListViewItem;

            if (ht.Location == ListViewHitTestLocations.None || li == null)
                return;

            if (!li.Selected)
            {
                SelectedIndices.Clear();
                li.Selected = true;
            }

            Context.GetService<IAnkhCommandService>().PostExecCommand(AnkhCommand.ExplorerOpen);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Enter means open if there's only one selected item
            if (e.KeyCode == Keys.Enter && this.SelectedItems.Count > 0)
            {
                Context.GetService<IAnkhCommandService>().PostExecCommand(AnkhCommand.ExplorerOpen);
            }
        }

        private void AddChildren(WCTreeNode directory)
        {
            this.currentDirectory = directory;

            this.BeginUpdate();
            try
            {
                this.Items.Clear();

                foreach (WCTreeNode item in directory.GetChildren())
                {
                    WCFileSystemNode fsNode = item as WCFileSystemNode;

                    //SvnItem svnItem = item.SvnItem;
                    if (fsNode != null)
                    {
                        if (fsNode.SvnItem.IsDirectory)
                        {
                            bool canRead;

                            foreach (Ankh.Scc.SccFileSystemNode node in Ankh.Scc.SccFileSystemNode.GetDirectoryNodes(fsNode.SvnItem.FullPath, out canRead))
                            {
                                canRead = true;
                                break;
                            }

                            if (!canRead)
                                continue;
                        }
                        FileSystemListViewItem lvi = new FileSystemListViewItem(this, fsNode.SvnItem);

                        Items.Add(lvi);

                        lvi.Tag = item;
                    }
                    else
                        continue; // TODO: Implement this case
                    //    fif = new FileInfo(svnItem.FullPath);

                    //FileSystemListViewItem lvi = new FileSystemListViewItem(this, item.SvnItem, fif);

                    //Items.Add(lvi);

                    //lvi.Tag = item;

                    // we need to know when this item changes
                }

                if (Items.Count > 0 && _nameColumn.DisplayIndex >= 0)
                    _nameColumn.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            finally
            {
                this.EndUpdate();
            }
        }

        SmartColumn _nameColumn;
        private void InitializeColumns()
        {
            AllColumns.Clear();
            SortColumns.Clear();
            this.Columns.Clear();
            _nameColumn = new SmartColumn(this, "&Name", characterWidth * NameColumnNumberOfCharacters);
            SmartColumn modified = new SmartColumn(this, "&Modified", characterWidth * 20);
            SmartColumn extension = new SmartColumn(this, "&Type", characterWidth * 7);
            SmartColumn contStatus = new SmartColumn(this, "&Content Status", characterWidth * 15);
            SmartColumn propStatus = new SmartColumn(this, "&Property Status", characterWidth * 15);
            SmartColumn locked = new SmartColumn(this, "&Locked", characterWidth * 8);
            SmartColumn revision = new SmartColumn(this, "&Revision", characterWidth * 8);
            SmartColumn lastChangeTime = new SmartColumn(this, "Last C&hange", characterWidth * 20);
            SmartColumn lastRev = new SmartColumn(this, "Last Re&vision", characterWidth * 8);
            SmartColumn lastAuthor = new SmartColumn(this, "Last &Author", characterWidth * 8);
            SmartColumn isCopied = new SmartColumn(this, "C&opied", characterWidth * 6);
            SmartColumn isConficted = new SmartColumn(this, "Co&nflicted", characterWidth * 6);
            SmartColumn fullPath = new SmartColumn(this, "Fu&ll Path", characterWidth * 60);

            _nameColumn.Sorter = new SortWrapper(
                delegate(FileSystemListViewItem x, FileSystemListViewItem y)
                {
                    if (x.IsDirectory ^ y.IsDirectory)
                        return x.IsDirectory ? -1 : 1;

                    return StringComparer.OrdinalIgnoreCase.Compare(x.Text, y.Text);
                });

            modified.Sorter = new SortWrapper(
                delegate(FileSystemListViewItem x, FileSystemListViewItem y)
                {
                    return x.Modified.CompareTo(y.Modified);
                });

            lastChangeTime.Sorter = new SortWrapper(
                delegate(FileSystemListViewItem x, FileSystemListViewItem y)
                {
                    return x.SvnItem.Status.LastChangeTime.CompareTo(y.SvnItem.Status.LastChangeTime);
                });

            AllColumns.Add(_nameColumn);
            AllColumns.Add(modified);
            AllColumns.Add(extension);
            AllColumns.Add(contStatus);
            AllColumns.Add(propStatus);
            AllColumns.Add(locked);
            AllColumns.Add(revision);
            AllColumns.Add(lastChangeTime);
            AllColumns.Add(lastRev);
            AllColumns.Add(lastAuthor);
            AllColumns.Add(isCopied);
            AllColumns.Add(isConficted);
            AllColumns.Add(fullPath);

            Columns.AddRange(
                new ColumnHeader[]
                {
                    _nameColumn,
                    modified,
                    extension,
                    contStatus,
                    propStatus,
                    locked,
                    revision
                });

            SortColumns.Add(_nameColumn);
            FinalSortColumn = _nameColumn;
            UpdateSortGlyphs();
        }

        private void InitializeCharacterWidth()
        {
            using (Graphics g = this.CreateGraphics())
            {
                string measureString = "Name of Something To Measure";
                this.characterWidth = (int)(g.MeasureString(measureString, this.Font).Width / measureString.Length);
            }
        }

        protected override void OnRetrieveSelection(ListViewWithSelection<FileSystemListViewItem>.RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new SvnItemData(Context, e.Item.SvnItem);
        }

        public override void OnShowContextMenu(MouseEventArgs e)
        {
            base.OnShowContextMenu(e);

            bool isHeaderContext = false;
            Point screen;
            if (e.X == -1 && e.Y == -1)
            {
                // Handle keyboard context menu
                if (SelectedItems.Count > 0)
                {
                    screen = PointToScreen(SelectedItems[SelectedItems.Count - 1].Position);
                }
                else
                {
                    isHeaderContext = true;
                    screen = PointToScreen(new Point(0, 0));
                }
            }
            else
            {
                screen = e.Location;
                isHeaderContext = PointToClient(e.Location).Y < HeaderHeight;
            }

            IAnkhCommandService sc = Context.GetService<IAnkhCommandService>();

            AnkhCommandMenu menu;
            if (isHeaderContext)
            {
                Select(); // Must be the active control for the menu to work
                menu = AnkhCommandMenu.ListViewHeader;
            }
            else
                menu = AnkhCommandMenu.WorkingCopyExplorerContextMenu;

            sc.ShowContextMenu(menu, screen);
        }

        protected override void OnResolveItem(ListViewWithSelection<FileSystemListViewItem>.ResolveItemEventArgs e)
        {
            SvnItemData sid = e.SelectionItem as SvnItemData;

            if (sid == null)
                return;

            foreach (FileSystemListViewItem lvi in Items)
            {
                if (lvi.SvnItem == sid.SvnItem)
                    e.Item = lvi;
            }
        }

        protected override string GetCanonicalName(FileSystemListViewItem item)
        {
            if (item != null)
            {
                SvnItem i = item.SvnItem;

                string name = i.FullPath;

                if (i.IsDirectory && !name.EndsWith("\\"))
                    name += "\\"; // VS usualy ends returned folders with '\\'

                return name;
            }

            return base.GetCanonicalName(item);
        }

        private ListViewItem FindListViewItemWithTag(WCTreeNode fileSystemItem)
        {
            foreach (ListViewItem lvi in this.Items)
            {
                if (Object.ReferenceEquals(lvi.Tag, fileSystemItem))
                {
                    return lvi;
                }
            }
            return null;
        }

        private int characterWidth;
        private WCTreeNode currentDirectory;
        private const int NameColumnNumberOfCharacters = 50;
    }
}
