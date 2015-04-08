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
using Ankh.Commands;

namespace Ankh.Scc.Commands
{
    /// <summary>
    /// Handles the finishtasks special command; this command is posted to the back of the command queueue
    /// if the SCC implementation needs to perform some post processing of VSs scc actions
    /// </summary>
    [SvnCommand(AnkhCommand.SccFinishTasks, AlwaysAvailable = true)]
    sealed class TaskFinisher : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        IAnkhCommandService _commandService;
        ProjectTracker _projectTracker;
        SvnSccProvider _sccProvider;

        public void OnExecute(CommandEventArgs e)
        {
            if (_commandService == null)
                _commandService = e.GetService<IAnkhCommandService>();
            if (_projectTracker == null)
                _projectTracker = e.GetService<ProjectTracker>();
            if (_sccProvider == null)
                _sccProvider = e.GetService<SvnSccProvider>(typeof(IAnkhSccService));

            _commandService.TockCommand(e.Command);

            _projectTracker.OnSccCleanup(e);
            _sccProvider.OnSccCleanup(e);
        }
    }
}
