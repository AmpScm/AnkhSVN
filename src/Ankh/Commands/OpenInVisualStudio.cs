using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;

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
            if (e.Command == AnkhCommand.ItemOpenSolutionExplorer)
            {
                e.Enabled = false; // Not implemented yet
                return;
            }
            bool first = true;
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems())
            {
                if (!item.Exists)
                    continue;
                else if(e.Command == AnkhCommand.ItemOpenVisualStudio || e.Command == AnkhCommand.ItemOpenTextEditor)
                    return;
                else if(!first)
                {
                    e.Enabled = false;
                    return;
                }
                else
                    first = false;
            }

            e.Enabled = !first;
        }
        public override void OnExecute(CommandEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems())
            {
                if (!item.Exists)
                    continue;

                switch (e.Command)
                {
                    case AnkhCommand.ItemOpenVisualStudio:
                        VsShellUtilities.OpenDocument(e.Context, item.FullPath);
                        break;
                    case AnkhCommand.ItemOpenTextEditor:
                        {
                            IVsUIHierarchy hier;
                            IVsWindowFrame frame;
                            uint id;

                            VsShellUtilities.OpenDocument(e.Context, item.FullPath, VSConstants.LOGVIEWID_TextView, out hier, out id, out frame);
                        }
                        break;
                    case AnkhCommand.ItemOpenFolder:
                        if(!item.IsDirectory)
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
                        //VsShellUtilities.GetRunningDocumentContents
                        break;
                }
            }            
        }
    }
}
