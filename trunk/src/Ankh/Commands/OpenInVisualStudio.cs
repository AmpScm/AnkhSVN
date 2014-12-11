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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.UI;
using Ankh.Scc;

namespace Ankh.Commands
{
    [SvnCommand(AnkhCommand.ItemOpenVisualStudio)]
    [SvnCommand(AnkhCommand.ItemOpenTextEditor)]
    [SvnCommand(AnkhCommand.ItemOpenWindows)]
    class OpenInVisualStudio : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool foundOne = false;
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (!item.Exists)
                    continue;

                if (!item.IsFile)
                {
                    e.Enabled = false;
                    return;
                }
                foundOne = true;
            }

            if (!foundOne)
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            AnkhMessageBox mb = new AnkhMessageBox(e.Context);

            // Cache items to avoid problems when selection changes by opening editor
            List<SvnItem> items = new List<SvnItem>(e.Selection.GetSelectedSvnItems(false));

            foreach (SvnItem item in items)
            {
                if (!item.Exists)
                    continue;

                bool selectInSolutionExplorer = false;

                try
                {
                    switch (e.Command)
                    {
                        case AnkhCommand.ItemOpenVisualStudio:
                            IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();

                            if (mapper.IsProjectFileOrSolution(item.FullPath))
                            {
                                selectInSolutionExplorer = true;
                                break;
                            }
                            if (item.IsDirectory)
                                goto case AnkhCommand.ItemOpenWindows;

                            VsShellUtilities.OpenDocument(e.Context, item.FullPath);
                            break;
                        case AnkhCommand.ItemOpenTextEditor:
                            {
                                IVsUIHierarchy hier;
                                IVsWindowFrame frame;
                                uint id;

                                if (!item.IsFile)
                                    continue;

                                VsShellUtilities.OpenDocument(e.Context, item.FullPath, VSConstants.LOGVIEWID_TextView, out hier, out id, out frame);
                            }
                            break;
                        case AnkhCommand.ItemOpenWindows:
                            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(item.FullPath);
                            psi.Verb = "open";
                            System.Diagnostics.Process.Start(psi);
                            break;
                    }
                }
                catch (IOException ee)
                {
                    mb.Show(ee.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (COMException ee)
                {
                    mb.Show(ee.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (InvalidOperationException ee)
                {
                    mb.Show(ee.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (System.ComponentModel.Win32Exception ee)
                {
                    mb.Show(ee.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (selectInSolutionExplorer)
                {
                    IVsUIHierarchyWindow hierWindow = VsShellUtilities.GetUIHierarchyWindow(e.Context, new Guid(ToolWindowGuids80.SolutionExplorer));

                    IVsProject project = VsShellUtilities.GetProject(e.Context, item.FullPath) as IVsProject;

                    if (hierWindow != null)
                    {
                        int found;
                        uint id;
                        VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];
                        if (project != null && VSErr.Succeeded(project.IsDocumentInProject(item.FullPath, out found, prio, out id)) && found != 0)
                        {
                            hierWindow.ExpandItem(project as IVsUIHierarchy, id, EXPANDFLAGS.EXPF_SelectItem);
                        }
                        else if (string.Equals(item.FullPath, e.Selection.SolutionFilename, StringComparison.OrdinalIgnoreCase))
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
                }
            }
        }
    }
}
