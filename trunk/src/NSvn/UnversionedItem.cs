using System;
using System.IO;
using NSvn.Core;
using NSvn.Common;

namespace NSvn
{
	/// <summary>
	/// Represents an unversioned item in a working copy.
	/// </summary>
	public class UnversionedItem : SvnItem, ILocalItem
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path to the item.</param>
		protected UnversionedItem( string path )
		{
			this.path = System.IO.Path.GetFullPath(path);
		}

        /// <summary>
        /// Adds the item to version control.
        /// </summary>
        /// <param name="recursive">Whether subitems should be recursively added.</param>
        /// <returns>A WorkingCopyItem object representing the added resource.</returns>
        public WorkingCopyItem Add( bool recursive )
        {
            return UnversionedItem.Add( this.Path, recursive );
        }

        /// <summary>
        /// Add a filesystem path to version control.
        /// </summary>
        /// <param name="path">The file system path to the item.</param>
        /// <param name="recursive">Whether subitems should be recursively added.</param>
        /// <returns>A WorkingCopyItem object pointing to the added path</returns>
        public static WorkingCopyItem Add( string path, bool recursive )
        {
            // TODO: Figure out how to deal with ClientContext here
            Client.Add( path, recursive, new ClientContext() );
            if ( System.IO.File.Exists( path ) )
                return new WorkingCopyFile( path );
            else if ( System.IO.Directory.Exists( path ) )
                return new WorkingCopyDirectory( path );
            else
                throw new ArgumentException( "Path must be a file or a directory", "path" );
        }

        /// <summary>
        /// The path to the item.
        /// </summary>
        public string Path
        {
            get{ return path; }
        }


        private string path;
	}
}
