// $Id$
using System.Collections;
using EnvDTE;

namespace Ankh.Solution
{
    public interface ISelectionContainer
    {
        /// <summary>
        /// Updates the status of selected items.
        /// </summary>
        void UpdateSelectionStatus();

        /// <summary>
        /// Refreshes the parents of the selected items.
        /// </summary>
        void RefreshSelectionParents();

        /// <summary>
        /// Refreshes the current selection.
        /// </summary>
        void RefreshSelection();

        void SyncAll();

        /// <summary>
        /// Returns the SvnItem resources associated with the selected items
        /// in the solution explorer.
        /// </summary>
        /// <param name="getChildItems">Whether children of the items in 
        /// question should be included.</param>        /// 
        /// <returns>A list of SvnItem instances.</returns>
        IList GetSelectionResources( bool getChildItems );

        /// <summary>	 	
        /// Visits all the selected nodes.	 	
        /// </summary>	 	
        /// <param name="visitor"></param>	 	
        void VisitSelectedNodes( INodeVisitor visitor );

        /// <summary>
        /// Returns the SvnItem resources associated with the selected items
        /// in the solution explorer.
        /// </summary>
        /// <param name="getChildItems">Whether children of the items in 
        /// question should be included.</param>
        /// <param name="filter">A callback used to filter the items
        /// that are added.</param>
        /// <returns>A list of SvnItem instances.</returns>
        IList GetSelectionResources( bool getChildItems, 
            ResourceFilterCallback filter );

        /// <summary>
        /// Returns all  the SvnItem resources from root
        /// </summary>
        /// <param name="getChildItems">Whether children of the items in 
        /// question should be included.</param>
        /// <param name="filter">A callback used to filter the items
        /// that are added.</param>
        /// <returns>A list of SvnItem instances.</returns>
        IList GetAllResources( bool getChildItems, 
            ResourceFilterCallback filter );
    }
}
