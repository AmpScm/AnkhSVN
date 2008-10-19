using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh
{
    public interface IAnkhProjectLayoutService
    {
        /// <summary>
        /// Gets all update roots of the current open solution
        /// </summary>
        /// <returns></returns>
        IEnumerable<SvnItem> GetUpdateRoots();
    }    
}
