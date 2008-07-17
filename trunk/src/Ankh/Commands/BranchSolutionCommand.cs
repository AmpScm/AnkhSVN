using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ProjectBranch)]
    [Command(AnkhCommand.SolutionBranch)]
    class BranchSolutionCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            e.Enabled = false;
        }
        public override void OnExecute(CommandEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
