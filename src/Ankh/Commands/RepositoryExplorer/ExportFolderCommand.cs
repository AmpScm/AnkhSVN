// $Id$
//
// Copyright 2004-2008 The AnkhSVN Project
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
using System.IO;
using System.Windows.Forms;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command for exporting a folder
    /// </summary>
    [Command(AnkhCommand.ExportFolder)]
    class ExportFolderCommand : CommandBase
    {
        

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            e.Enabled = false;
            /*IExplorersShell shell = e.GetService<IExplorersShell>();

            if (shell.RepositoryExplorerService.SelectedNode == null ||
                !shell.RepositoryExplorerService.SelectedNode.IsDirectory)
            {
                // BH: Why don't we allow exporting single files?
                e.Enabled = false;
            }*/
        }

        public override void OnExecute(CommandEventArgs e)
        {
            /*IExplorersShell shell = e.GetService<IExplorersShell>();
            IContext context = e.Context.GetService<IContext>();

            /// first get the parent folder
            using (FolderBrowserDialog browser = new FolderBrowserDialog())
            {

                /// give a chance to the user to bail
                if (browser.ShowDialog() != DialogResult.OK)
                    return;

                using (context.StartOperation("Exporting"))
                {
                    INode node = shell.RepositoryExplorerService.SelectedNode;

                    ExportRunner runner = new ExportRunner(browser.SelectedPath, node.Revision, node.Url);
                    e.GetService<IProgressRunner>().Run(
                        "Exporting folder",
                        runner.Work);
                }
            }*/
        }
    }
}
