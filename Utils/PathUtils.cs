using System;
using System.IO;

namespace Utils
{
	/// <summary>
	/// Contains utility functions for path manipulations.
	/// </summary>
	public class PathUtils
	{
        private PathUtils()
        {
            // nothing here
        }

        /// <summary>
        /// Determines whether path is in the tree under directory.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool IsSubPathOf( string path, string directory )
        {
            if ( !Path.IsPathRooted( path ) )
                path = Path.GetFullPath( path );

            if ( !Path.IsPathRooted( directory ) )
                directory = Path.GetFullPath( directory );

            path = path.ToLower();
            directory = directory.ToLower();

            if ( path.IndexOf( directory ) != 0 )
                return false;
            else 
                return true;
        }
	}
}
