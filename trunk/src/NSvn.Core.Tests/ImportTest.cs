// $Id$
using System;
using NUnit.Framework;
using NSvn.Core;
using System.IO;
using System.Collections;

using NSvn.Common;

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
            base.TearDown();
        }

		
        /// <summary>
        /// Tests importing an unverioned file into the repository with the new entry :
        /// testfile2.txt.
        /// </summary>	
        [Test]
        public void TestImportFile()
        {
            string truePath = this.CreateTextFile( "testfile.txt" );
            string trueDstUrl = this.ReposUrl + "/testfile.txt";

            CommitInfo info = Client.Import( truePath, trueDstUrl, Recurse.None );

            String cmd = this.RunCommand( "svn", "list " + this.ReposUrl );
            Assert.IsTrue( cmd.IndexOf( "testfile.txt" ) >= 0, "File wasn't imported " );
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

            string trueDstUrl = this.ReposUrl + "/newDir2";

            CommitInfo info = this.Client.Import( dir1, trueDstUrl, Recurse.Full );

            String cmd = this.RunCommand( "svn", "list " + this.ReposUrl );
            Assert.IsTrue( cmd.IndexOf( "newDir2" ) >= 0, "File wasn't imported" );
      
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