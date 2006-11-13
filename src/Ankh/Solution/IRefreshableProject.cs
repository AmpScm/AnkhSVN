using System;
using System.Text;
using EnvDTE;

namespace Ankh.Solution
{
    public interface IRefreshableProject
    {
        Project Project { get; }
        bool IsValid { get; }
    }
}
