using System;

namespace NSvn
{
	/// <summary>
	/// Represents a versioned directory in an SVN working copy.
	/// </summary>
	public class WorkingCopyDirectory : WorkingCopyItem
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
		public WorkingCopyDirectory( string path ) : base( path )
		{
			
		}

        /// <summary>
        /// Is this a directory?
        /// </summary>
        public override bool IsDirectory
        {
            get{ return true; }
        }
	}
}
