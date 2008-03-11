using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh
{
    interface IAnkhRuntimeInfo
    {
        bool IsInAutomation { get; }
    }
}
