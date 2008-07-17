using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.VS.Extenders;
using Ankh.VS.SolutionExplorer;

namespace Ankh.VS
{
    [Command(AnkhCommand.ActivateVsExtender, AlwaysAvailable=true)]
    class VsActivatorCommand : ICommandHandler
    {
        static bool _activated;
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            return;
        }

        public void OnExecute(CommandEventArgs e)
        {
            if(_activated)
                return;

            _activated = true;

            e.GetService<AnkhExtenderProvider>().Initialize();
            e.GetService<SolutionExplorerWindow>(typeof(IAnkhSolutionExplorerWindow)).Initialize();
        }
    }
}
