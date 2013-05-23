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
using System.Text;
using Ankh.Commands;

namespace Ankh.UI.Commands
{
    [SvnCommand(AnkhCommand.ForceUIShow, AlwaysAvailable=true, MaxVersion=VSInstance.VS2008)]
    sealed class UIEditCommand : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            e.Enabled = e.Visible = false;

            // We use this command in Visual Studio 2005/2008 to make all commands visible
            // in the menu/toolbar editor.

            // We explicitly disable this in VS-Versions that don't use this to make sure
            // we don't accidentally enable the customize mode

            if (VSVersion.VS2008OrOlder)
                e.GetService<CommandMapper>().EnableCustomizeMode();
        }

        public void OnExecute(CommandEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
