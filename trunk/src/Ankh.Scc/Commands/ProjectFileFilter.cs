// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.Selection;

namespace Ankh.Scc.Commands
{
    [Command((AnkhCommand)AnkhCommandMenu.ProjectFileScc)]
    class ProjectFileFilter : ICommandHandler
    {
        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.State.SccProviderActive)
                foreach (SvnProject p in e.Selection.GetSelectedProjects(false))
                {
                    ISvnProjectInfo pi = e.GetService<IProjectFileMapper>().GetProjectInfo(p);

                    if (p == null || pi == null || string.IsNullOrEmpty(pi.ProjectFile))
                    {
                        break; // No project file
                    }

                    if (!string.IsNullOrEmpty(pi.ProjectDirectory) &&
                        string.Equals(pi.ProjectDirectory, pi.ProjectFile, StringComparison.OrdinalIgnoreCase))
                    {
                        break; // Project file is directory
                    }

                    SvnItem item = e.GetService<IFileStatusCache>()[pi.ProjectFile];

                    if (item != null && item.IsDirectory)
                        break; // Project file is not file

                    return; // Show the menu
                }

            e.Enabled = e.Visible = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            throw new InvalidOperationException(); // Never reached; not a real command
        }

        #endregion
    }
}
