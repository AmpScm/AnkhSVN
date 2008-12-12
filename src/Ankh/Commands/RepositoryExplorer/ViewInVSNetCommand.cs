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

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// A command that opens a file from the server in VS.NET
    /// </summary>
    [Command(AnkhCommand.ViewInVsNet)]
    [Command(AnkhCommand.ViewInWindows)]
    class ViewInVSNetCommand : ViewRepositoryFileCommand
    {
        const int NOASSOCIATEDAPP = 1155;

        public override void OnExecute(CommandEventArgs e)
        {
            ISvnRepositoryItem ri = null;

            foreach (ISvnRepositoryItem i in e.Selection.GetSelection<ISvnRepositoryItem>())
            {
                ri = i;
                break;
            }
            if (ri == null)
                return;

            string toFile = e.GetService<IAnkhTempFileManager>().GetTempFileNamed(ri.Name);

            SaveFile(e, ri, toFile);

            if (e.Command == AnkhCommand.ViewInVsNet)
                VsShellUtilities.OpenDocument(e.Context, toFile);
            else
            {
                Process process = new Process();
                process.StartInfo.FileName = toFile;
                process.StartInfo.UseShellExecute = true;

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
