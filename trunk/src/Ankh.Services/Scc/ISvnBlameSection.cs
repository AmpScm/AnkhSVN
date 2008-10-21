using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    public interface IBlameSection
    {
        string Author { get; }
        long Revision { get; }
        DateTime Time { get; }
        SvnOrigin Origin { get; }
    }
}
