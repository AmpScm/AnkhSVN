using System;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NSvn.Core.Tests
{
	/// <summary>
	/// Summary description for TestBase.
	/// </summary>
	public class TestBase
	{
        /// <summary>
        /// extract our test repository
        /// </summary>
        public void ExtractRepos( )
        {
            //already exists?
            this.reposPath = Path.Combine( BASEPATH, REPOS_NAME );
            if ( Directory.Exists( this.reposPath ) )
                Directory.Delete( this.reposPath, true );

            ExtractToTempPath(this.reposPath, REPOS_FILE );
            this.reposUrl = "file://" + 
                this.reposPath.Replace( "\\", "/" );


        }

        

        /// <summary>
        /// Extract our test working copy
        /// </summary>
        public void ExtractWorkingCopy( )
        {
            this.wcPath = this.FindDirName( Path.Combine( BASEPATH, WC_NAME ) );
            ExtractToTempPath( this.wcPath, WC_FILE );        

        }

        

        /// <summary>
        /// The fully qualified URI to the repository
        /// </summary>
        public string ReposUrl
        {
            get{ return this.reposUrl; }
        }

        /// <summary>
        /// The path to the repository
        /// </summary>
        public string ReposPath
        {
            get{ return this.reposPath; }
        }

        /// <summary>
        /// The path to the working copy
        /// </summary>
        public string WcPath
        {
            get{ return this.wcPath; }
        }

        private void ExtractToTempPath( string destinationPath, string resource )
        {
            Directory.CreateDirectory( destinationPath );
            
            //extract to the temp folder
            Stream zipStream = this.GetType().Assembly.GetManifestResourceStream( resource );
            this.ExtractZipStream( zipStream, destinationPath );
        }

       

        /// <summary>
        /// Extract a zip file
        /// </summary>
        /// <param name="zipFile">Path to the zip file</param>
        /// <param name="parentPath">Path to the folder in which we want to extract the
        /// zip file</param>
        private void ExtractZipStream( Stream zipStream, string parentPath )
        {
            ZipFile zip = new ZipFile( zipStream );

            //Go through all the entries in the zip file
            foreach( ZipEntry entry in zip )
            {
                string destPath = Path.Combine( parentPath, entry.Name );

                //t'is a directory?
                if ( entry.IsDirectory )
                    Directory.CreateDirectory( destPath );
                else
                {  
                    // nope - this is a file
                    // EXTRACT IT!

                    //make sure the parent path exists
                    string parent = Path.GetDirectoryName( destPath );
                    if ( !Directory.Exists( parent ) )
                        Directory.CreateDirectory( parent );

                    Stream instream = zip.GetInputStream( entry );
                    using( Stream outstream = new FileStream( destPath, FileMode.Create ) )
                    {
                        byte[] buffer = new byte[ BUF_SIZE ];
                        int count = 0;
                        do
                        {
                            count = instream.Read( buffer, 0, BUF_SIZE );
                            if( count > 0 )
                                outstream.Write( buffer, 0, count );
                        } while( count > 0 );
                    } // using
                } // else
            } // foreach
        }

        /// <summary>
        /// generate a unique directory name
        /// </summary>
        /// <param name="baseName"></param>
        /// <returns></returns>
        private string FindDirName( string baseName )
        {
            string dir = baseName;
            int i = 1;
            while ( Directory.Exists( dir ) )
            {
                dir = string.Format( "{0}-{1}", baseName, i );
                ++i;
            }

            return dir;
        }
        
       

        private static void Main()
        {
            TestBase test = new TestBase();
            test.ExtractRepos();

            Console.WriteLine( "repos url: {0}", test.ReposUrl );

            Process p = Process.Start( "svn", string.Format( "ls {0}", test.ReposUrl ) );
            p.WaitForExit();
            Debug.Assert( p.ExitCode == 0, "svn lsexit code not 0" );
            
            

            test.ExtractWorkingCopy();

            Console.WriteLine( "working copy path: {0}", test.WcPath );
            p = Process.Start( "svn", string.Format( "status {0}", test.WcPath ) );
            p.WaitForExit();
            Debug.Assert( p.ExitCode == 0, "svn status exit code not 0");
            
            
        }
        private const int BUF_SIZE = 4096;
        private const string REPOS_FILE="NSvn.Core.Tests.repos.zip";
        private const string REPOS_NAME = "repos";
        private const string BASEPATH = @"\tmp";
        private const string WC_FILE = "NSvn.Core.Tests.wc.zip";
        private const string WC_NAME = "wc";
        private string reposUrl;
        private string wcPath;
        private string reposPath;
	}
}
