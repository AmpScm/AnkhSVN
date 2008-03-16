using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Selection
{
    /// <summary>
    /// An <see cref="SvnProject"/> instance is a black box reference to a project
    /// </summary>
    /// <remarks>
    /// <para>Normally you only use <see cref="SvnProject"/> instances to pass between the <see cref="ISelectionContext"/>, 
    /// <see cref="ISccHierarchyWalker"/> and <see cref="IProjectFileMapper"/> services</para>
    /// <para>The SvnProject contains a <see cref="path"/> and a <see cref="rawhandle"/> which can both be null but not both at the same time</para>
    /// <para>FullPath = null in case of a solution only-project (E.g. website project)</para>
    /// <para>RawHandle = null when retrieved from the selectionprovider when the file is not in a project (E.g. solution folder)</para>
    /// </remarks>
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
            if (string.IsNullOrEmpty(fullPath) && rawHandle == null)
                throw new ArgumentNullException("fullPath");

            // Current implementation details (which might change)

            // fullpath or rawHandle must be non-null

            // rawHandle = a IVsSccProject2 instance
            // fullPath = a file in the

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
