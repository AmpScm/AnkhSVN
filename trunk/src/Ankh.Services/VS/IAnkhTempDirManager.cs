using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.VS
{
    public interface IAnkhTempDirManager
    {
        /// <summary>
        /// Gets a temporary directory
        /// </summary>
        /// <returns></returns>
        /// <remarks>The directory is created.</remarks>
        string GetTempDir();
    }
}
