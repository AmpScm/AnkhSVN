using System;

namespace NSvn
{
	/// <summary>
	/// Represents an unversioned file in a working copy.
	/// </summary>
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
