using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;

namespace Ankh.Scc
{
    public interface IProjectFileMapper
    {
        /// <summary>
        /// Gets an IEnumerable over all projects containing <paramref name="path"/>
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        IEnumerable<SvnProject> GetAllProjectsContaining(string path);
        /// <summary>
        /// Gets an IEnumerable over all projects containing one or more of the specified <paramref name="paths"/>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        IEnumerable<SvnProject> GetAllProjectsContaining(ICollection<string> paths);

        /// <summary>
        /// Gets a list of all files contained within <paramref name="project"/>
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        IEnumerable<string> GetAllFilesOf(SvnProject project);
        /// <summary>
        /// Gets a list of all files contained within the list of <paramref name="projects"/>
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        IEnumerable<string> GetAllFilesOf(ICollection<SvnProject> projects);

        /// <summary>
        /// Gets all files of all projects.
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetAllFilesOfAllProjects();

        SvnProject ResolveRawProject(SvnProject project);
    }
}
