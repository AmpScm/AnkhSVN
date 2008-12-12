// $Id$
//
// Copyright 2005-2008 The AnkhSVN Project
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

// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;
using SharpSvn;
using Ankh.UI;
using Ankh.UI.RepositoryExplorer;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to add a new URL to the Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.RepositoryBrowse, ArgumentDefinition="u", AlwaysAvailable=true)]
    class RepositoryBrowseCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IUIShell shell = e.GetService<IUIShell>();
            Uri info;

            if (e.Argument is string)
            {
                // Allow opening from
                info = new Uri((string)e.Argument);
            }
            else if (e.Argument is Uri)
                info = (Uri)e.Argument;
            else
                info = shell.ShowAddRepositoryRootDialog();

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
