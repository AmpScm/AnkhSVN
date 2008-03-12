using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Selection
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>All members are based on IEnumerable to be able to delayload all cases required by commands. They return lists when completed
    /// but might return just an IEnumerable creating a list while not complete (e.g. we might traverse all files for a complete result,
    /// while we need only 1 un-added one)</para>
    /// 
    /// <para>A simple test implementation could just return arrays</para>
    /// </remarks>
    public interface ISelectionContext
    {
        /// <summary>
        /// Gets a list of the currently direct selected files 
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetSelectedFiles();
        /// <summary>
        /// Gets a list of the currently selected files
        /// </summary>
        /// <param name="recursive"><c>true</c> to select all descendants of selected nodes too</param>
        /// <returns></returns>
        IEnumerable<string> GetSelectedFiles(bool recursive);
        /// <summary>
        /// Gets a list of the currently selected <see cref="SvnItem"/> instances, mapped via their path. See <see cref="GetSelectedFiles()"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<SvnItem> GetSelectedSvnItems();
        /// <summary>
        /// Gets a list of the currently selected <see cref="SvnItem"/> instances, mapped via their path. See <see cref="GetSelectedFiles(Boolean)"/>
        /// </summary>
        /// <param name="recursive"></param>
        /// <returns></returns>
        IEnumerable<SvnItem> GetSelectedSvnItems(bool recursive);

        /// <summary>
        /// Gets the projects owning selected files
        /// </summary>
        /// <returns></returns>
        IEnumerable<SvnProject> GetOwnerProjects();
        /// <summary>
        /// Gets the projects owning selected files
        /// </summary>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        IEnumerable<SvnProject> GetOwnerProjects(bool recursive);

        /// <summary>
        /// Gets the current solution filename (full path)
        /// </summary>
        /// <value>The solution filename.</value>
        string SolutionFilename { get; }
    }
}
