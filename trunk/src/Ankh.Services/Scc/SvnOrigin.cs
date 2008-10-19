using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    public class SvnOrigin
    {
        SvnTarget _target;
        Uri _uri;
        Uri _reposRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnOrigin"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="svnItem">The SVN item.</param>
        /// <param name="reposRoot">The repos root.</param>
        public SvnOrigin(IAnkhServiceProvider context, SvnItem svnItem, Uri reposRoot)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (svnItem == null)
                throw new ArgumentNullException("svnItem");
            else if (reposRoot == null)
                throw new ArgumentNullException("reposRoot");

            _target = new SvnPathTarget(svnItem.FullPath);
            _uri = svnItem.Status.Uri;
            _reposRoot = svnItem.WorkingCopy.RepositoryRoot;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnOrigin"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <param name="reposRoot">The repos root.</param>
        public SvnOrigin(IAnkhServiceProvider context, SvnTarget target, Uri reposRoot)
        {
        }

        /// <summary>
        /// Gets the repository root.
        /// </summary>
        /// <value>The repository root.</value>
        public Uri RepositoryRoot
        {
            get { return _reposRoot; }
        }

        /// <summary>
        /// Gets the repository URI of the item
        /// </summary>
        /// <value>The URI.</value>
        public Uri Uri
        {
            get { return _uri; }
        }

        /// <summary>
        /// Gets the target of the item
        /// </summary>
        /// <value>The target.</value>
        public SvnTarget Target
        {
            get { return _target; }
        }

    }
}
