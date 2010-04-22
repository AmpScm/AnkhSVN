// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemSelectInWorkingCopyExplorer)]
    [Command(AnkhCommand.ItemSelectInRepositoryExplorer)]
    [Command(AnkhCommand.ItemSelectInSolutionExplorer)]
    class OpenInXExplorer : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            SvnItem node = EnumTools.GetSingle(e.Selection.GetSelectedSvnItems(false));

            if (node == null && e.Selection.IsSingleNodeSelection)
                node = EnumTools.GetFirst(e.Selection.GetSelectedSvnItems(false));

            bool enable = true;
            if (node == null)
                enable = false;
            else if (e.Command == AnkhCommand.ItemSelectInRepositoryExplorer)
                enable = node.Status.Uri != null;
            else if (e.Command == AnkhCommand.ItemSelectInWorkingCopyExplorer)
                enable = node.Exists;
            else if (e.Command == AnkhCommand.ItemSelectInSolutionExplorer)
            {
                if (e.Selection.IsSolutionExplorerSelection)
                    e.Visible = enable = false;
                else if (!node.InSolution)
                    enable = false;
            }

            if (!enable)
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            SvnItem node = EnumTools.GetFirst(e.Selection.GetSelectedSvnItems(false));

            IAnkhCommandService cmd = e.GetService<IAnkhCommandService>();
            switch (e.Command)
            {
                case AnkhCommand.ItemSelectInRepositoryExplorer:
                    if (node == null || node.Status.Uri == null)
                        return;

                    if (cmd != null)
                        cmd.DirectlyExecCommand(AnkhCommand.RepositoryBrowse, node.FullPath);
                    break;
                case AnkhCommand.ItemSelectInWorkingCopyExplorer:
                    if (node == null || !node.Exists)
                        return;

                    if (cmd != null)
                        cmd.DirectlyExecCommand(AnkhCommand.WorkingCopyBrowse, node.FullPath);
                    break;
                case AnkhCommand.ItemSelectInSolutionExplorer:
                    if (node == null)
                        return;

                    IVsUIHierarchyWindow hierWindow = VsShellUtilities.GetUIHierarchyWindow(e.Context, new Guid(ToolWindowGuids80.SolutionExplorer));

                    IVsProject project = VsShellUtilities.GetProject(e.Context, node.FullPath) as IVsProject;

                    if (hierWindow != null)
                    {
                        int found;
                        uint id;
                        VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];
                        if (project != null && ErrorHandler.Succeeded(project.IsDocumentInProject(node.FullPath, out found, prio, out id)) && found != 0)
                        {
                            hierWindow.ExpandItem(project as IVsUIHierarchy, id, EXPANDFLAGS.EXPF_SelectItem);
                        }
                        else if (string.Equals(node.FullPath, e.Selection.SolutionFilename, StringComparison.OrdinalIgnoreCase))
                            hierWindow.ExpandItem(e.GetService<IVsUIHierarchy>(typeof(SVsSolution)), VSConstants.VSITEMID_ROOT, EXPANDFLAGS.EXPF_SelectItem);

                        // Now try to activate the solution explorer
                        IVsWindowFrame solutionExplorer;
                        Guid solutionExplorerGuid = new Guid(ToolWindowGuids80.SolutionExplorer);
                        IVsUIShell shell = e.GetService<IVsUIShell>(typeof(SVsUIShell));

                        if (shell != null)
                        {
                            shell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref solutionExplorerGuid, out solutionExplorer);

                            if (solutionExplorer != null)
                                solutionExplorer.Show();
                        }

                    }
                    break;
            }
        }
    }
}
