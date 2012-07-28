using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    public interface ISccHelper
    {
        bool EnsureProjectLoaded(Guid project, bool recursive);
    }
}
