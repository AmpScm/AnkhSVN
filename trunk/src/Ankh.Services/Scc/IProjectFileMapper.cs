using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;

namespace Ankh.Scc
{
    public interface IProjectFileMapper
    {
        /// <summary>
        /// Gets an IEnumerable over all projects containing <paramref name="file"/>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        IEnumerable<SvnProject> GetProjectsContaining(string file);
        /// <summary>
        /// Gets an IEnumerable over all projects containing one or more of the specified <paramref name="files"/>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        IEnumerable<SvnProject> GetProjectsContaining(ICollection<string> files);

        /// <summary>
        /// Gets a list of all files contained within <paramref name="project"/>
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        IEnumerable<string> GetFiles(SvnProject project);
        /// <summary>
        /// Gets a list of all files contained within the list of <paramref name="projects"/>
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        IEnumerable<string> GetFiles(ICollection<SvnProject> projects);

        /// <summary>
        /// Resolves the <paramref name="project"/> to an optimized <see cref="SvnProject"/> reference
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        SvnProject Resolve(SvnProject project);
    }
}
