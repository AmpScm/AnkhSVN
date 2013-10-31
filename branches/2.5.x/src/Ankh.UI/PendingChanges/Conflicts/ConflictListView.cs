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
using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using System.Windows.Forms;
using Ankh.UI.PendingChanges.Commits;
using System.Drawing;
using Ankh.Commands;

namespace Ankh.UI.PendingChanges.Conflicts
{
    class ConflictListView : ListViewWithSelection<ConflictListItem>
    {
        IAnkhServiceProvider _context;

        public ConflictListView()
        {
            Initialize();
        }

        void Initialize()
        {
            SmartColumn path = new SmartColumn(this, PCResources.PathColumn, 288, "Path");
            SmartColumn project = new SmartColumn(this, PCResources.ProjectColumn, 76, "Project");
            SmartColumn conflictType = new SmartColumn(this, PCResources.ConflictTypeColumn, 92, "ConflictType");
            SmartColumn conflictDescription = new SmartColumn(this, PCResources.ConflictDescriptionColumn, 288, "ConflictDescription");

            SmartColumn change = new SmartColumn(this, PCResources.ChangeColumn, 76, "Change");
            SmartColumn fullPath = new SmartColumn(this, PCResources.FullPathColumn, 327, "FullPath");

            SmartColumn changeList = new SmartColumn(this, PCResources.ChangeListColumn, 76, "ChangeList");
            SmartColumn folder = new SmartColumn(this, PCResources.FolderColumn, 196, "Folder");
            SmartColumn locked = new SmartColumn(this, PCResources.LockedColumn, 38, "Locked");
            SmartColumn modified = new SmartColumn(this, PCResources.ModifiedColumn, 76, "Modified");
            SmartColumn name = new SmartColumn(this, PCResources.NameColumn, 76, "Name");
            SmartColumn type = new SmartColumn(this, PCResources.TypeColumn, 76, "Type");
            SmartColumn workingCopy = new SmartColumn(this, PCResources.WorkingCopyColumn, 76, "WorkingCopy");

            Columns.AddRange(new ColumnHeader[]
            {
                path,
                project,
                conflictType,
                conflictDescription,
            });

            modified.Sorter = new SortWrapper(
                delegate(ConflictListItem x, ConflictListItem y)
                {
                    return x.PendingChange.SvnItem.Modified.CompareTo(y.PendingChange.SvnItem.Modified);
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
            AllColumns.Add(conflictType);
            AllColumns.Add(conflictDescription);
            AllColumns.Add(folder);
            AllColumns.Add(fullPath);
            AllColumns.Add(locked);
            AllColumns.Add(modified);
            AllColumns.Add(name);
            AllColumns.Add(path);
            AllColumns.Add(project);
            AllColumns.Add(type);
            AllColumns.Add(workingCopy);

            SortColumns.Add(path);

            FinalSortColumn = path;
        }

        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;
                SelectionPublishServiceProvider = value;
                if (value != null)
                {
                    IFileIconMapper mapper = value.GetService<IFileIconMapper>();
                    SmallImageList = mapper.ImageList;
                }
            }
        }

        protected override string GetCanonicalName(ConflictListItem item)
        {
            return item.PendingChange.FullPath;
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
    }
}
