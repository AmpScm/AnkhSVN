// $Id$
using System;
using System.IO;
using NUnit.Framework;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests for the Client::Checkout method
    /// </summary>
    [TestFixture]
    public class DeleteTest : TestBase
    {
        [SetUp]
        public override void SetUp() 
        {
            base.SetUp();
            this.ExtractRepos();
            this.ExtractWorkingCopy();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

		
        /// <summary>
        /// Tests deleting files in a working copy
        /// </summary>
        /// //TODO: Implement the variable admAccessBaton
        [Test]
        public void TestDeleteWCFiles()
        {
            string path1 = Path.Combine( this.WcPath, "Form.cs" );
            string path2 = Path.Combine( this.WcPath, "AssemblyInfo.cs" );

            CommitInfo info = this.Client.Delete( new string[]{ path1, path2 }, false );

            Assert.IsTrue( !File.Exists( path1 ), "File not deleted" );
            Assert.IsTrue( !File.Exists( path2 ), "File not deleted" );

            Assert.AreEqual( 'D', this.GetSvnStatus( path1 ), "File not deleted" );
            Assert.AreEqual( 'D', this.GetSvnStatus( path2 ), "File not deleted" );
            Assert.AreSame( CommitInfo.Invalid, info, "CommitInfo should be invalid" );
           
        }

        /// <summary>
        /// Tests deleting a directory in the repository
        /// </summary>
        //TODO: Implement the variable admAccessBaton
        [Test]
        public void TestDeleteFromRepos()
        {
            string path1 = Path.Combine( this.ReposUrl, @"doc" );
            string path2 = Path.Combine( this.ReposUrl, "Form.cs" );

            CommitInfo info = this.Client.Delete( new string[]{path1, path2}, false );

            String cmd = this.RunCommand( "svn", "list " + this.ReposUrl );
            Assert.IsTrue( cmd.IndexOf( "doc" ) == -1, "Directory wasn't deleted " );
            Assert.IsTrue( cmd.IndexOf( "Form.cs" ) == -1, "Directory wasn't deleted" );

            Assert.IsTrue( CommitInfo.Invalid != info, "CommitInfo is invalid" );
        }

        [Test]
        public void TestForceDelete()
        {
            string path = Path.Combine( this.WcPath, "Form.cs" );

            // modify the file
            using ( StreamWriter writer = new StreamWriter( path, true ) )
            {
                writer.WriteLine( "Hi ho" );
            }

            // this will throw if force doesn't work
            this.Client.Delete( new string[] { path }, true );
        }

    }
}
