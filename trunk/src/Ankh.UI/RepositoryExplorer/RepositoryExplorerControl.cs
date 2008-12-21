// $Id$
//
// Copyright 2003-2008 The AnkhSVN Project
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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SharpSvn;
using Ankh.UI.Services;
using Ankh.Ids;
using Ankh.UI.RepositoryExplorer;
using Ankh.VS;
using Ankh.UI.VSSelectionControls;
using Ankh.Commands;
using Microsoft.VisualStudio;
using System.ComponentModel.Design;
using Ankh.Scc;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.UI.RepositoryExplorer
{
    /// <summary>
    /// Gives a tree view of the repository based on revision.
    /// </summary>
    public partial class RepositoryExplorerControl : AnkhToolWindowControl
    {
        public RepositoryExplorerControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new Container();
            treeView.RetrieveItems = SvnDirEntryItems.Kind | SvnDirEntryItems.LastAuthor | SvnDirEntryItems.Revision | SvnDirEntryItems.Size | SvnDirEntryItems.Time;
        }

        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            ToolWindowHost.CommandContext = AnkhId.SccExplorerContextGuid;
            ToolWindowHost.KeyboardContext = AnkhId.SccExplorerContextGuid;

            VSCommandHandler.Install(Context, this, new CommandID(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.Delete), OnDelete);
            VSCommandHandler.Install(Context, this, AnkhCommand.RepositoryOpen, OnOpen, OnUpdateOpen);
        }

        void OnDelete(object sender, CommandEventArgs e)
        {
            e.GetService<IAnkhCommandService>().PostExecCommand(AnkhCommand.SvnNodeDelete);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            treeView.Context = Context;
            fileView.Context = Context;
            treeView.SelectionPublishServiceProvider = Context;
            fileView.SelectionPublishServiceProvider = Context;
        }

        /// <summary>
        /// Add a new URL root to the tree.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="node"></param>
        public void AddRoot(Uri uri)
        {
            this.treeView.AddRoot(uri);
        }

        /// <summary>
        /// Remove the root(server) of the node identified by the <paramref name="uri"/>
        /// </summary>
        /// <param name="uri">URI of the tree node</param>
        public void RemoveRootOf(Uri uri)
        {
            this.treeView.RemoveRootOf(uri);
        }

        /// <summary>
        /// Reloads the node identified by the <paramref name="uri"/><br/>
        /// </summary>
        /// <param name="uri">URI of the tree node</param>
        public void Reload(Uri uri)
        {
            this.treeView.Reload(uri);
        }

        /// <summary>
        /// Get the URI of the selected node
        /// </summary>
        public Uri SelectedUri
        {
            get
            {
                RepositoryTreeNode selected = this.treeView.SelectedNode;
                return selected == null ? null : selected.RawUri;
            }
        }

        private void OnTreeViewShowContextMenu(object sender, MouseEventArgs e)
        {
            Point screen = (e.Location != new Point(-1, -1)) ? e.Location : treeView.PointToScreen(new Point(0, 0));
            ToolWindowHost.ShowContextMenu(AnkhCommandMenu.RepositoryExplorerContextMenu, screen.X, screen.Y);
        }

        IFileIconMapper _iconMapper;
        IFileIconMapper IconMapper
        {
            get
            {
                if (_iconMapper == null && Context != null)
                    _iconMapper = Context.GetService<IFileIconMapper>();

                return _iconMapper;
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RefreshFileList();
        }

        void RefreshFileList()
        {
            fileView.Items.Clear();

            RepositoryTreeNode tn = treeView.SelectedNode as RepositoryTreeNode;

            if (tn != null && tn.Origin != null)
            {
                foreach (RepositoryTreeNode sn in tn.Nodes)
                {
                    if (sn.FolderItems.Contains(sn.RawUri))
                    {
                        RepositoryListItem item = new RepositoryListItem(fileView, sn.FolderItems[sn.RawUri], tn.Origin, IconMapper);

                        fileView.Items.Add(item);
                    }
                }
                foreach (SvnListEventArgs ee in tn.FolderItems)
                {
                    if (ee.EntryUri != tn.RawUri)
                    {
                        RepositoryListItem item = new RepositoryListItem(fileView, ee, tn.Origin, IconMapper);

                        fileView.Items.Add(item);
                    }
                }

                if (fileView.Items.Count > 0)
                {
                    SmartColumn fileColumn = fileView.AllColumns[0];

                    if (fileColumn.DisplayIndex >= 0)
                        fileColumn.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                }
            }
        }

        BusyOverlay _bo;
        private void treeView_RetrievingChanged(object sender, EventArgs e)
        {
            if (treeView.Retrieving)
            {
                if (_bo == null)
                    _bo = new BusyOverlay(treeView, AnchorStyles.Top | AnchorStyles.Right);

                _bo.Show();
            }
            else
            {
                if (_bo != null)
                {
                    _bo.Hide();
                    _bo.Dispose();
                    _bo = null;
                }
            }
        }

        private void treeView_SelectedNodeRefresh(object sender, EventArgs e)
        {
            RefreshFileList();
        }

        private void fileView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Context.GetService<IAnkhCommandService>().PostExecCommand(AnkhCommand.RepositoryOpen);            
        }


        void OnUpdateOpen(object sender, CommandUpdateEventArgs e)
        {
            RepositoryExplorerItem item = EnumTools.GetSingle(e.Selection.GetSelection<RepositoryExplorerItem>());

            if (item == null || item.Uri == null)
                e.Enabled = false;
        }

        void OnOpen(object sender, CommandEventArgs e)
        {
            RepositoryExplorerItem item = EnumTools.GetSingle(e.Selection.GetSelection<RepositoryExplorerItem>());

            if (item.Info == null || item.Info.Entry.NodeKind == SvnNodeKind.Directory || item.Origin == null)
            {
                treeView.BrowseTo(item.Uri);
                return;
            }

            IAnkhCommandService svc = e.GetService<IAnkhCommandService>();
            IAnkhSolutionSettings solutionSettings = e.GetService<IAnkhSolutionSettings>();

            if (svc == null || solutionSettings == null)
                return;

            // Ok, we can assume we have a file
            SvnOrigin origin = item.Origin;

            string filename = origin.Target.FileName;
            string ext = Path.GetExtension(filename);

            if (string.IsNullOrEmpty(ext))
            {
                // No extension -> Open as text
                svc.PostExecCommand(AnkhCommand.ViewInVsText);
                return;
            }

            foreach (string projectExt in solutionSettings.AllProjectExtensionsFilter.Split(';'))
            {
                if (projectExt.TrimStart('*').Trim().Equals(ext, StringComparison.OrdinalIgnoreCase))
                {
                    // We found a project or solution, use Open from Subversion to create a checkout

                    svc.PostExecCommand(AnkhCommand.FileFileOpenFromSubversion, origin);
                    return;
                }
            }

            bool odd = false;
            foreach (string block in solutionSettings.OpenFileFilter.Split('|'))
            {
                odd = !odd;
                if (odd)
                    continue;

                foreach (string itemExt in block.Split(';'))
                {
                    if (itemExt.TrimStart('*').Trim().Equals(ext, StringComparison.OrdinalIgnoreCase))
                    {
                        svc.PostExecCommand(AnkhCommand.ViewInVsNet);
                        return;
                    }
                }
            }

            // Ultimate fallback: Just ask the user what to do (don't trust the repository!)
            svc.PostExecCommand(AnkhCommand.ViewInWindowsWith);
        }
    }
}
