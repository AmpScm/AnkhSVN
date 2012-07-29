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

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogFollowCopies, AlwaysAvailable = true)]
    sealed class LogFollowCopies : ICommandHandler
    {
        LogToolWindowControl _ctrl;

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (_ctrl == null)
                _ctrl = e.GetService<LogToolWindowControl>();

            if (_ctrl == null)
            {
                e.Enabled = false;
                return;
            }

            e.Checked = !_ctrl.StopOnCopy;
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (_ctrl == null)
                return;

            _ctrl.StopOnCopy = !_ctrl.StopOnCopy;
        }
    }
}
