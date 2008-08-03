using System;
using System.Collections.Generic;
using System.Text;
using PostCommit.Remoting;

namespace AnkhBot
{
    class RepositoryService : RemotelyDelegatableObject, IService
    {
        public void Initialize( AnkhBot bot )
        {
            this.bot = bot;
            this.repository = (IRepository)Activator.GetObject(
                typeof( IRepository ), "tcp://localhost:9999/Repository" );

            this.repository.Committed += this.CommittedCallback;
        }

        protected override void InternalCommittedCallback( object sender, CommitEventArgs args )
        {
            string paths;
            int count = 0;

            // we can display all the paths, if 
            if ( args.Commit.ChangedPaths.Length < MaxPaths )
            {
                paths = String.Join( ", ", args.Commit.ChangedPaths );
            }
            else 
            {
                count = args.Commit.ChangedDirs.Length < MaxPaths ?
                    args.Commit.ChangedDirs.Length : MaxPaths;
                paths = String.Join( ", ", args.Commit.ChangedDirs, 0, count ) + "...";
            }

            string[] lines = args.Commit.LogMessage.Trim().Split( '\n' );
            count = lines.Length < MaxPaths ? lines.Length : MaxPaths;
            string logMessage = String.Join( "\n", lines, 0, count );
            if ( count < lines.Length )
                logMessage += "\r\n...";

            string msg = String.Format( "{0} committed revision {1} ({2})\r\n{3}", 
                args.Commit.Author, args.Commit.Revision, paths, logMessage );

            Console.WriteLine( msg );
            bot.Broadcast( msg );
        }

        private const int MaxPaths = 7;
        private AnkhBot bot;
        private IRepository repository;
}
}
