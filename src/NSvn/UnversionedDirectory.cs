using System;

namespace NSvn
{
	/// <summary>
	/// Represents an unversioned directory in a working copy.
	/// </summary>
	public class UnversionedDirectory : UnversionedItem
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">File system path to the item.</param>
		public UnversionedDirectory( string path ) : base( path )
		{
			//empty
		}
	}
}
