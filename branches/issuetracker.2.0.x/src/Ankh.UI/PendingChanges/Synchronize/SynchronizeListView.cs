// $Id$
//
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
using System.Text;
using System.Windows.Forms;
using Ankh.UI.VSSelectionControls;
using System.Drawing;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.VS;

namespace Ankh.UI.PendingChanges.Synchronize
{    
    class SynchronizeListView : ListViewWithSelection<SynchronizeListItem>
    {
        IAnkhServiceProvider _context;

        public SynchronizeListView()
        {
            Initialize();
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set 
            { 
                _context = value; 
                SelectionPublishServiceProvider = value;

                if (value != null && SmallImageList == null)
                {
                    IFileIconMapper mapper = value.GetService<IFileIconMapper>();
                    SmallImageList = mapper.ImageList;
                    StateImageList = mapper.StateImageList;
                }
            }
        }

        protected override void OnResolveItem(ListViewWithSelection<SynchronizeListItem>.ResolveItemEventArgs e)
        {
            e.Item = ((SynchronizeItem)e.SelectionItem).ListItem;
        }

        protected override void OnRetrieveSelection(ListViewWithSelection<SynchronizeListItem>.RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new SynchronizeItem(Context, e.Item);
        }

        void Initialize()
        {
            SmartColumn path = new SmartColumn(this, PCStrings.PathColumn, 288);
            SmartColumn project = new SmartColumn(this, PCStrings.ProjectColumn, 76);
            SmartColumn localChange = new SmartColumn(this, PCStrings.LocalChangeColumn, 102);
            SmartColumn remoteChange = new SmartColumn(this, PCStrings.RemoteChangeColumn, 102);
            SmartColumn fullPath = new SmartColumn(this, PCStrings.FullPathColumn, 327);

            SmartColumn changeList = new SmartColumn(this, PCStrings.ChangeListColumn, 76);
            SmartColumn folder = new SmartColumn(this, PCStrings.FolderColumn, 196);
            SmartColumn locked = new SmartColumn(this, PCStrings.LockedColumn, 38);
            SmartColumn modified = new SmartColumn(this, PCStrings.ModifiedColumn, 76);
            SmartColumn name = new SmartColumn(this, PCStrings.NameColumn, 76);
            SmartColumn type = new SmartColumn(this, PCStrings.TypeColumn, 76);
            SmartColumn workingCopy = new SmartColumn(this, PCStrings.WorkingCopyColumn, 76);

            Columns.AddRange(new ColumnHeader[]
            {
                path,
                project,
                localChange,
                remoteChange,
                fullPath
            });

            localChange.Groupable = true;
            remoteChange.Groupable = true;

            changeList.Groupable = true;
            folder.Groupable = true;
            locked.Groupable = true;
            project.Groupable = true;
            type.Groupable = true;
            workingCopy.Groupable = true;

            path.Hideable = false;

            modified.Sorter = new SortWrapper(
                delegate(SynchronizeListItem x, SynchronizeListItem y)
                {
                    return x.SvnItem.Modified.CompareTo(y.SvnItem.Modified);
                });
            
            AllColumns.Add(changeList);
            AllColumns.Add(folder);
            AllColumns.Add(fullPath);
            AllColumns.Add(localChange);            
            AllColumns.Add(locked);
            AllColumns.Add(modified);
            AllColumns.Add(name);
            AllColumns.Add(path);
            AllColumns.Add(project);
            AllColumns.Add(remoteChange);
            AllColumns.Add(type);
            AllColumns.Add(workingCopy);

            SortColumns.Add(path);
            GroupColumns.Add(changeList);

            FinalSortColumn = path;
        }

        protected override string GetCanonicalName(SynchronizeListItem item)
        {
            return item.SvnItem.FullPath;
        }

        public override void OnShowContextMenu(MouseEventArgs e)
        {
            base.OnShowContextMenu(e);

            Point p = e.Location;
            bool showSort = false;
            if (p != new Point(-1, -1))
            {
                // Mouse context menu
                Point clP = PointToClient(e.Location);
                ListViewHitTestInfo hti = HitTest(clP);

                showSort = (hti.Item == null || hti.Location == ListViewHitTestLocations.None || hti.Location == ListViewHitTestLocations.AboveClientArea);
                if (!showSort && hti.Item != null)
                {
                    Rectangle r = hti.Item.GetBounds(ItemBoundsPortion.Entire);

                    if (!r.Contains(clP))
                        showSort = true;
                }
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
