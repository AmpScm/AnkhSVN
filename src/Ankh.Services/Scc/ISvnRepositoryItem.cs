using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.Scc
{
    public interface ISvnRepositoryItem
    {
        /// <summary>
        /// Gets the Uri of the item (Required)
        /// </summary>
        Uri Uri { get; }
        /// <summary>
        /// Gets the <see cref="SvnNodeKind"/> of the item (Optional)
        /// </summary>
        SvnNodeKind NodeKind { get; }
        /// <summary>
        /// Gets the <see cref="SvnRevision"/> of the item (Optional)
        /// </summary>
        SvnRevision Revision { get; }

        /// <summary>
        /// Gets the name of the item (its filename)
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Refreshes the item.
        /// </summary>
        void RefreshItem(bool refreshParent);

        /// <summary>
        /// Gets whether this item's URI points to an actual remote repository item.
        /// </summary>
        bool IsRepositoryItem { get; }

        /// <summary>
        /// Gets the URI to the root of the repository, or null when the node is a dummy.
        /// </summary>
        Uri RepositoryRoot { get; }
    }
}
