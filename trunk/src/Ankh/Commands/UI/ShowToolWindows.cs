// $Id$
//
// Copyright 2005-2009 The AnkhSVN Project
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

using Ankh.UI;
using Ankh.UI.RepositoryExplorer;
using Ankh.VS;

namespace Ankh.Commands.UI
{
    /// <summary>
    /// Command implementation of the show toolwindow commands
    /// </summary>
    [SccCommand(AnkhCommand.ShowPendingChanges)]
    [SvnCommand(AnkhCommand.ShowWorkingCopyExplorer)]
    [SccCommand(AnkhCommand.ShowSubversionInfo)]
    [SvnCommand(AnkhCommand.ShowRepositoryExplorer, AlwaysAvailable=true)]
    class ShowToolWindows : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IAnkhPackage package = e.Context.GetService<IAnkhPackage>();

            AnkhToolWindow toolWindow;
            switch (e.Command)
            {
                case AnkhCommand.ShowPendingChanges:
                    toolWindow = e.State.GitSccProviderActive ? AnkhToolWindow.GitPendingChanges : AnkhToolWindow.PendingChanges;
                    break;
                case AnkhCommand.ShowWorkingCopyExplorer:
                    toolWindow = AnkhToolWindow.WorkingCopyExplorer;
                    break;
                case AnkhCommand.ShowRepositoryExplorer:
                    toolWindow = AnkhToolWindow.RepositoryExplorer;
                    break;
                case AnkhCommand.ShowSubversionInfo:
                    toolWindow = AnkhToolWindow.SvnInfo;
                    break;
                default:
                    return;
            }

            package.ShowToolWindow(toolWindow);

            if (e.Command == AnkhCommand.ShowRepositoryExplorer)
            {
                IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();

                if (ss.ProjectRootUri != null)
                {
                    RepositoryExplorerControl ctrl = e.Selection.ActiveDialogOrFrameControl as RepositoryExplorerControl;

                    if (ctrl != null)
                        ctrl.AddRoot(ss.ProjectRootUri);
                }
            }
        }
    }
}
