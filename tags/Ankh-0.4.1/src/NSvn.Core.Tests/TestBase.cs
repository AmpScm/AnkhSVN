// $Id$
using System;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using NUnit.Framework;
using System.Collections;
using System.Threading;
using Utils;
using Utils.Win32;
using System.Text;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Serves as a base class for tests for NSvn::Core::Add
    /// </summary>
    [TestFixture]
    public class TestBase
    {
        public TestBase()
        {
            string asm = this.GetType().FullName;
            this.REPOS_FILE = asm.Substring(0, asm.LastIndexOf(".")) + ".repos.zip";
            this.WC_FILE = asm.Substring(0, asm.LastIndexOf(".")) + ".wc.zip";
        }

        [SetUp]
        public virtual void SetUp()
        {
            this.notifications = new ArrayList();
        }

        [TearDown]
        public virtual void TearDown()
        {
            // clean up
            try
            {
                if ( this.ReposPath != null )
                    Directory.Delete( this.ReposPath, true );
                if ( this.WcPath != null )
                    this.RecursiveDelete( this.wcPath );
            }
            catch( Exception )
            {
                // swallow 
            }
        }
        /// <summary>
        /// extract our test repository
        /// </summary>
        public void ExtractRepos( )
        {
            //already exists?
            this.reposPath = Path.Combine( BASEPATH, REPOS_NAME );
            if ( Directory.Exists( this.reposPath ) )
                Directory.Delete( this.reposPath, true );

            Zip.ExtractZipResource(this.reposPath, this.GetType(), REPOS_FILE );
            this.reposUrl = "file://" + 
                this.reposPath.Replace( "\\", "/" );
            if( this.reposUrl[ this.reposUrl.Length-1 ] != '/' )
                this.reposUrl = this.reposUrl + "/";


        }

        

        /// <summary>
        /// Extract our test working copy
        /// </summary>
        public void ExtractWorkingCopy( )
        {
            this.wcPath = this.FindDirName( Path.Combine( BASEPATH, WC_NAME ) );
            Zip.ExtractZipResource( this.wcPath, this.GetType(), WC_FILE );        

        }

        /// <summary>
        /// Determines the SVN status of a given path
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>Same character codes as used by svn st</returns>
        public char GetSvnStatus( string path )
        {   

            string output = this.RunCommand( "svn", "st \"" + path + "\"" );

            string regexString = String.Format( @"(\w).*\s{0}\s*", Regex.Escape(path) );
            Match match = Regex.Match( output, regexString );
            if ( match != Match.Empty )
                return match.Groups[1].ToString()[0];
            else 
                return '-';

        }

        /// <summary>
        /// Runs a command
        /// </summary>
        /// <param name="command">The command to run</param>
        /// <param name="args">Arguments to the command</param>
        /// <returns>The output from the command</returns>
        public string RunCommand( string command, string args )
        {
            Process proc = new Process();
            proc.StartInfo.FileName = command;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();

            ProcessReader outreader = new ProcessReader( proc.StandardOutput );
            ProcessReader errreader = new ProcessReader( proc.StandardError );
            outreader.Start();
            errreader.Start();

            proc.WaitForExit();

            outreader.Wait();
            errreader.Wait();

            if ( proc.ExitCode != 0 )
                throw new ApplicationException( "command exit code was " + 
                    proc.ExitCode.ToString() +
                    Environment.NewLine + errreader.Output + Environment.NewLine +
                    "Command was " + 
                    proc.StartInfo.FileName + " " + proc.StartInfo.Arguments );


            // normalize newlines
            string[] lines = Regex.Split( outreader.Output, @"\r?\n" );  
            return String.Join( Environment.NewLine, lines );
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

        /// <summary>
        /// The notifications generated during a call to Client::Add
        /// </summary>
        public Notification[] Notifications
        {
            get{ return (Notification[])this.notifications.ToArray( typeof(Notification) ); }
        }

        /// <summary>
        /// Callback method to be used as ClientContext.NotifyCallback
        /// </summary>
        /// <param name="notification">An object containing information about the notification</param>
        public virtual void NotifyCallback( Notification notification )
        {
            this.notifications.Add( notification );
        }

        /// <summary>
        /// Creates a textfile with the given name in the WC
        /// </summary>
        /// <param name="name">The name of the ifle to create</param>
        /// <returns>The path to the created text file</returns>
        protected string CreateTextFile( string name )
        {
            string path = Path.Combine( this.WcPath, name );
            using ( StreamWriter writer = File.CreateText( path ) )
                writer.Write( "Hello world" );

            return path;
        }

        

        

        protected void RecursiveDelete( string path )
        {
            foreach( string dir in Directory.GetDirectories( path ) )
            {
                this.RecursiveDelete( dir );
            }

            foreach( string file in Directory.GetFiles( path ) )
                File.SetAttributes( file, FileAttributes.Normal );

            File.SetAttributes( path, FileAttributes.Normal );
            Directory.Delete( path, true );
        }
       

        

        /// <summary>
        /// generate a unique directory name
        /// </summary>
        /// <param name="baseName"></param>
        /// <returns></returns>
        protected string FindDirName( string baseName )
        {
            string dir = baseName;
            int i = 1;
            while ( Directory.Exists( dir ) )
            {
                dir = string.Format( "{0}-{1}", baseName, i );
                ++i;
            }

            return Path.GetFullPath( dir );
        }

        protected string GetTempFile()
        {
            // ensure we get a long path
            StringBuilder builder = new StringBuilder( 260 );
            Win32.GetLongPathName( Path.GetTempFileName(), builder, 260 );
            string tmpPath = builder.ToString();
            File.Delete( tmpPath );

            return tmpPath;
        }


        
        
       

        //        private static void Main()
        //        {
        //            TestBase test = new TestBase();
        //            test.ExtractRepos();
        //
        //            Console.WriteLine( "repos url: {0}", test.ReposUrl );
        //
        //            Process p = Process.Start( "svn", string.Format( "ls {0}", test.ReposUrl ) );
        //            p.WaitForExit();
        //            Debug.Assert( p.ExitCode == 0, "svn ls exit code not 0" );
        //            
        //
        //            test.ExtractWorkingCopy();
        //
        //            Console.WriteLine( "working copy path: {0}", test.WcPath );
        //            p = Process.Start( "svn", string.Format( "status {0}", test.WcPath ) );
        //            p.WaitForExit();
        //            Debug.Assert( p.ExitCode == 0, "svn status exit code not 0");
        //            
        //            
        //        }
        protected readonly string REPOS_FILE;
        private const string REPOS_NAME = "repos";
        protected const string BASEPATH = @"\tmp";
        protected readonly string WC_FILE;
        protected const string WC_NAME = "wc";
        private string reposUrl;
        private string wcPath;
        private string reposPath;       
    
        
        protected ArrayList notifications;
    }
}
