// $Id$
using System;
using System.IO;
using NSvn.Core;

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
            // empty
        }

         /// <summary>
        /// Accepts an ILocalResourceVisitor.
        /// </summary>
        public override void Accept( ILocalResourceVisitor visitor )
        {
            visitor.VisitWorkingCopyFile( this );
        }

        public void Resolve()
        {
            Client.Resolve( this.Path, false, this.ClientContext );
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
        protected override DateTime LastWriteTime
        {
            get
            { 
                return File.GetLastWriteTime( this.Path );
            }
        }

        /// <summary>
        /// Returns the timestamp on the administrative directory of this
        /// resource.
        /// </summary>
        protected override DateTime AdminAreaWriteTime
        {
            get
            {
                string basePath = System.IO.Path.GetDirectoryName( this.Path );
                return Directory.GetLastWriteTime( System.IO.Path.Combine(
                    basePath, ADMIN_AREA ) );
            }
        }
     
    }
}
