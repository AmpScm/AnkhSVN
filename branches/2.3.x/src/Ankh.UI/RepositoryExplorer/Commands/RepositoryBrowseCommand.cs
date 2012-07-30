// $Id: RepositoryBrowseCommand.cs 10665 2011-12-30 12:22:24Z rhuijben $
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

using System;
using System.Windows.Forms;

using Ankh.Scc;
using Ankh.Commands;

namespace Ankh.UI.RepositoryExplorer.Commands
{
    /// <summary>
    /// Command to add a new URL to the Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.RepositoryBrowse, ArgumentDefinition = "u|d", AlwaysAvailable = true)]
    sealed class RepositoryBrowseCommand : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public void OnExecute(CommandEventArgs e)
        {
            Uri info;

            if (e.Argument is string)
            {
                string arg = (string)e.Argument;

                info = null;
                if (SvnItem.IsValidPath(arg, true))
                {
                    SvnItem item = e.GetService<IFileStatusCache>()[arg];

                    if (item.IsVersioned)
                    {
                        info = item.Uri;

                        if (item.IsFile)
                            info = new Uri(info, "./");
                    }
                }

                if (info == null)
                    info = new Uri((string)e.Argument);
            }
            else if (e.Argument is Uri)
                info = (Uri)e.Argument;
            else
                using (AddRepositoryRootDialog dlg = new AddRepositoryRootDialog())
                {
                    if (dlg.ShowDialog(e.Context) != DialogResult.OK || dlg.Uri == null)
                        return;

                    info = dlg.Uri;
                }

            if (info != null)
            {
                RepositoryExplorerControl ctrl = e.Selection.ActiveDialogOrFrameControl as RepositoryExplorerControl;

                if (ctrl == null)
                {
                    IAnkhPackage pkg = e.GetService<IAnkhPackage>();
                    pkg.ShowToolWindow(AnkhToolWindow.RepositoryExplorer);
                }

                ctrl = e.Selection.ActiveDialogOrFrameControl as RepositoryExplorerControl;

                if (ctrl != null)
                    ctrl.AddRoot(info);
            }
        }
    }
}
