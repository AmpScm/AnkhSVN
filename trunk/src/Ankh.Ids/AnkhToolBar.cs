using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace AnkhSvn.Ids
{
    [Guid(AnkhId.CommandSet)]
    public enum AnkhToolBar
    {

        ToolBarFirst = 0x7FFFFFF,
        PendingChanges,
        SourceControl,
    }
}
