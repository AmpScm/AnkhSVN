using System;
using System.IO;

namespace NSvn
{
	/// <summary>
	/// Represents a versioned file in an SVN working copy.
	/// </summary>
	public class WorkingCopyFile : WorkingCopyResource
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path to the file.</param>
		public WorkingCopyFile( string path ) : base( path )
		{
			this.lastModTime = File.GetLastWriteTime( this.Path );
		}

        /// <summary>
        /// Is this a folder?
        /// </summary>
        public override bool IsDirectory
        {
            get{ return false; }
        }   

        /// <summary>
        /// Whether this resource has been modified
        /// </summary>
        protected override bool IsModified
        {
            get
            { 
                return File.GetLastWriteTime( this.Path ) > this.lastModTime; 
            }
        }

        /// <summary>
        /// Invalidates instance data associated with this file.
        /// </summary>
        protected override void DoInvalidate()
        {
            this.lastModTime = File.GetLastWriteTime( this.Path );
        }
     
        private DateTime lastModTime;
	}
}
