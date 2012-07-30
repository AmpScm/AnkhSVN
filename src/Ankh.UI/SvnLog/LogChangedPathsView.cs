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
using System.Windows.Forms;
using Ankh.UI.VSSelectionControls;
using SharpSvn;
using Ankh.Scc;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using Ankh.Commands;

namespace Ankh.UI.SvnLog
{
    class LogChangedPathsView : ListViewWithSelection<PathListViewItem>
    {
        public LogChangedPathsView()
        {
            Init();
        }

        public LogChangedPathsView(IContainer container)
            : this()
        {

            container.Add(this);
        }

        LogDataSource _logSource;
        public LogDataSource LogSource
        {
            get { return _logSource; }
            set { _logSource = value; }
        }

        void Init()
        {
            SmartColumn action = new SmartColumn(this, "&Action", 60);
            SmartColumn path = new SmartColumn(this, "&Path", 342);
            SmartColumn copy = new SmartColumn(this, "&Copy", 60);
            SmartColumn copyRev = new SmartColumn(this, "Copy &Revision", 60);

            AllColumns.Add(action);
            AllColumns.Add(path);
            AllColumns.Add(copy);
            AllColumns.Add(copyRev);

            Columns.AddRange(
                new ColumnHeader[]
                {
                    action,
                    path,
                    copy,
                    copyRev
                });

            SortColumns.Add(path);
            FinalSortColumn = path;
        }

        protected override void OnRetrieveSelection(RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new PathItem(e.Item);
            base.OnRetrieveSelection(e);
        }

        protected override void OnResolveItem(ResolveItemEventArgs e)
        {
            e.Item = ((PathItem)e.SelectionItem).ListViewItem;
            base.OnResolveItem(e);
        }

        IAnkhServiceProvider _context;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;
                SelectionPublishServiceProvider = value;
            }
        }

        #region ICurrentItemDestination<ISvnLogItem> Members
        ICurrentItemSource<ISvnLogItem> itemSource;
        public ICurrentItemSource<ISvnLogItem> ItemSource
        {
            get { return itemSource; }
            set
            {
                if (itemSource != null)
                {
                    itemSource.FocusChanged -= new EventHandler(FocusChanged);
                }
                itemSource = value;
                if (itemSource != null)
                {
                    itemSource.FocusChanged += new EventHandler(FocusChanged);
                }

            }
        }

        #endregion

        void FocusChanged(object sender, EventArgs e)
        {
            Items.Clear();

            ISvnLogItem item = ItemSource.FocusedItem;

            if (item != null && item.ChangedPaths != null)
            {
                IAnkhCommandStates states = null;

                if (Context != null)
                    states = Context.GetService<IAnkhCommandStates>();

                Color[] colorInfo = null;

                if (!SystemInformation.HighContrast &&
                    (states != null && (!states.ThemeDefined || states.ThemeLight)))
                {
                    colorInfo = new Color[] { Color.Gray, Color.FromArgb(100, 0, 100), Color.DarkRed, Color.DarkBlue };
                }

                List<PathListViewItem> paths = new List<PathListViewItem>();

                List<string> origins = new List<string>();
                foreach (SvnOrigin o in LogSource.Targets)
                {
                    string origin = SvnTools.UriPartToPath(o.RepositoryRoot.MakeRelativeUri(o.Uri).ToString()).Replace('\\', '/');
                    if (origin.Length == 0 || origin[0] != '/')
                        origin = "/" + origin;

                    origins.Add(origin.TrimEnd('/'));
                }

                foreach (SvnChangeItem i in item.ChangedPaths)
                {
                    bool hasFocus = (colorInfo != null) && HasFocus(origins, i.Path);

                    paths.Add(new PathListViewItem(this, item, i, item.RepositoryRoot, hasFocus, colorInfo));
                }

                Items.AddRange(paths.ToArray());
            }
        }

        static bool HasFocus(IEnumerable<string> originPaths, string itemPath)
        {
            foreach (string origin in originPaths)
            {
                if (!itemPath.StartsWith(origin))
                    continue;

                int n = itemPath.Length - origin.Length;

                if (n == 0)
                    return true;

                if (n > 0)
                {
                    if (itemPath[origin.Length] == '/')
                        return true;
                }
            }

            return false;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            Point mp = PointToClient(MousePosition);
            ListViewHitTestInfo info = HitTest(mp);
            PathListViewItem lvi = info.Item as PathListViewItem;
            if (lvi != null && Context != null)
            {
                IAnkhCommandService cmdSvc = Context.GetService<IAnkhCommandService>();
                cmdSvc.PostExecCommand(AnkhCommand.LogShowChanges);
            }
        }

        public override void OnShowContextMenu(MouseEventArgs e)
        {
            base.OnShowContextMenu(e);

            if (Context == null)
                return;

            Point screen;
            bool isHeaderContextMenu = false;

            if (e.X == -1 && e.Y == -1)
            {
                if (SelectedItems.Count > 0)
                {
                    screen = PointToScreen(SelectedItems[SelectedItems.Count - 1].Position);
                }
                else
                {
                    screen = PointToScreen(new Point(1, 1));
                    isHeaderContextMenu = true;
                }
            }
            else
            {
                isHeaderContextMenu = PointToClient(e.Location).Y < HeaderHeight;
                screen = e.Location;
            }

            IAnkhCommandService cs = Context.GetService<IAnkhCommandService>();
            cs.ShowContextMenu(isHeaderContextMenu ? AnkhCommandMenu.ListViewHeader : AnkhCommandMenu.LogChangedPathsContextMenu, screen);
        }
    }
}
