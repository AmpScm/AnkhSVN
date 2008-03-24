// $Id$
using System;
using System.Collections;
using EnvDTE;

namespace Ankh
{
    public interface IAnkhSelectionContainer
    {
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
        /// Returns the SvnItem resources associated with the selected items
        /// in the solution explorer.
        /// </summary>
        /// <param name="getChildItems">Whether children of the items in 
        /// question should be included.</param>
        /// <param name="filter">A callback used to filter the items
        /// that are added.</param>
        /// <returns>A list of SvnItem instances.</returns>
        IList GetSelectionResources( bool getChildItems, 
            Predicate<SvnItem> filter );

        /// <summary>
        /// Returns all  the SvnItem resources from root
        /// </summary>
        /// <param name="filter">A callback used to filter the items
        /// that are added.</param>
        /// <returns>A list of SvnItem instances.</returns>
        IList GetAllResources( Predicate<SvnItem> filter );
    }
}
