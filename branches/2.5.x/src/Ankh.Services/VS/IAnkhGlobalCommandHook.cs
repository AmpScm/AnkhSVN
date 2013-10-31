using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Ankh.VS
{
    public interface IAnkhGlobalCommandHook
    {
        void HookCommand(CommandID command, EventHandler handler);
        void UnhookCommand(CommandID command, EventHandler handler);
    }
}
