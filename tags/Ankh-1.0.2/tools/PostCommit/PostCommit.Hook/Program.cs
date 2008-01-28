using System;
using System.Collections.Generic;
using System.Text;
using PostCommit.Remoting;
using System.Diagnostics;
using System.IO;
using System.Messaging;

namespace PostCommit.Hook
{
    class Program
    {
        static int Main( string[] args )
        {
            if ( args.Length != 3 )
            {
                Usage();
                return 1;
            }

            string repos = args[0];
            string revisionString = args[1];
            string queueName = args[2];

            try
            {
                // get the info we want
                string author = SvnLook( "author", repos, revisionString ).Trim();
                string logMessage = SvnLook( "log", repos, revisionString ).Trim();
                string changedPathsString = SvnLook( "changed", repos, revisionString ).Trim();
                string changedDirsString = SvnLook( "dirs-changed", repos, revisionString ).Trim();

                // convert the path strings to arrays
                string[] changedPaths = changedPathsString.Split( '\n' );
                string[] changedDirs = changedDirsString.Split( '\n' );
                changedPaths = Array.ConvertAll<string, string>( 
                    changedPaths, delegate( string s ) { return s.Trim(); } );
                changedDirs = Array.ConvertAll<string, string>( 
                    changedDirs, delegate( string s ) { return s.Trim(); } );

                // create a Commit object to send as a message
                int revision = int.Parse( revisionString );
                Commit commit = new Commit( author, revision, logMessage, changedPaths, changedDirs );

                // create our message queue
                MessageQueue queue;
                if ( !MessageQueue.Exists( queueName ) )
                    queue = MessageQueue.Create( queueName );
                else
                    queue = new MessageQueue( queueName );

                // ship it off
                queue.Send( commit );                
            }

            catch ( Exception ex )
            {
                Console.Error.WriteLine( ex );
                return 1;
            }

            return 0;
        }

        private static void Usage()
        {
            Console.Error.WriteLine( "Usage: PostCommit.Hook REPOSPATH REVISION QUEUENAME" );
        }

        private static string SvnLook( string command, string repos, string revision )
        {
            ProcessStartInfo psi = new ProcessStartInfo( "svnlook",
                String.Format( "{0} {1} --revision {2}", command, repos, revision ) );
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;

            Process process = new Process();
            process.StartInfo = psi;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit( TimeOut );

            return output;
        }

        private const int TimeOut = 10000;
    }
}
