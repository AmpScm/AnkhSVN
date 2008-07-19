using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop; 

using Ankh.UI;
using Ankh.Ids;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemOpenVisualStudio)]
    [Command(AnkhCommand.ItemOpenFolder)]
    [Command(AnkhCommand.ItemOpenSolutionExplorer)]
    [Command(AnkhCommand.ItemOpenTextEditor)]
    [Command(AnkhCommand.ItemOpenWindows)]
    class OpenInVisualStudio : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool first = true;
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (!item.Exists)
                    continue;
                else
                    switch (e.Command)
                    {
                        case AnkhCommand.ItemOpenTextEditor:
                        case AnkhCommand.ItemOpenVisualStudio:
                            if (item.IsFile)
                                return;
                            break;
                        default:
                            if (!first)
                            {
                                e.Enabled = false;
                                return;
                            }
                            else
                                first = false;
                            break;
                    }
            }

            e.Enabled = !first;
        }
        public override void OnExecute(CommandEventArgs e)
        {
            AnkhMessageBox mb = new AnkhMessageBox(e.Context);
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (!item.Exists)
                    continue;

                try
                {
                    switch (e.Command)
                    {
                        case AnkhCommand.ItemOpenVisualStudio:
                            {
                                Ankh.Scc.IProjectFileMapper mapper = e.GetService<Ankh.Scc.IProjectFileMapper>();

                                if (mapper.IsProjectFileOrSolution(item.FullPath))
                                    goto case AnkhCommand.ItemOpenSolutionExplorer;
                            }

                            if (!item.IsFile || !item.Exists)
                                continue;

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
                        case AnkhCommand.ItemOpenFolder:
                            if (!item.IsDirectory)
                                System.Diagnostics.Process.Start(Path.GetDirectoryName(item.FullPath));
                            else
                                System.Diagnostics.Process.Start(item.FullPath);
                            break;
                        case AnkhCommand.ItemOpenWindows:
                            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(item.FullPath);
                            psi.Verb = "open";
                            System.Diagnostics.Process.Start(psi);
                            break;

                        case AnkhCommand.ItemOpenSolutionExplorer:
                            IVsUIHierarchyWindow hierWindow = VsShellUtilities.GetUIHierarchyWindow(e.Context, new Guid(ToolWindowGuids80.SolutionExplorer));                            

                            IVsProject project = VsShellUtilities.GetProject(e.Context, item.FullPath) as IVsProject;

                            if (project != null && hierWindow != null)
                            {
                                int found;
                                uint id;
                                VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];
                                if (ErrorHandler.Succeeded(project.IsDocumentInProject(item.FullPath, out found, prio, out id)) && found != 0)
                                {
                                    hierWindow.ExpandItem(project as IVsUIHierarchy, id, EXPANDFLAGS.EXPF_SelectItem);

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
                    
            }            
        }
    }
}
