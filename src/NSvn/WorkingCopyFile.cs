using System;

namespace NSvn
{
	/// <summary>
	/// Represents a versioned file in an SVN working copy.
	/// </summary>
	public class WorkingCopyFile : WorkingCopyItem, ILocalItem
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path to the file.</param>
		public WorkingCopyFile( string path ) : base( path )
		{
			// empty for now
		}
	}
}
