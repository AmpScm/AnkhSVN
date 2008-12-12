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
using Ankh.UI.SccManagement;
using Ankh.Ids;

namespace Ankh.Commands
{
    [Command(AnkhCommand.FileSccChangeSourceControl)]
    sealed class ChangeSourceControlCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SolutionExists)
            {
                e.Visible = e.Enabled = false;
                return;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            // TODO: Verify if all project files and the solution are open and saved

            using (ChangeSourceControl csc = new ChangeSourceControl())
            {
                csc.Context = e.Context;

                csc.ShowDialog(e.Context.DialogOwner);
            }
        }
    }
}
