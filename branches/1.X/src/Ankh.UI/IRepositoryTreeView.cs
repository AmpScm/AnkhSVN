using System;

namespace Ankh.UI
{
	/// <summary>
	/// Defines the interface exposed by the repository tree view control.
	/// </summary>
	public interface IRepositoryTreeView
	{
        /// <summary>
        /// Returns the number of selected items.
        /// </summary>
        int SelectionCount
        {
            get;
        }

        /// <summary>
        /// Returns the selected nodes.
        /// </summary>
        IRepositoryTreeNode[] SelectedNodes
        {
            get;
        }
	}
}
