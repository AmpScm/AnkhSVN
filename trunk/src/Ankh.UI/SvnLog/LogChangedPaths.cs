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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using Ankh.UI.SvnLog;
using Ankh.UI.Services;
using Ankh.Scc;
using Ankh.Ids;
using Ankh.Commands;
using Ankh.Selection;
using Ankh.Scc.UI;

namespace Ankh.UI
{
    partial class LogChangedPaths : UserControl, ICurrentItemDestination<ISvnLogItem>
    {
        public LogChangedPaths()
        {
            InitializeComponent();
        }

        public LogChangedPaths(IContainer container)
            : this()
        {
            container.Add(this);
        }

        LogDataSource _dataSource;
        public LogDataSource LogSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        IAnkhServiceProvider _context;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;
                changedPaths.SelectionPublishServiceProvider = value;


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
                    itemSource.SelectionChanged -= new EventHandler<CurrentItemEventArgs<ISvnLogItem>>(SelectionChanged);
                    itemSource.FocusChanged -= new EventHandler<CurrentItemEventArgs<ISvnLogItem>>(FocusChanged);
                }
                itemSource = value;
                if (itemSource != null)
                {
                    itemSource.SelectionChanged += new EventHandler<CurrentItemEventArgs<ISvnLogItem>>(SelectionChanged);
                    itemSource.FocusChanged += new EventHandler<CurrentItemEventArgs<ISvnLogItem>>(FocusChanged);
                }

            }
        }

        #endregion



        void SelectionChanged(object sender, CurrentItemEventArgs<ISvnLogItem> e)
        {
        }

        void FocusChanged(object sender, CurrentItemEventArgs<ISvnLogItem> e)
        {
            changedPaths.Items.Clear();

            ISvnLogItem item = e.Source.FocusedItem;

            if (item != null && item.ChangedPaths != null)
            {
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
                    paths.Add(new PathListViewItem(changedPaths, item, i, item.RepositoryRoot, HasFocus(origins, i.Path)));
                }

                changedPaths.Items.AddRange(paths.ToArray());
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

        internal void Reset()
        {
            changedPaths.Items.Clear();
        }



        private void changedPaths_ShowContextMenu(object sender, MouseEventArgs e)
        {
            Point p = MousePosition;

            if (Context != null)
            {
                IAnkhCommandService cs = Context.GetService<IAnkhCommandService>();
                cs.ShowContextMenu(AnkhCommandMenu.LogChangedPathsContextMenu, p.X, p.Y);
            }
        }

        private void changedPaths_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Point mp = changedPaths.PointToClient(MousePosition);
            ListViewHitTestInfo info = changedPaths.HitTest(mp);
            PathListViewItem lvi = info.Item as PathListViewItem;
            if (lvi != null && Context != null)
            {
                IAnkhCommandService cmdSvc = Context.GetService<IAnkhCommandService>();
                cmdSvc.PostExecCommand(AnkhCommand.LogShowChanges);
            }
        }
    }
}
