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
using Ankh.UI;
using Ankh.Ids;
using Ankh.Scc;
using Microsoft.VisualStudio;
using Ankh.UI.WorkingCopyExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to refresh this view.
    /// </summary>
    [Command(AnkhCommand.Refresh)]
    class RefreshCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (null == EnumTools.GetFirst(e.Selection.GetSelectedFiles(true)))
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            // Refresh all global states on the selected files
            // * File Status Cache
            // * Glyph cache (in VS Projects)
            // * Pending changes
            // * Editor dirty state

            // Don't handle individual windows here, they can just override the refresh handler

            // See WorkingCopyExplorerControl.OnFrameCreated() for some examples on how to do that

            IFileStatusMonitor monitor = e.GetService<IFileStatusMonitor>();

            monitor.ScheduleSvnStatus(e.Selection.GetSelectedFiles(true));

            IAnkhOpenDocumentTracker dt = e.GetService<IAnkhOpenDocumentTracker>();

            dt.RefreshDirtyState();

            IPendingChangesManager pm = e.GetService<IPendingChangesManager>();

            pm.Refresh((string)null); // Perform a full incremental refresh on the PC window            
        }
    }
}
