// $Id$
using EnvDTE;
using Ankh.Solution;
using System.Collections;

namespace Ankh
{
    public interface ISolutionExplorer : IAnkhSelectionContainer
    {
        /// <summary>
        /// Refreshes all subnodes of a specific project.
        /// </summary>
        /// <param name="project"></param>
        void Refresh( Project project );


        /// <summary>
        /// Removes the project from Ankh's control.
        /// </summary>
        /// <param name="project"></param>
        void RemoveProject( Project project );

        /// <summary>	 	
        /// Visits all the selected nodes.	 	
        /// </summary>	 	
        /// <param name="visitor"></param>	 	
        void VisitSelectedNodes( INodeVisitor visitor );

        /// <summary>
        /// Notify the Solution Explorer to unload.
        /// </summary>
        void Unload();

        /// <summary>
        /// Whether a rename is currently in progress.
        /// </summary>
        bool RenameInProgress { get; }

        /// <summary>
        /// Set up a refresh for the project after a delay.
        /// </summary>
        /// <param name="project"></param>
        void SetUpDelayedProjectRefresh( IRefreshableProject project );

        /// <summary>
        /// Set up a refresh for the entire solution after a delay.
        /// </summary>
        void SetUpDelayedSolutionRefresh();
    }
}
