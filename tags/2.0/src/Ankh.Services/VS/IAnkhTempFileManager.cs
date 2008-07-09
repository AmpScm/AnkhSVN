﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.VS
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAnkhTempFileManager
    {
        /// <summary>
        /// Gets a temporary file with the specified extension
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        /// <remarks>The file is created as a 0 byte unique file before this function returns
        /// and will be removed after AnkhSVN exits</remarks>
        string GetTempFile(string extension);
    }
}
