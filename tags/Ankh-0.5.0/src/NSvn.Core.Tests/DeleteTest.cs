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

            Assertion.Assert( "File not deleted", !File.Exists( path1 ) );
            Assertion.Assert( "File not deleted", !File.Exists( path2 ) );

            Assertion.AssertEquals( "File not deleted", 'D', this.GetSvnStatus( path1 ) );
            Assertion.AssertEquals( "File not deleted", 'D', this.GetSvnStatus( path2 ) );
            Assertion.AssertSame( "CommitInfo should be invalid", CommitInfo.Invalid, info );
           
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
            Assertion.Assert( "Directory wasn't deleted ", cmd.IndexOf( "doc" ) == -1 );
            Assertion.Assert( "Directory wasn't deleted", cmd.IndexOf( "Form.cs" ) == -1 );

            Assertion.Assert( "CommitInfo is invalid", CommitInfo.Invalid != info );


        }

    }
}
