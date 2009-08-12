// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Ankh.Scc;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemAddToPending)]
    [Command(AnkhCommand.ItemRemoveFromPending)]
    class ItemAddToPending : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool add = (e.Command == AnkhCommand.ItemAddToPending);
            IPendingChangesManager pcm= null;

            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
            {
                if (i.InSolution)
                    continue;

                if (!PendingChange.IsPending(i))
                    continue;

                if (pcm == null)
                {
                    pcm = e.GetService<IPendingChangesManager>();
                    if (pcm == null)
                        break;
                }

                if (pcm.Contains(i.FullPath) != add)
                    return;
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IFileStatusMonitor fsm = e.GetService<IFileStatusMonitor>();

            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
            {
                if (i.InSolution)
                    continue;

                if (e.Command == AnkhCommand.ItemAddToPending)
                    fsm.ScheduleMonitor(i.FullPath);
                else
                    fsm.StopMonitoring(i.FullPath);
            }            
        }
    }
}
