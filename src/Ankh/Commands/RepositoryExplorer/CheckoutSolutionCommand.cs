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
using Ankh.UI;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;
using Ankh.UI.RepositoryExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to checkout current solution in Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.CheckoutSolution)]
    class CheckoutSolutionCommand : CommandBase
    {
        

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            /*IExplorersShell shell = e.GetService<IExplorersShell>();
            IContext context = e.GetService<IContext>();
            EnvDTE._DTE dte = e.GetService<EnvDTE._DTE>(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));

            /// first get the parent folder
			using (FolderBrowserDialog browser = new FolderBrowserDialog())
			{
				/// give a chance to the user to bail
				if (browser.ShowDialog() != DialogResult.OK)
					return;

				using(context.StartOperation("Checking out"))
				{
					INode node = shell.RepositoryExplorerService.SelectedNode;
					INode parent = node.Parent;

					CheckoutRunner runner = new CheckoutRunner(browser.SelectedPath, parent.Revision, new Uri(parent.Url));
                    e.GetService<IProgressRunner>().Run(
                        "Checking out solution",
                        runner.Work);

					dte.Solution.Open(Path.Combine(browser.SelectedPath, node.Name));
				}
			}*/
        }
    }
}
