using System;
using System.Collections.Generic;
using System.Text;

namespace PostCommit.Remoting
{
    [Serializable]
    public class Commit
    {
        public Commit( string author, int revision, string logMessage,
            string[] changedPaths, string[] changedDirs )
        {
            this.author = author;
            this.revision = revision;
            this.logMessage = logMessage;
            this.changedPaths = changedPaths;
            this.changedDirs = changedDirs;
        }

        public Commit()
        {
            // empty
        }


        public string[] ChangedDirs
        {
            get { return changedDirs; }
            set { this.changedDirs = value; }
        }

        public string[] ChangedPaths
        {
            get { return changedPaths; }
            set { this.changedPaths = value; }
        }

        public string Author
        {
            get { return author; }
            set { this.author = value; }
        }

        public string LogMessage
        {
            get { return logMessage; }
            set { this.logMessage = value; }
        }

        public int Revision
        {
            get { return this.revision; }
            set { this.revision = value; }
        }

        private string logMessage;
        private string author;
        private string[] changedPaths;
        private string[] changedDirs;
        private int revision;
    }
}
