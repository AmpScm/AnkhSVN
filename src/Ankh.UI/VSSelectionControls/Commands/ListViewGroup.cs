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
    [Command(AnkhCommand.ListViewGroup0, LastCommand = AnkhCommand.ListViewGroupMax, AlwaysAvailable = true)]
    [Command((AnkhCommand)AnkhCommandMenu.ListViewGroup, AlwaysAvailable=true)]
    class ListViewGroup : ListViewCommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!SmartListView.SupportsGrouping)
            {
                e.Visible = e.Enabled = false; // Group by is XP+
                e.DynamicMenuEnd = (e.Command != (AnkhCommand)AnkhCommandMenu.ListViewGroup);
                return;
            }

            if (e.Command == (AnkhCommand)AnkhCommandMenu.ListViewGroup)
                return;

            base.OnUpdate(e);
        }

        protected override void OnUpdate(SmartListView list, CommandUpdateEventArgs e)
        {
            int n = (int)(e.Command - AnkhCommand.ListViewGroup0);

            if (n >= list.AllColumns.Count || n < 0)
            {
                e.Text = "";
                e.DynamicMenuEnd = true;
                return;
            }

            SmartColumn column = list.AllColumns[n];

            if (e.TextQueryType == TextQueryType.Name)
            {
                e.Text = column.MenuText;
            }

            if (column == null || !column.Groupable)
                e.Enabled = false;

            e.Checked = list.GroupColumns.Contains(column);
        }

        protected override void OnExecute(SmartListView list, CommandEventArgs e)
        {
            bool extend = ((Control.ModifierKeys & Keys.Shift) != 0);

            int n = (int)(e.Command - AnkhCommand.ListViewGroup0);
            SmartColumn column = list.AllColumns[n];

            if (list.GroupColumns.Contains(column))
            {
                list.GroupColumns.Remove(column);
            }
            else if (!extend)
            {
                list.GroupColumns.Clear();
                list.GroupColumns.Add(column);
            }
            else
            {
                list.GroupColumns.Add(column);
            }

            list.RefreshGroups();
        }
    }
}
