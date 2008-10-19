using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc.UI
{
    public interface IBlameControl
    {
        bool HasWorkingCopyItems { get; }
        SvnItem[] WorkingCopyItems { get; }
    }
}
