using System;

namespace NSvn
{
	/// <summary>
	/// Represents a versioned file in an SVN working copy.
	/// </summary>
	public class WorkingCopyFile : WorkingCopyItem
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path to the file.</param>
		public WorkingCopyFile( string path ) : base( path )
		{
			// empty for now
		}

        /// <summary>
        /// Is this a folder?
        /// </summary>
        public override bool IsDirectory
        {
            get{ return false; }
        }        
	}
}
