using System;

namespace NSvn
{
	/// <summary>
	/// Represents an unversioned file in a working copy.
	/// </summary>
	public class UnversionedFile : UnversionedItem
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">File system path to the item.</param>
		public UnversionedFile( string path ) : base( path )
		{
			// empty
		}
	}
}
