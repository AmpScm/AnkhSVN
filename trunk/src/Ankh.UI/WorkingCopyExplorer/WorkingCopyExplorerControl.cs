// $Id$
//
// Copyright 2006-2008 The AnkhSVN Project
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
using Ankh.Ids;
using Ankh.Scc;
using SharpSvn;
using Ankh.UI.WorkingCopyExplorer.Nodes;
using System.ComponentModel.Design;
using Ankh.Commands;
using Ankh.VS;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;

namespace Ankh.UI.WorkingCopyExplorer
{
    public partial class WorkingCopyExplorerControl : AnkhToolWindowControl
    {
        private FileSystemNode[] _selection = new FileSystemNode[] { };

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkingCopyExplorerControl"/> class.
        /// </summary>
        public WorkingCopyExplorerControl()
        {
            this.InitializeComponent();

            this.folderTree.SelectedItemChanged += new EventHandler(treeView_SelectedItemChanged);
        }

        /// <summary>
        /// Called when the frame is created
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            ToolWindowHost.CommandContext = AnkhId.SccExplorerContextGuid;
            ToolWindowHost.KeyboardContext = AnkhId.SccExplorerContextGuid;

            folderTree.Context = Context;
            fileList.Context = Context;            

            VSCommandHandler.Install(Context, this, AnkhCommand.ExplorerOpen, OnOpen, OnUpdateOpen);
            VSCommandHandler.Install(Context, this, AnkhCommand.ExplorerUp, OnUp, OnUpdateUp);
        }

        internal void AddRoot(FileSystemNode root)
        {
            this.folderTree.AddRoot(root);
        }

        internal void RemoveRoot(FileSystemNode root)
        {
            this.folderTree.RemoveRoot(root);
        }

        internal void RefreshItem(FileSystemNode item)
        {
            throw new System.NotImplementedException();
        }

        void treeView_SelectedItemChanged(object sender, EventArgs e)
        {
            FileSystemNode item = this.folderTree.SelectedItem;
            this.fileList.SetDirectory(item);
        } 

        public bool IsWcRootSelected()
        {
            return false;
        }

        public void RemoveRoot()
        {
            //
        }

        IFileStatusCache _cache;
        protected internal IFileStatusCache StatusCache
        {
            get { return _cache ?? (_cache = Context.GetService<IFileStatusCache>()); }
        }

        public void BrowsePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SvnItem item = StatusCache[path];

            if (item == null)
                return;

            SvnWorkingCopy wc = item.WorkingCopy;

            string root;
            if (wc != null)
                root = wc.FullPath;
            else
                root = item.FullPath;



            FileSystemRootItem rt = CreateRoot(root);

            AddRoot(rt);
        }

        private FileSystemRootItem CreateRoot(string directory)
        {
            StatusCache.UpdateStatus(directory, SvnDepth.Infinity);
            SvnItem item = StatusCache[directory];
            FileSystemRootItem root = new FileSystemRootItem(this, item);
            return root;
        }

        internal void OpenItem(IAnkhServiceProvider context, string p)
        {
            Ankh.Commands.IAnkhCommandService cmd = context.GetService<Ankh.Commands.IAnkhCommandService>();

            if (cmd != null)
                cmd.ExecCommand(AnkhCommand.ItemOpenVisualStudio, true);
        }

        void OnUpdateUp(object sender, CommandUpdateEventArgs e)
        {
            FileSystemTreeNode tn = folderTree.SelectedNode as FileSystemTreeNode;

            if (tn == null || !(tn.Parent is FileSystemTreeNode))
                e.Enabled = false;
        }

        void OnUp(object sender, CommandEventArgs e)
        {
            FileSystemTreeNode t = folderTree.SelectedNode as FileSystemTreeNode;
            if (t != null && t.Parent != null)
            {
                folderTree.SelectedNode = t.Parent;
            }
        }

        void OnUpdateOpen(object sender, CommandUpdateEventArgs e)
        {
            SvnItem item = EnumTools.GetSingle(e.Selection.GetSelectedSvnItems(false));

            if (item == null)
                e.Enabled = false;
        }

        void OnOpen(object sender, CommandEventArgs e)
        {
            SvnItem item = EnumTools.GetSingle(e.Selection.GetSelectedSvnItems(false));

            if (item.IsDirectory)
            {
                BrowsePath(item.FullPath);
                return;
            }

            AutoOpenCommand(e, item);
        }

        private static void AutoOpenCommand(CommandEventArgs e, SvnItem item)
        {
            IAnkhCommandService svc = e.GetService<IAnkhCommandService>();
            IAnkhSolutionSettings solutionSettings = e.GetService<IAnkhSolutionSettings>();

            if (svc == null || solutionSettings == null)
                return;

            // Ok, we can assume we have a file
            string filename = item.Name;
            string ext = item.Extension;

            if (string.IsNullOrEmpty(ext))
            {
                // No extension -> Open as text
                svc.PostExecCommand(AnkhCommand.ItemOpenTextEditor);
                return;
            }

            foreach (string projectExt in solutionSettings.AllProjectExtensionsFilter.Split(';'))
            {
                if (projectExt.TrimStart('*').Trim().Equals(ext, StringComparison.OrdinalIgnoreCase))
                {
                    // We found a project or solution, use Open from Subversion to create a checkout

                    e.GetService<IAnkhSolutionSettings>().OpenProjectFile(item.FullPath);
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
                        VsShellUtilities.OpenDocument(e.Context, item.FullPath);
                        return;
                    }
                }
            }

            // Ultimate fallback: Just open the file as windows would
            svc.PostExecCommand(AnkhCommand.ItemOpenWindows);
        }

    }
}
