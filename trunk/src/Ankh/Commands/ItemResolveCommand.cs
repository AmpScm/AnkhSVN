using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using SharpSvn;

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
            else if(/* Subversion 1.5 && */ (e.Command == AnkhCommand.ItemResolveTheirsConflict || e.Command == AnkhCommand.ItemResolveMineConflict))
                e.Visible = e.Enabled = false;
            else
            {               
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            SvnAccept accept = SvnAccept.Postpone;
            switch (e.Command)
            {
                case AnkhCommand.ItemResolveMineFull:
                    accept = SvnAccept.MineFull;
                    break;
                case AnkhCommand.ItemResolveTheirsFull:
                    accept = SvnAccept.TheirsFull;
                    break;
                case AnkhCommand.ItemResolveWorking:
                    accept = SvnAccept.Merged;
                    break;
                case AnkhCommand.ItemResolveBase:
                    accept = SvnAccept.Base;
                    break;
                case AnkhCommand.ItemResolveMineConflict:
                case AnkhCommand.ItemResolveTheirsConflict:
                default:
                    throw new NotImplementedException();                
            }
            if(accept == SvnAccept.Postpone)
                throw new NotImplementedException();

            using (SvnClient client = e.GetService<ISvnClientPool>().GetNoUIClient())
            {
                SvnResolveArgs a = new SvnResolveArgs();
                a.Depth = SvnDepth.Empty;

                foreach(SvnItem item in e.Selection.GetSelectedSvnItems(true))
                {
                    if(!item.IsConflicted)
                        continue;

                    // Let the command throw exceptions for now
                    client.Resolve(item.FullPath, accept, a);
                }                
            }
        }
    }
}
