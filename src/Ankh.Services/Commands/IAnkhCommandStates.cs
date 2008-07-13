using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Commands
{
    public interface IAnkhCommandStates
    {
        bool CodeWindow { get; }

        bool Debugging { get; }

        bool DesignMode { get; }

        bool Dragging { get; }

        bool EmptySolution { get; }

        bool FullScreenMode { get; }

        bool NoSolution { get; }

        bool SolutionBuilding { get; }

        bool SolutionExists { get; }

        bool SolutionHasMultipleProjects { get; }

        bool SolutionHasSingleProject { get; }

        bool SccProviderActive { get; }

        bool OtherSccProviderActive { get; }
        bool OtherSccProviderMarkedActive { get; }

        bool GetRawOtherSccProviderMarkedActive();
        bool GetRawOtherSccProviderActive();
    }
}
