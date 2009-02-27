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

namespace Ankh.Scc.Commands
{
    /// <summary>
    /// Handles the finishtasks special command; this command is posted to the back of the command queueue
    /// if the SCC implementation needs to perform some post processing of VSs scc actions
    /// </summary>
    [Command(AnkhCommand.SccFinishTasks, AlwaysAvailable=true)]
    sealed class TaskFinisher : ICommandHandler
    {
        ProjectTracker _tracker;
        AnkhSccProvider _scc;

        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (_tracker == null)
                _tracker = (ProjectTracker)e.Context.GetService<IAnkhProjectDocumentTracker>();
            if(_scc == null)
                _scc = (AnkhSccProvider)e.Context.GetService<IAnkhSccService>();

            if(_tracker != null)
                _tracker.OnSccCleanup(e);

            if (_scc != null)
                _scc.OnSccCleanup(e);            
        }

        #endregion
    }
}
