using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Commands
{
    public interface ICommandHandler
    {
        void OnUpdate(CommandUpdateEventArgs e);
        void OnExecute(CommandEventArgs e);        
    }
}
