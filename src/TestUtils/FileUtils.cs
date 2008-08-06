using System;
using System.Text;
using System.IO;

namespace Utils
{
    public static class FileUtils
    {
        /// <summary>
        /// Copies a directory recursively.
        /// </summary>
        /// <param name="source">The source directory. This must exist.</param>
        /// <param name="target">The target directory. This must not exist.</param>
        public static void CopyDirectory( string source, string target )
        {
            if ( !Directory.Exists(source) )
            {
                throw new ArgumentException( "Directory does not exist: " + source, "source" );
            }

            if ( Directory.Exists(target) )
            {
                throw new ArgumentException( "Directory already exists: " + target, "target" );
            }

            RecursivelyCopyDirectory( source, target );
        }

        private static void RecursivelyCopyDirectory( string source, string target )
        {
            Directory.CreateDirectory( target );

            foreach ( string file in Directory.GetFiles( source ) )
            {
                string fileName = Path.GetFileName( file );
                string sourcePath = Path.Combine( source, fileName );
                string destPath = Path.Combine( target, fileName );

                File.Copy( sourcePath, destPath );
            }

            foreach ( string dir in Directory.GetDirectories( source ) )
            {
                string dirName = Path.GetFileName( dir );
                string sourcePath = Path.Combine( source, dirName );
                string destPath = Path.Combine( target, dirName );

                RecursivelyCopyDirectory( sourcePath, destPath ); 
            }
        }
    }
}
