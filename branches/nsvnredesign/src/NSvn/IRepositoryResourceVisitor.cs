// $Id$
using System;

namespace NSvn
{
    /// <summary>
    /// Defines a visitor for visiting RepositoryResource subclasses.
    /// </summary>
    [Obsolete("Will be removed in a future version")]
    public interface IRepositoryResourceVisitor
    {
        /// <summary>
        /// Visit a RepositoryDirectory object.
        /// </summary>
        /// <param name="directory"></param>
        void VisitDirectory( RepositoryDirectory directory );

        /// <summary>
        /// Visit a RepositoryFile object.
        /// </summary>
        /// <param name="file"></param>
        void VisitFile( RepositoryFile file );
    }
}
