// $Id$
using System;
using System.IO;
using NUnit.Framework;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests the Client.Commit method.
    /// </summary>
    [TestFixture]
    public class CommitTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractRepos();
            this.ExtractWorkingCopy();
        }

        /// <summary>
        /// Modifies a file in the working copy and commits the containing directory.
        /// </summary>
        [Test]
        public void TestBasicCommit()
        {
            string filepath = Path.Combine( this.WcPath, "Form.cs" );
            using ( StreamWriter w = new StreamWriter( filepath ) )
                w.Write( "Moo" );

            CommitInfo info = this.Client.Commit( new string[]{ this.WcPath }, false );
           
            char status = this.GetSvnStatus( filepath );
            Assert.AreEqual( 0, status, "File not committed" );
        }

        /// <summary>
        /// Locks and modifies a file in the working copy and commits the containing directory keeping, and not keeping locks
        /// </summary>
        [Test]
        public void TestCommitWithLocks()
        {
            string filepath = Path.Combine( this.WcPath, "Form.cs" );
            using ( StreamWriter w = new StreamWriter( filepath ) )
                w.Write( "Moo" );

            this.RunCommand( "svn", "lock " + filepath );

            CommitInfo info = this.Client.Commit( new string[]{ this.WcPath }, true, true );
            Assert.IsNotNull( info );
            Assert.IsTrue( info != CommitInfo.Invalid );
            char locked = this.RunCommand( "svn", "status " + filepath )[5];
            Assert.IsTrue( locked == 'K', "File was unlocked while lock should be kept" );

            using ( StreamWriter w = new StreamWriter( filepath ) )
                w.Write( "Bah" );
            
            info = this.Client.Commit( new string[]{ this.WcPath }, true, false );
            Assert.IsNotNull( info );
            Assert.IsTrue( info != CommitInfo.Invalid );

            string output = this.RunCommand( "svn", "status " + filepath );
            Assert.AreEqual( String.Empty, output );
        }

        /// <summary>
        /// Commits a single file
        /// </summary>
        [Test]
        public void TestCommitFile()
        {
            string filepath = Path.Combine( this.WcPath, "Form.cs" );
            using ( StreamWriter w = new StreamWriter( filepath ) )
                w.Write( "Moo" );

            CommitInfo info = this.Client.Commit( new string[]{ filepath }, true );

            char status = this.GetSvnStatus( filepath );
            Assert.AreEqual( 0, status, "File not committed" );

        }

        [Test]
        public void TestCommitWithLogMessage()
        {
            this.filepath = Path.Combine( this.WcPath, "Form.cs" );
            using ( StreamWriter w = new StreamWriter( filepath ) )
                w.Write( "Moo" );

            AuthenticationBaton baton = new AuthenticationBaton();
            this.Client.AuthBaton.Add( AuthenticationProvider.GetUsernameProvider() );
            this.Client.AuthBaton.SetParameter( 
                AuthenticationBaton.ParamDefaultUsername, Environment.UserName );

            
            this.Client.LogMessage += new LogMessageDelegate(this.LogMessageCallback);

            this.logMessage = "Moo ";
            CommitInfo info = this.Client.Commit( new string[]{ this.WcPath }, false );

            Assert.AreEqual( Environment.UserName, info.Author, "Wrong username" );
            string output = this.RunCommand( "svn", "log " + this.filepath + " -r HEAD" );
            
            Assert.IsTrue( output.IndexOf( this.logMessage ) >= 0, 
                "Log message not set" );

        } 

        [Test]
        public void TestCommitWithNonAnsiCharsInLogMessage()
        {
            this.filepath = Path.Combine( this.WcPath, "Form.cs" );
            using ( StreamWriter w = new StreamWriter( filepath ) )
                w.Write( "Moo" );
            this.Client.LogMessage +=new LogMessageDelegate(this.LogMessageCallback);
            this.logMessage = "Æ e i a æ å. Møøøø! über";
            CommitInfo info = this.Client.Commit( new string[]{ this.WcPath }, false );
        }

        /// <summary>
        /// Tests that a commit on an unmodified repos returns CommitInfo.Invalid.
        /// </summary>
        [Test]
        public void TestCommitWithNoModifications()
        {
            this.Client.LogMessage += new LogMessageDelegate(this.LogMessageCallback);
            CommitInfo ci = this.Client.Commit( new string[]{ this.WcPath }, false );
            Assert.AreSame( CommitInfo.Invalid, ci );
        }
        
        /// <summary>
        /// Tests that you can cancel a commit.
        /// </summary>
        [Test]
        public void TestCancelledCommit()
        {
            string path = Path.Combine( this.WcPath, "Form.cs" );
            using( StreamWriter w = new StreamWriter( path ) )
                w.Write( "MOO" );
            this.Client.LogMessage += new LogMessageDelegate( this.CancelLogMessage );
            CommitInfo info = this.Client.Commit( new string[]{ path }, true );

            Assert.AreEqual( CommitInfo.Invalid, info,
                "info should be Invalid for a cancelled commit" );

            string output = this.RunCommand( "svn", "st " + this.WcPath ).Trim();
            Assert.AreEqual( 'M', output[0], 
                "File committed even for a cancelled log message" );
        }

        [Test]
        [ExpectedException( typeof(WorkingCopyLockedException) )]
        public void TestLockedWc()
        {
            string lockPath = Path.Combine(
                Path.Combine( this.WcPath, Client.AdminDirectoryName ), "lock" );
            using( File.CreateText( lockPath ) )
            {
                this.Client.Commit( new string[]{ this.WcPath }, true );            
            }
        }

        private void LogMessageCallback( object sender, LogMessageEventArgs args )
        {
            Assert.AreEqual( 1, args.CommitItems.Length, "Wrong number of commit items" );
            Assert.AreEqual( args.CommitItems[0].Path.IndexOf( this.filepath ) >= 0,
                "Wrong path");
            Assert.AreEqual( NodeKind.File, args.CommitItems[0].Kind, "Wrong kind" );
            Assert.AreEqual( 6, args.CommitItems[0].Revision, "Wrong revision" );

            args.Message = this.logMessage;
        }

        private void CancelLogMessage( object sender, LogMessageEventArgs args )
        {
            args.Message = null;
        }


        private string logMessage;
        private string filepath;
    }
}
