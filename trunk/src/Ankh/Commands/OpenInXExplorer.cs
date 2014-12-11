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
using SharpSvn;
using System.Runtime.InteropServices;

namespace Ankh.Commands
{
    [SvnCommand(AnkhCommand.ItemSelectInFileExplorer)]
    [SvnCommand(AnkhCommand.ItemSelectInRepositoryExplorer)]
    [SvnCommand(AnkhCommand.ItemSelectInSolutionExplorer)]
    [SvnCommand(AnkhCommand.ItemSelectInWorkingCopyExplorer)]
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
                enable = node.Uri != null;
            else if (e.Command == AnkhCommand.ItemSelectInWorkingCopyExplorer || e.Command == AnkhCommand.ItemSelectInFileExplorer)
                enable = node.Exists;
            else if (e.Command == AnkhCommand.ItemSelectInSolutionExplorer)
            {
                if (e.State.SolutionExplorerActive)
                    enable = false;
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
                    if (node == null || node.Uri == null)
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
                case AnkhCommand.ItemSelectInFileExplorer:
                    if (node == null || !node.Exists)
                        return;

                    SelectInFileExplorer(node.FullPath);
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
                        if (project != null && VSErr.Succeeded(project.IsDocumentInProject(node.FullPath, out found, prio, out id)) && found != 0)
                        {
                            hierWindow.ExpandItem(project as IVsUIHierarchy, id, EXPANDFLAGS.EXPF_SelectItem);
                        }
                        else if (string.Equals(node.FullPath, e.Selection.SolutionFilename, StringComparison.OrdinalIgnoreCase))
                            hierWindow.ExpandItem(e.GetService<IVsUIHierarchy>(typeof(SVsSolution)), VSItemId.Root, EXPANDFLAGS.EXPF_SelectItem);

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

        private void SelectInFileExplorer(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath");

            fullPath = SvnTools.GetNormalizedFullPath(fullPath);

            IntPtr pidlList = NativeMethods.ILCreateFromPathW(fullPath);
            if (pidlList != IntPtr.Zero)
                try
                {
                    // Open parent folder and select item
                    Marshal.ThrowExceptionForHR(NativeMethods.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
                }
                finally
                {
                    NativeMethods.ILFree(pidlList);
                }
        }

        static class NativeMethods
        {

            [DllImport("shell32.dll", ExactSpelling=true)]
            public static extern void ILFree(IntPtr pidlList);

            [DllImport("shell32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
            public static extern IntPtr ILCreateFromPathW(string pszPath);

            [DllImport("shell32.dll", ExactSpelling=true)]
            public static extern int SHOpenFolderAndSelectItems(IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);
        }
    }
}
