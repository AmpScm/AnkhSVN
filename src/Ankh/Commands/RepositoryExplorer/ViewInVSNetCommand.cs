// $Id$
//
// Copyright 2003-2008 The AnkhSVN Project
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
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;
using System.Windows.Forms.Design;
using Ankh.VS;
using System.IO;
using Ankh.Scc;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// A command that opens a file from the server in VS.NET
    /// </summary>
    [Command(AnkhCommand.ViewInVsNet, AlwaysAvailable=true)]
    [Command(AnkhCommand.ViewInWindows, AlwaysAvailable=true)]
    [Command(AnkhCommand.ViewInVsText, AlwaysAvailable=true)]
    [Command(AnkhCommand.ViewInWindowsWith, AlwaysAvailable=true)]
    class ViewInVSNetCommand : ViewRepositoryFileCommand
    {
        const int NOASSOCIATEDAPP = 1155;

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            base.OnUpdate(e);


            if (e.Enabled && e.Command == AnkhCommand.ViewInVsNet)
            {
                ISvnRepositoryItem single = EnumTools.GetSingle(e.Selection.GetSelection<ISvnRepositoryItem>());
                IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();

                SvnOrigin origin = single.Origin; // Checked in parent

                string ext = Path.GetExtension(origin.Target.FileName);

                if (!string.IsNullOrEmpty(ext) && settings.OpenFileFilter.IndexOf("*" + ext, StringComparison.OrdinalIgnoreCase) < 0)
                    e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            ISvnRepositoryItem ri = null;

            foreach (ISvnRepositoryItem i in e.Selection.GetSelection<ISvnRepositoryItem>())
            {
                if (i.Origin == null)
                    continue;

                ri = i;
                break;
            }
            if (ri == null)
                return;

            string toFile = e.GetService<IAnkhTempFileManager>().GetTempFileNamed(ri.Origin.Target.FileName);

            if (!SaveFile(e, ri, toFile))
                return;

            if (e.Command == AnkhCommand.ViewInVsNet)
                VsShellUtilities.OpenDocument(e.Context, toFile);
            else if (e.Command == AnkhCommand.ViewInVsText)
            {
                IVsUIHierarchy hier;
                IVsWindowFrame frame;
                uint id;
                VsShellUtilities.OpenDocument(e.Context, toFile, VSConstants.LOGVIEWID_TextView, out hier, out id, out frame);
            }
            else
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;

                if (e.Command == AnkhCommand.ViewInWindowsWith)
                {
                    // TODO: BH: I tested with adding quotes around {0} but got some error
                    process.StartInfo.FileName = "rundll32.exe";
                    process.StartInfo.Arguments = string.Format("Shell32,OpenAs_RunDLL {0}", toFile);
                }
                else
                    process.StartInfo.FileName = toFile;

                try
                {
                    process.Start();
                }
                catch (Win32Exception ex)
                {
                    // no application is associated with the file type
                    if (ex.NativeErrorCode == NOASSOCIATEDAPP)
                        e.GetService<IAnkhDialogOwner>()
                            .MessageBox.Show("Windows could not find an application associated with the file type",
                            "No associated application", MessageBoxButtons.OK);
                    else
                        throw;
                }
            }
        }        
    }
}
