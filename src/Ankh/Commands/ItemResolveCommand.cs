using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemResolveMerge)]
    [Command(AnkhCommand.ItemResolveMineFull)]
    [Command(AnkhCommand.ItemResolveTheirsFull)]
    [Command(AnkhCommand.ItemResolveMineConflict)]
    [Command(AnkhCommand.ItemResolveTheirsConflict)]
    [Command(AnkhCommand.ItemResolveBase)]
    [Command(AnkhCommand.ItemResolveWorking)]
    [Command(AnkhCommand.ItemResolveMergeTool)]
    class ItemResolveCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool foundOne = false;
            bool canDiff = true;
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (!item.IsConflicted)
                    continue;

                foundOne = true;

                if (!item.IsTextFile)
                {
                    canDiff = false;
                    break;                
                }
            }

            if (!foundOne)
                e.Visible = e.Enabled = false;
            else if (!canDiff && (e.Command == AnkhCommand.ItemResolveTheirsConflict || e.Command == AnkhCommand.ItemResolveMineConflict))
                e.Visible = e.Enabled = false;
            else
            {               
                //e.Enabled = false; // Not supported yet
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
