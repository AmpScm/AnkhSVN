using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    public interface ISvnProjectInfo
    {
        string ProjectName { get; }
        string ProjectDirectory { get; }
    }
}
