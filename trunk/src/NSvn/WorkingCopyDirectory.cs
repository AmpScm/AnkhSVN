using System;
using NSvn.Common;
using NSvn.Core;
using System.IO;

namespace NSvn
{
	/// <summary>
	/// Represents a versioned directory in an SVN working copy.
	/// </summary>
	public class WorkingCopyDirectory : WorkingCopyResource
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

        /// <summary>
        /// Accepts an ILocalResourceVisitor.
        /// </summary>
        public override void Accept( ILocalResourceVisitor visitor )
        {
            visitor.VisitWorkingCopyDirectory( this );
        }

        /// <summary>
        /// Returns all the child objects of this resource.
        /// </summary>
        public LocalResourceDictionary Children
        {
            get
            {
                this.CheckForModifications();
                if ( this.children == null )
                    this.children = this.GetChildren();
                return this.children;
            }
        }

        /// <summary>
        /// Whether the resource has been modified.
        /// </summary>
        protected override DateTime LastWriteTime
        {
            get
            {
                return Directory.GetLastWriteTime( this.Path );
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
                return Directory.GetLastWriteTime( System.IO.Path.Combine(
                    this.Path, ADMIN_AREA ) );
            }
        }

        /// <summary>
        /// Invalidates instance data of this resource object.
        /// </summary>
        protected override void DoInvalidate()
        {
            this.children = null; 
        }


        /// <summary>
        /// Retrieve the children of this directory.
        /// </summary>
        /// <returns>A LocalResourceDictionary object containing all the children.</returns>
        private LocalResourceDictionary GetChildren()
        {
            LocalResourceDictionary resDict = new LocalResourceDictionary();
            int youngest;

            // get the status of all subitems - this will effectively give us a list of them
            // and and we get their statuses as an added bonus
            // TODO: should this be recursive?
            StatusDictionary statusDict = Client.Status( out youngest, this.Path, false, true, false, 
                false, this.ClientContext );

            // delete the root directory - we dont want it in the returned dict
            statusDict.Remove( this.Path );

            // go through all the returned resources
            foreach( string path in statusDict.Keys )
            {
                string basePath = System.IO.Path.GetFileName( path );

                // is this a versioned or an unversioned resource?
                if ( statusDict.Get(path).TextStatus == StatusKind.Unversioned )
                    resDict[ basePath ] = UnversionedResource.FromPath( path );
                else
                    resDict[ basePath ] = WorkingCopyResource.FromPath( path, statusDict.Get(path) );
            }

            return resDict;
        }

        private LocalResourceDictionary children;    
	}
}
