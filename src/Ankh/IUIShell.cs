// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;

namespace Ankh
{
	/// <summary>
	/// Represents the UI of the addin.
	/// </summary>
	public interface IUIShell
	{
       
        /// <summary>
        /// The repository explorer UI.
        /// </summary>
        RepositoryExplorerControl RepositoryExplorer
        {
            get; 
        }

        /// <summary>
        /// An IContext.
        /// </summary>
        IContext Context
        {
            get;
            set;
        }

        /// <summary>
        /// Ask the user whether Ankh should load for a given solution.
        /// </summary>
        /// <returns></returns>
        DialogResult QueryWhetherAnkhShouldLoad();

        /// <summary>
        /// Whether to show the repository explorer tool window.
        /// </summary>
        /// <param name="show"></param>
        void ShowRepositoryExplorer(bool show);

        /// <summary>
        /// Set the selection for the repository explorer.
        /// </summary>
        /// <param name="selection"></param>
        void SetRepositoryExplorerSelection( object[] selection );
	}
}
