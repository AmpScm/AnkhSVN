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
using System.Windows.Forms;

namespace Ankh.UI.VSSelectionControls.Commands
{
    abstract class ListViewCommandBase : ICommandHandler
    {
        public virtual void OnUpdate(CommandUpdateEventArgs e)
        {
            SmartListView list = GetListView(e);

            if (list == null)
            {
                e.Enabled = false;
                return;
            }

            OnUpdate(list, e);
        }

        public virtual void OnExecute(CommandEventArgs e)
        {
            SmartListView list = GetListView(e);

            if (list == null)
                return;

            OnExecute(list, e);
        }

        private static SmartListView GetListView(BaseCommandEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            Control c = e.Selection.ActiveDialogOrFrameControl;
            SmartListView list = null;
            ContainerControl cc;
            while (null != (cc = c as ContainerControl))
            {
                c = cc.ActiveControl;

                list = c as SmartListView;
                if (list != null)
                    break;
            }

            if (list != null)
                list = c as SmartListView;
            return list;
        }

        protected abstract void OnUpdate(SmartListView list, CommandUpdateEventArgs e);
        protected abstract void OnExecute(SmartListView list, CommandEventArgs e);
    }
}
