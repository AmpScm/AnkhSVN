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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.UI;
using Ankh.Selection;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.Scc.ProjectMap;
using System.Diagnostics;

namespace Ankh.Scc.Commands
{
    [Command(AnkhCommand.MarkProjectDirty, AlwaysAvailable=true)]    
    public class MarkProjectDirty : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public void OnExecute(CommandEventArgs e)
        {
            ProjectNotifier pn = e.GetService<ProjectNotifier>(typeof(IFileStatusMonitor));

            Debug.Assert(pn != null, "ProjectNotifier must be available!", "ProjectNotifier service not available");
            if (pn != null)
            {
                pn.HandleEvent(e.Command);
            }
        }
    }
}
