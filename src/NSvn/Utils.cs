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

        /// <summary>
        /// Determines whether a given path is a working copy.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the path is/is in a working copy.</returns>
        public static bool IsWorkingCopyPath( string path )
        {
            string dir;
            if ( Directory.Exists( path ) )
                dir = path;
            else
                dir = Path.GetDirectoryName( path );
            string wcDir = Path.Combine( dir, WC_ADMIN_AREA );
            return Directory.Exists( wcDir );
        }

        public const string WC_ADMIN_AREA = ".svn";
    }
}
