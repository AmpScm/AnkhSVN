using System;
using NUnit.Framework;
using NSvn.Core;
using System.IO;
using System.Collections;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests for the Client::Import method
    /// </summary>
    [TestFixture]
    public class ImportTest : TestBase
    {
        [SetUp]
        public override void SetUp() 
        {
            base.SetUp();
            this.ExtractRepos();
            this.ExtractWorkingCopy();
            this.notifications = new ArrayList();
         }

        [TearDown]
        public override void TearDown()
        {
           // base.TearDown();
        }

		
        /// <summary>
        /// Tests importing an unverioned file into the repository with the new entry :
        /// testfile2.txt.
        /// </summary>	
        [Test]
        public void TestImportFile()
        {
            string truePath = this.CreateTextFile( "testfile.txt" );
            string trueDstUrl = this.ReposUrl;
            string trueNewEntry = "testfile2.txt";
            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );

            CommitInfo info = Client.Import( truePath, trueDstUrl, trueNewEntry, true, ctx );

            String cmd = this.RunCommand( "svn", "list " + this.ReposUrl );
            Assertion.Assert( "File wasn't imported ", cmd.IndexOf( trueNewEntry ) >= 0 );		   
        }

        /// <summary>
        /// Tests importing an unversioned directory into the repository recursively
        /// with the new entry: newDir2.
        /// </summary>
        [Test]
        public void TestImportDir()
        {
			string dir1, dir2, testFile1, testFile2;
			this.CreateSubdirectories(out dir1, out dir2, out testFile1, out testFile2);

			string trueDstUrl = this.ReposUrl;
			string trueNewEntry = "newDir2";
			ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );

			CommitInfo info = Client.Import( dir1, trueDstUrl, trueNewEntry, false, ctx );

			String cmd = this.RunCommand( "svn", "list " + this.ReposUrl );
			Assertion.Assert( "File wasn't imported ", cmd.IndexOf( trueNewEntry ) >= 0 );		   
      
        }

		private void CreateSubdirectories(out string dir1, out string dir2, out string testFile1, out string testFile2)
		{
			dir1 = Path.Combine( this.WcPath, "subdir" );
			Directory.CreateDirectory( dir1 );

			dir2 = Path.Combine( dir1, "subsubdir" );
			Directory.CreateDirectory( dir2 );

			testFile1 = this.CreateTextFile( @"subdir\testfile.txt" );
			testFile2 = this.CreateTextFile( @"subdir\subsubdir\testfile2.txt" );
		}   
    }
	
}