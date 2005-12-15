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
        /// Make sure all paths are on the same form, lower case and rooted.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizePath( string path )
        {
            return NormalizePath( path, Environment.CurrentDirectory );           
        }

        /// <summary>
        /// Make sure all paths are on the same form, lower case and rooted.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizePath( string path, string basepath )
        {
            string normPath = path.Replace( "/", "\\" );
            if ( !Path.IsPathRooted( normPath ) )
                normPath = Path.Combine( basepath, normPath );

            if ( normPath[normPath.Length-1] == '\\' )
                normPath = normPath.Substring(0, normPath.Length-1);

            return normPath.ToLower();
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


        /// <summary>
        /// Recursively deletes a directory.
        /// </summary>
        /// <param name="path"></param>
        public static void RecursiveDelete( string path )
        {
            foreach( string dir in Directory.GetDirectories( path ) )
            {
                RecursiveDelete( dir );
            }

            foreach( string file in Directory.GetFiles( path ) )
                File.SetAttributes( file, FileAttributes.Normal );

            File.SetAttributes( path, FileAttributes.Normal );
            Directory.Delete( path, true );
        }

        /// <summary>
        /// Retrieves the parent directory of a path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The parent directory.</returns>
        public static string GetParent( string path )
        {
            path = PathUtils.StripTrailingSlash( path );            
            return Path.GetDirectoryName( path );
        }

        /// <summary>
        /// Get the name of the item(file or folder).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetName( string path )
        {
            path = PathUtils.StripTrailingSlash( path );
            return Path.GetFileName( path );
        }

        /// <summary>
        /// Strips the trailing slash off a path, if present.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string StripTrailingSlash( string path )
        {
            if ( path.EndsWith( Path.DirectorySeparatorChar.ToString() ) )
                path = path.Substring( 0, path.Length-1 );
            return path;
        }

	}
}
