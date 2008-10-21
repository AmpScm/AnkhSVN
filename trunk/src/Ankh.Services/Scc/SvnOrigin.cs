using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.Diagnostics;

namespace Ankh.Scc
{
    /// <summary>
    /// Container of a <see cref="SvnTarget"/>, its repository Uri and its repository root.
    /// </summary>
    public class SvnOrigin
    {
        SvnTarget _target;
        Uri _uri;
        Uri _reposRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnOrigin"/> class using a SvnItem
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="svnItem">The SVN item.</param>
        public SvnOrigin(SvnItem svnItem)
        {
            if (svnItem == null)
                throw new ArgumentNullException("svnItem");

            if (!svnItem.IsVersioned)
                throw new InvalidOperationException("Can only create a SvnOrigin from versioned items");

            _target = new SvnPathTarget(svnItem.FullPath);
            _uri = svnItem.Status.Uri;
            _reposRoot = svnItem.WorkingCopy.RepositoryRoot;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnOrigin"/> class.
        /// </summary>
        /// <param name="uriTarget">The URI target.</param>
        /// <param name="reposRoot">The repos root.</param>
        public SvnOrigin(SvnUriTarget uriTarget, Uri reposRoot)
        {
            if (uriTarget == null)
                throw new ArgumentNullException("uriTarget");
            else if (reposRoot == null)
                throw new ArgumentNullException("reposRoot");

            _target = uriTarget;
            _uri = uriTarget.Uri;
            _reposRoot = reposRoot;
#if DEBUG
            Debug.Assert(!_reposRoot.MakeRelativeUri(_uri).IsAbsoluteUri);
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnOrigin"/> class from a SvnTarget
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <param name="reposRoot">The repos root or <c>null</c> to retrieve the repository root from target</param>
        public SvnOrigin(IAnkhServiceProvider context, SvnTarget target, Uri reposRoot)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if(target == null)
                throw new ArgumentNullException("target");

            SvnPathTarget pt = target as SvnPathTarget;

            if (pt != null)
            {
                SvnItem item = context.GetService<IFileStatusCache>()[pt.FullPath];

                if(item == null || !item.IsVersioned)
                    throw new InvalidOperationException("Can only create a SvnOrigin from versioned items");

                _target = target;
                _uri = item.Status.Uri;
                _reposRoot = item.WorkingCopy.RepositoryRoot; // BH: Prefer the actual root over the provided
                return;
            }

            SvnUriTarget ut = target as SvnUriTarget;

            if (ut != null)
            {
                _target = ut;
                _uri = ut.Uri;
                if (reposRoot != null)
                    _reposRoot = reposRoot;
                else
                {
                    using (SvnClient client = context.GetService<ISvnClientPool>().GetClient())
                    {
                        _reposRoot = client.GetRepositoryRoot(ut.Uri);

                        if (_reposRoot != null)
                            throw new InvalidOperationException("Can't retrieve the repository root of the UriTarget");

#if DEBUG
                        Debug.Assert(!_reposRoot.MakeRelativeUri(_uri).IsAbsoluteUri);
#endif
                    }
                }

                return;
            }

            throw new InvalidOperationException("Invalid target type");
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
