using System;
using System.IO;
using NSvn.Core;
using NSvn.Common;

namespace NSvn
{
	/// <summary>
	/// Represents an unversioned item in a working copy.
	/// </summary>
	public abstract class UnversionedResource : SvnResource, ILocalResource
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path to the item.</param>
		protected UnversionedResource( string path )
		{
			this.path = System.IO.Path.GetFullPath(path);
		}

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="path">Path to the resource.</param>
        /// <returns>An UnversionedItem object representing the resource.</returns>
        public static UnversionedResource FromPath( string path )
        {
            if ( System.IO.File.Exists( path ) )
                return new UnversionedFile( path );
            else if ( System.IO.Directory.Exists( path ) )
                return new UnversionedDirectory( path );
            else
                throw new ArgumentException( "Path must be a file or a directory", "path" );  
        }

        /// <summary>
        /// Adds the item to version control.
        /// </summary>
        /// <param name="recursive">Whether subitems should be recursively added.</param>
        /// <returns>A WorkingCopyItem object representing the added resource.</returns>
        public WorkingCopyResource Add( bool recursive )
        {
            Client.Add( this.Path, recursive, this.ClientContext );
            return WorkingCopyResource.FromPath( this.Path );            
        }

        /// <summary>
        /// Add a filesystem path to version control.
        /// </summary>
        /// <param name="path">The file system path to the item.</param>
        /// <param name="recursive">Whether subitems should be recursively added.</param>
        /// <returns>A WorkingCopyItem object pointing to the added path</returns>
        public static WorkingCopyResource Add( string path, bool recursive )
        {
            return FromPath( path ).Add( recursive );     
        }        

        /// <summary>
        /// The path to the item.
        /// </summary>
        public string Path
        {
            get{ return path; }
        }

        /// <summary>
        /// Is this a versioned resource?
        /// </summary>
        public bool IsVersioned
        {
            get{ return false; }
        }

        /// <summary>
        /// Is this a directory?
        /// </summary>
        public abstract bool IsDirectory
        { 
            get;
        }       


        private string path;
	}
}
