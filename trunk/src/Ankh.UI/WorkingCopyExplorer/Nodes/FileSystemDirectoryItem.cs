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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.UI.WorkingCopyExplorer.Nodes
{
    internal class FileSystemDirectoryItem : FileSystemNode
    {
        public FileSystemDirectoryItem(WorkingCopyExplorerControl control, SvnItem item)
            : base(control, null, item)
        {
            this.FindChildren();
            this.Refresh(false);
        }

        public override bool IsContainer
        {
            get { return true; }
        }

        public override FileSystemNode[] GetChildren()
        {
            try
            {
                FileSystemNode[] children = new FileSystemNode[this.Children.Count];
                this.Children.CopyTo(children, 0);
                return children;
            }
            catch
            {
                return new FileSystemNode[] { };
            }
        }


        public override void Refresh(bool rescan)
        {
            if (rescan)
            {
                IFileStatusCache cache = Context.GetService<IFileStatusCache>();

                if (cache != null)
                    cache.UpdateStatus(SvnItem.FullPath, SvnDepth.Infinity);

                this.FindChildren();

            }
        }

        private void FindChildren()
        {
            this.Children.Clear();

            if (this.SvnItem.IsVersioned)
            {
                foreach (FileSystemNode item in GetFileSystemItemsForDirectory(this.SvnItem))
                {
                    this.Children.Add(item);
                }
            }
        }

        internal FileSystemNode[] GetFileSystemItemsForDirectory(SvnItem directoryItem)
        {
            SortedList<string, FileSystemNode> items = new SortedList<string, FileSystemNode>(StringComparer.OrdinalIgnoreCase);

            SvnDirectory dir = Control.StatusCache.GetDirectory(directoryItem.FullPath);

            if (dir != null)
            {
                SvnItem dirItem = dir.Directory;

                foreach (SvnItem item in dir)
                {
                    if (items.ContainsKey(item.Name))
                        items.Add(item.Name, FileSystemFileItem.Create(Control, item));
                }
            }

            foreach (string path in Directory.GetFileSystemEntries(directoryItem.FullPath))
            {
                string name = Path.GetFileName(path);
                if (!string.Equals(name, SvnClient.AdministrativeDirectoryName, StringComparison.OrdinalIgnoreCase) &&
                    !items.ContainsKey(name))
                {
                    SvnItem item = Control.StatusCache[path];

                    items.Add(name, FileSystemNode.Create(Control, item));
                }
            }

            return new List<FileSystemNode>(items.Values).ToArray();
        }

        public override int ImageIndex
        {
            get { throw new NotImplementedException(); }
        }
    }
}
