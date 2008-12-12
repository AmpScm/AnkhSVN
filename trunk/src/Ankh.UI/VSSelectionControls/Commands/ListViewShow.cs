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
//using System.Windows.Forms;
using Ankh.Commands;
using Ankh.Ids;
using System.Diagnostics;
using System.Windows.Forms;

namespace Ankh.UI.VSSelectionControls.Commands
{
    [Command(AnkhCommand.ListViewShow0, LastCommand = AnkhCommand.ListViewShowMax, AlwaysAvailable = true, HideWhenDisabled = false)]
    class ListViewShow : ListViewCommandBase
    {
        protected override void OnUpdate(SmartListView list, CommandUpdateEventArgs e)
        {
            int n = (int)(e.Command - AnkhCommand.ListViewShow0);

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

            if (!column.Hideable)
                e.Enabled = false;

            e.Checked = list.Columns.Contains(column);
        }

        protected override void OnExecute(SmartListView list, CommandEventArgs e)
        {
            int n = (int)(e.Command - AnkhCommand.ListViewShow0);

            SmartColumn sc = list.AllColumns[n];
            int col = sc.Index;
            if (col > 0)
            {
                list.Columns.Remove(sc);

                if (!list.VirtualMode)
                {
                    Debug.Assert(sc.Index < 0);

                    foreach (ListViewItem li in list.Items)
                    {
                        if (li.SubItems.Count > col)
                            li.SubItems.RemoveAt(col);
                    }
                }
            }
            else
            {
                list.Columns.Add(sc);

                if (!list.VirtualMode)
                {
                    col = sc.Index;
                    foreach (ListViewItem li in list.Items)
                    {
                        SmartListViewItem si = li as SmartListViewItem;

                        if (si != null)
                            si.SetValue(sc.AllColumnsIndex, si.GetValue(sc.AllColumnsIndex));
                    }
                }
            }            
        }

    }
}
