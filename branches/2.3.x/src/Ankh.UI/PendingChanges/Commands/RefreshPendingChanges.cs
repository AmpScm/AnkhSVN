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
using System.Text;
using Ankh.Commands;
using Ankh.UI.PendingChanges.Commits;
using Ankh.UI.PendingChanges.Synchronize;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.RefreshPendingChanges)]
    public class RefreshPendingChanges : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SolutionExists)
            {
                e.Enabled = false;
                return;
            }
            PendingChangesPage page = GetPage(e);

            if (page == null || !page.CanRefreshList)
                e.Enabled = false;
        }

        private PendingChangesPage GetPage(BaseCommandEventArgs e)
        {
            PendingChangesPage page = e.Context.GetService<PendingCommitsPage>();

            if (page != null && page.Visible)
                return page;

            page = e.Context.GetService<RecentChangesPage>();

            if (page != null && page.Visible)
                return page;

            return null;
        }

        public void OnExecute(CommandEventArgs e)
        {
            PendingChangesPage page = GetPage(e);

            if(page != null && page.CanRefreshList)
                page.RefreshList();
        }
    }
}
