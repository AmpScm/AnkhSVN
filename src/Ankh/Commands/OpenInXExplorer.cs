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

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemSelectInWorkingCopyExplorer)]
    [Command(AnkhCommand.ItemOpenFolderInRepositoryExplorer)]
    class OpenInXExplorer : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            SvnItem node = EnumTools.GetSingle(e.Selection.GetSelectedSvnItems(false));

            if (node == null && e.Selection.IsSingleNodeSelection)
                node = EnumTools.GetFirst(e.Selection.GetSelectedSvnItems(false));

            if (node == null)
                e.Enabled = false;
            else if (e.Command == AnkhCommand.ItemOpenFolderInRepositoryExplorer)
                e.Enabled = node.Status.Uri != null;
            else if (e.Command == AnkhCommand.ItemSelectInWorkingCopyExplorer)
                e.Enabled = node.Exists;
            else
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            SvnItem node = EnumTools.GetFirst(e.Selection.GetSelectedSvnItems(false));

            IAnkhCommandService cmd = e.GetService<IAnkhCommandService>();
            switch (e.Command)
            {
                case AnkhCommand.ItemOpenFolderInRepositoryExplorer:
                    if (node == null || node.Status.Uri == null)
                        return;

                    if (node.IsFile)
                    {
                        SvnItem parent = node.Parent;

                        if (parent.IsVersioned && parent.Status.Uri != null)
                            node = parent;
                    }

                    if (cmd != null)
                        cmd.DirectlyExecCommand(AnkhCommand.RepositoryBrowse, node.Status.Uri);
                    break;
                case AnkhCommand.ItemSelectInWorkingCopyExplorer:
                    if (node == null || !node.Exists)
                        return;

                    if (cmd != null)
                        cmd.DirectlyExecCommand(AnkhCommand.WorkingCopyBrowse, node.FullPath);
                    break;
            }
        }
    }
}
