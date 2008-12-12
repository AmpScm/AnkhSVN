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
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Ankh.UI.Services;
using Ankh.Ids;
using Microsoft.VisualStudio.Shell;
using Ankh.Scc;
using Ankh.UI.VSSelectionControls;
using Ankh.WorkingCopyExplorer;
using SharpSvn;

namespace Ankh.UI.WorkingCopyExplorer
{
    public partial class WorkingCopyExplorerControl : AnkhToolWindowControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkingCopyExplorerControl"/> class.
        /// </summary>
        public WorkingCopyExplorerControl()
        {
            this.InitializeComponent();

            this.folderTree.SelectedItemChanged += new EventHandler(treeView_SelectedItemChanged);
            this.fileList.CurrentDirectoryChanged += new EventHandler(listView_CurrentDirectoryChanged);
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

            folderTree.SelectionPublishServiceProvider = Context;
            fileList.SelectionPublishServiceProvider = Context;

            fileList.StateImageList = Context.GetService<IStatusImageMapper>().StatusImageList;
        }

        public void AddRoot(IFileSystemItem root)
        {
            this.folderTree.AddRoot(root);
        }

        public void RemoveRoot(IFileSystemItem root)
        {
            this.folderTree.RemoveRoot(root);
        }

        public void RefreshItem(IFileSystemItem item)
        {
            throw new System.NotImplementedException();
        }
       
        void treeView_SelectedItemChanged(object sender, EventArgs e)
        {
            IFileSystemItem item = this.folderTree.SelectedItem;
            this.fileList.SetDirectory(item);
        }

        void listView_CurrentDirectoryChanged(object sender, EventArgs e)
        {
            this.folderTree.SelectedItemChanged -= new EventHandler(this.treeView_SelectedItemChanged);
            try
            {
                this.folderTree.SelectedItem = this.fileList.CurrentDirectory;
            }
            finally
            {
                this.folderTree.SelectedItemChanged += new EventHandler(this.treeView_SelectedItemChanged);
            }
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
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SvnItem item = StatusCache[path];

            if(item == null)
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
    }
}
