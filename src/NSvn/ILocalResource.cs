using System;

namespace NSvn
{
	/// <summary>
	/// TODO: doc-comment here
	/// </summary>
	public interface ILocalResource
	{
        /// <summary>
        /// The file system path to the item.
        /// </summary>
        string Path
        { 
            get;
        }

        /// <summary>
        /// Is this a directory?
        /// </summary>
        bool IsDirectory
        {
            get;
        }

        /// <summary>
        /// Is this a versioned item?
        /// </summary>
        bool IsVersioned
        {
            get;
        }
	}
}
