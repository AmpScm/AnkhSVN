using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using System.Windows.Forms;

namespace Ankh.UI.VSSelectionControls.Commands
{
    [Command(AnkhCommand.ListViewSort0, LastCommand = AnkhCommand.ListViewSortMax, AlwaysAvailable = true)]
    class ListViewSort : ListViewCommandBase
    {
        protected override void OnUpdate(SmartListView list, CommandUpdateEventArgs e)
        {
            int n = (int)(e.Command - AnkhCommand.ListViewSort0);

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

            if (!column.Sortable)
                e.Enabled = false;

            e.Checked = list.SortColumns.Contains(column);
        }

        protected override void OnExecute(SmartListView list, CommandEventArgs e)
        {
            bool extend = ((Control.ModifierKeys & Keys.Shift) != 0);

            int n = (int)(e.Command - AnkhCommand.ListViewSort0);
            SmartColumn column = list.AllColumns[n];

            if (list.SortColumns.Contains(column))
            {
                list.SortColumns.Remove(column);

                list.UpdateSortGlyphs();

                if (list.SortColumns.Count > 0)
                    list.Sort();
            }
            else if (!extend)
            {
                list.SortColumns.Clear();
                list.SortColumns.Add(column);
                list.UpdateSortGlyphs();
                list.Sort();
            }
            else
            {
                list.SortColumns.Add(column);
                list.UpdateSortGlyphs();
                list.Sort();
            }
        }
    }
}
