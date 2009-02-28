// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using Ankh.VS;
using System.IO;

namespace Ankh.UI.WorkingCopyExplorer.Nodes
{
    class WCSolutionNode : WCFileSystemNode
    {
        readonly int _imageIndex;
        public WCSolutionNode(IAnkhServiceProvider context, SvnItem item)
            : base(context, null, item)
        {
            _imageIndex = context.GetService<IFileIconMapper>().GetIconForExtension(".sln");
        }

        public override string Title
        {
            get {return  Context.GetService<IAnkhSolutionSettings>().SolutionFilename; }
        }

        public override IEnumerable<WCTreeNode> GetChildren()
        {
            IAnkhProjectLayoutService pls = Context.GetService<IAnkhProjectLayoutService>();
            foreach (SvnItem item in pls.GetUpdateRoots(null))
            {
                yield return new WCDirectoryNode(Context, this, item);
            }
        }

        public override System.IO.FileSystemInfo FileInfo
        {
            get { return new DirectoryInfo(SvnItem.FullPath); }
        }

        public override bool IsContainer
        {
            get
            {
                return true;
            }
        }

        public override void GetResources(System.Collections.ObjectModel.Collection<SvnItem> list, bool getChildItems, Predicate<SvnItem> filter)
        {
//            throw new NotImplementedException();
        }

        protected override void RefreshCore(bool rescan)
        {
//            throw new NotImplementedException();
        }

        public override int ImageIndex
        {
            get { return _imageIndex; }
        }
    }
}
