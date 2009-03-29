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

namespace Ankh.UI.VSSelectionControls.Commands
{
    [Command(AnkhCommand.ListViewSortAscending, AlwaysAvailable = true, HideWhenDisabled = false)]
    [Command(AnkhCommand.ListViewSortDescending, AlwaysAvailable = true, HideWhenDisabled = false)]
    class ListViewSortOrder : ListViewCommandBase
    {
        protected override void OnUpdate(SmartListView list, Ankh.Commands.CommandUpdateEventArgs e)
        {
            bool foundOne = false;

            e.Checked = true;

            foreach (SmartColumn sc in list.SortColumns)
            {
                foundOne = true;

                switch (e.Command)
                {
                    case AnkhCommand.ListViewSortAscending:
                        if (sc.ReverseSort)
                        {
                            e.Checked = false;
                            return;
                        }
                        break;
                    case AnkhCommand.ListViewSortDescending:
                        if (!sc.ReverseSort)
                        {
                            e.Checked = false;
                            return;
                        }
                        break;
                }
            }
            if (!foundOne)
            {
                e.Checked = e.Enabled = false;
            }
        }

        protected override void OnExecute(SmartListView list, CommandEventArgs e)
        {
            bool value = (e.Command == AnkhCommand.ListViewSortDescending);

            foreach (SmartColumn sc in list.SortColumns)
            {
                sc.ReverseSort = value;
            }
            list.UpdateSortGlyphs();
            list.Sort();
        }
    }
}
