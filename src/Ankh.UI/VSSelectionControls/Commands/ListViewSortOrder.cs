using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.VSSelectionControls.Commands
{
    [Command(AnkhCommand.ListViewSortAscending, AlwaysAvailable = true, HideWhenDisabled = false)]
    [Command(AnkhCommand.ListViewSortDescending, AlwaysAvailable = true, HideWhenDisabled = false)]
    class ListViewSortOrder : ListViewCommandBase
    {
        protected override void OnUpdate(SmartListView list, Ankh.Commands.CommandUpdateEventArgs e)
        {
            bool foundOne = false;
            bool ok = false;

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
            bool next = false;
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
