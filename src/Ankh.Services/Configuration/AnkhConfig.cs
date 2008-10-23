using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using Microsoft.Win32;
using System.Security.AccessControl;
namespace Ankh.Configuration
{
    /// <summary>
    /// Ankh configuration container. Read and written via <see cref="IAnkhConfigurationService"/>
    /// </summary>
    public class AnkhConfig
    {
        string _mergeExePathField;
        string _diffExePathField;
        bool _interactiveMergeOnConflict;

        /// <summary>
        /// Gets or sets the merge exe path.
        /// </summary>
        /// <value>The merge exe path.</value>
        [DefaultValue(null)]
        public string MergeExePath
        {
            get { return _mergeExePathField; }
            set { _mergeExePathField = value; }
        }

        /// <summary>
        /// Gets or sets the diff exe path.
        /// </summary>
        /// <value>The diff exe path.</value>
        [DefaultValue(null)]
        public string DiffExePath
        {
            get { return _diffExePathField; }
            set { _diffExePathField = value; }
        }

        [DefaultValue(false)]
        public bool InteractiveMergeOnConflict
        {
            get { return _interactiveMergeOnConflict; }
            set { _interactiveMergeOnConflict = value; }
        }
    }
}