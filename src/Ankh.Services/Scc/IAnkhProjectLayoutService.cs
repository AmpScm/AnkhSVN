using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;

namespace Ankh
{
    public interface IAnkhProjectLayoutService
    {
        /// <summary>
        /// Gets all update roots of the current open solution
        /// </summary>
        /// <param name="project">The project specified or <c>null</c> to use all projects</param>
        /// <returns></returns>
        IEnumerable<SvnItem> GetUpdateRoots(SvnProject project);
    }    
}
