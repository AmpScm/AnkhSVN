using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;

namespace Ankh.Scc
{
	/// <summary>
	/// 
	/// </summary>
	public interface IAnkhSccService
	{
		/// <summary>
		/// Gets a value indicating whether the Ankh Scc service is active
		/// </summary>
		/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
		bool IsActive { get; }


        /// <summary>
        /// Gets or sets a boolean indicating whether te solution should be saved for changed scc settings
        /// </summary>
        bool IsSolutionDirty { get; set; }

        /// <summary>
        /// Called by the package when loading a managed solution
        /// </summary>
        /// <param name="asPrimarySccProvider">if set to <c>true</c> Ankh is marked as the primary SCC provider; otherwise it is running as second chair</param>
        void LoadingManagedSolution(bool asPrimarySccProvider);

        /// <summary>
        /// Marks the specified project as managed by the Scc provider
        /// </summary>
        /// <param name="project">A reference to the project or null for the solution</param>
        /// <param name="managed"></param>
        void SetProjectManaged(SvnProject project, bool managed);

        /// <summary>
        /// Marks the specified project as managed by the Scc provider
        /// </summary>
        /// <param name="project">A reference to the project or null for the solution</param>
        /// <param name="managed"></param>
        void SetProjectManagedRaw(object project, bool managed);

        /// <summary>
        /// Gets a boolean indicating whether the specified project (or the solution) is 
        /// managed by the Subversion Scc provider
        /// </summary>
        /// <param name="project">A reference to the project or null for the solution</param>
        /// <returns><c>true</c> if the solution is managed by the scc provider, otherwise <c>false</c></returns>
        bool IsProjectManaged(SvnProject project);

        /// <summary>
        /// Gets a boolean indicating whether the specified project (or the solution) is 
        /// managed by the Subversion Scc provider
        /// </summary>
        /// <param name="project">A reference to the project or null for the solution</param>
        /// <returns><c>true</c> if the solution is managed by the scc provider, otherwise <c>false</c></returns>
        bool IsProjectManagedRaw(object project);

        /// <summary>
        /// Register the scc provider as primary scc provider in Visual Studio
        /// </summary>
        void RegisterAsPrimarySccProvider();
    }
}
