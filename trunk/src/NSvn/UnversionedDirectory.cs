using System;

namespace NSvn
{
	/// <summary>
	/// Represents an unversioned directory in a working copy.
	/// </summary>
	public class UnversionedDirectory : UnversionedResource
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">File system path to the item.</param>
		public UnversionedDirectory( string path ) : base( path )
		{
			//empty
		}

        /// <summary>
        /// Is this a directory?
        /// </summary>
        public override bool IsDirectory
        {
            get{ return true; }
        }

        /// <summary>
        /// Accepts an ILocalResourceVisitor.
        /// </summary>
        public override void Accept( ILocalResourceVisitor visitor )
        {
            visitor.VisitUnversionedDirectory( this );
        }
	}
}
