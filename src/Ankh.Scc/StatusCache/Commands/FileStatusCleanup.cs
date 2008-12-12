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
using Ankh.Scc;
using Ankh.Ids;

namespace Ankh.Scc.StatusCache.Commands
{
    [Command(AnkhCommand.FileCacheFinishTasks)]
    [Command(AnkhCommand.TickRefreshSvnItems)]
    public class FileStatusCleanup : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {            
        }

        public void OnExecute(CommandEventArgs e)
        {
            FileStatusCache cache = e.Context.GetService<FileStatusCache>(typeof(IFileStatusCache));

            if (cache != null)
            {
                if (e.Command == AnkhCommand.FileCacheFinishTasks)
                    cache.OnCleanup();
                else
                    cache.BroadcastChanges();
            }
        }
    }
}
