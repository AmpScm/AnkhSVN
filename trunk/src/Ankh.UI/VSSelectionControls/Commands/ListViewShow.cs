using System;
using System.Collections.Generic;
using System.Text;
//using System.Windows.Forms;
using Ankh.Commands;
using Ankh.Ids;

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
            if (n >= list.Columns.Count || n < 0)
                return;

            System.Windows.Forms.ColumnHeader ch = list.Columns[n];

            int i = ch.DisplayIndex;

            if(i < 0)
                ch.DisplayIndex = list.Columns.Count; // Auto fixed?
            else
                ch.DisplayIndex = -1;
        }

    }
}
