// $Id$
using System;

namespace NSvn
{
    /// <summary>
    /// Represents an unversioned file in a working copy.
    /// </summary>
    [Obsolete("Will be removed in a future version")]
    public class UnversionedFile : UnversionedResource
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">File system path to the item.</param>
        public UnversionedFile( string path ) : base( path )
        {
            // empty
        }

        public override bool IsDirectory
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return false; }
        }

        /// <summary>
        /// Accepts an ILocalResourceVisitor.
        /// </summary>
        public override void Accept( ILocalResourceVisitor visitor )
        {
            visitor.VisitUnversionedFile( this );
        }
    }
}
