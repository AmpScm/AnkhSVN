using System;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Diagnostics;

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
            string reposDir = this.FindDirName( Path.Combine( Path.GetTempPath(), REPOS_NAME ) );
            Directory.CreateDirectory( reposDir );
            
            //extract to the temp folder
            Stream zipStream = this.GetType().Assembly.GetManifestResourceStream( REPOS_FILE );
            this.ExtractZipStream( zipStream, reposDir );
            this.reposUrl = "file:///" + 
                reposDir.Replace( "\\", "/" );


        }

        /// <summary>
        /// Extract our test working copy
        /// </summary>
        public void ExtractWorkingCopy( )
        {
            this.wcPath = this.FindDirName( Path.Combine( Path.GetTempPath(), WC_NAME ) );

            ProcessStartInfo pi = new ProcessStartInfo( "svn.exe", string.Format( "co {0} {1}", 
                this.reposUrl, this.wcPath ) );
            pi.CreateNoWindow = true;
            pi.RedirectStandardError = true;
            pi.UseShellExecute = false;

            Process proc = Process.Start( pi );
            proc.WaitForExit();
            
            if( proc.ExitCode != 0 )
            {
                using( StreamReader reader = proc.StandardError )
                    Console.WriteLine( reader.ReadToEnd() );
                throw new Exception();
            }
    

            int a = 42;
            int b = a +2;

        }

        

        /// <summary>
        /// The fully qualified URI to the repository
        /// </summary>
        public string ReposUrl
        {
            get{ return this.reposUrl; }
        }

        /// <summary>
        /// The path to the working copy
        /// </summary>
        public string WcPath
        {
            get{ return this.wcPath; }
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
            int i = 1;
            string dir = baseName;
            while( Directory.Exists( dir ) )
            {
                dir = baseName + string.Format( "-{0}", i );
                i++;
            }

            return dir;
        }

        

        private static void Main()
        {
            TestBase test = new TestBase();
            test.ExtractRepos();
            
            Console.WriteLine( "repos url: {0}", test.ReposUrl );

            test.ExtractWorkingCopy();
            Debug.Assert( Directory.Exists( Path.Combine( Path.GetTempPath(), "wc" ) ) );
            Console.WriteLine( "working copy path: {0}", test.WcPath );
        }

        private const int BUF_SIZE = 4096;
        private const string REPOS_FILE="NSvn.Core.Tests.repos.zip";
        private const string REPOS_NAME = "repos";
        private const string WC_FILE = "NSvn.Core.Tests.wc.zip";
        private const string WC_NAME = "wc";
        private string reposUrl;
        private string wcPath;
	}
}
