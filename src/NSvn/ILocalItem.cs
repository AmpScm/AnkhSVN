using System;

namespace NSvn
{
	/// <summary>
	/// TODO: doc-comment here
	/// </summary>
	public interface ILocalItem
	{
        /// <summary>
        /// The file system path to the item.
        /// </summary>
        string Path
        { 
            get;
        }
	}
}
