using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Selection
{
    /// <summary>
    /// 
    /// </summary>
    public class SvnProject
    {
        readonly string _fullPath;
        readonly object _rawHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnProjectItem"/> class.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="rawHandle">The raw handle.</param>
        public SvnProject(string fullPath, object rawHandle)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath");

            _fullPath = fullPath;
            _rawHandle = rawHandle;
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath
        {
            get { return _fullPath; }
        }

        /// <summary>
        /// Gets the raw handle.
        /// </summary>
        /// <value>The raw handle.</value>
        public object RawHandle
        {
            get { return _rawHandle; }
        }
    }
}
