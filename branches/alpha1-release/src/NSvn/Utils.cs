using System;
using System.IO;
using NSvn.Common;

namespace NSvn
{
    /// <summary>
    /// Contains utility methods for SVN
    /// </summary>
    public class Utils
    {
        private Utils()
        {
            // move along - nothing to see here
        } 

        /// <summary>
        /// Takes a full path and strips off all leading directories that are not
        /// working copies.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The top level working copy.</returns>
        public static string GetWorkingCopyRootedPath( string path )
        {
            if ( IsWorkingCopyPath( path ) && !IsWorkingCopyPath( GetParentDir( path ) ) )
                return Path.DirectorySeparatorChar.ToString();

            string retPath = path;
            int separator = retPath.IndexOf( Path.DirectorySeparatorChar );;
            while( true )
            {
                if ( separator == -1 )
                    throw new SvnException( "Path not part of a working copy" );

                string dir = path.Substring( 0, separator + 1 );                
                 
                // is our current path a working copy?               
                if ( IsWorkingCopyPath( dir ) )
                    break;

                // find the next subcomponent
                separator = path.IndexOf( Path.DirectorySeparatorChar, separator + 1 );
            }

            return path.Substring( separator, path.Length - separator );
        }

        public static string GetParentDir( string path )
        {
            if ( path.Length == 1 )
                return null;
            else return path[ path.Length - 1 ] == Path.DirectorySeparatorChar ?
                Path.GetDirectoryName( path.Substring( 0, path.Length - 1 ) ) :
                Path.GetDirectoryName( path );
        }

        /// <summary>
        /// Determines whether a given path is a working copy.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the path is/is in a working copy.</returns>
        public static bool IsWorkingCopyPath( string path )
        {
            //TODO: Re
            string baseDir;
            // is the path a directory or a file?
            if( Directory.Exists( path ) )
            {
                // is there a .svn dir under this directory?
                if ( Directory.Exists( Path.Combine( path, WC_ADMIN_AREA ) ) )
                    return true;
                else
                {
                    // is there a .svn dir under the directory above?
                    baseDir = GetParentDir( path );
                    if ( baseDir == null )
                        baseDir = path;
                    return Directory.Exists( Path.Combine( baseDir, WC_ADMIN_AREA ) );
                }
            }
            else if ( File.Exists( path ) )
            {
                // is there a .svn directory in the dir containing this file?
                baseDir = Path.GetDirectoryName( path );
                return Directory.Exists( Path.Combine( baseDir, WC_ADMIN_AREA ) );
            }
            else
                return false;
        }

        public const string WC_ADMIN_AREA = ".svn";
    }
}
