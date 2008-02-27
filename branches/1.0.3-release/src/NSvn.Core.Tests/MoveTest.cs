// $Id$
using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NSvn.Core.Tests 
{
    /// <summary>
    /// Tests the NSvn.Client.MoveFile method
    /// </summary>
    [TestFixture]
    public class MoveTest : TestBase 
    {
        [SetUp]
        public override void SetUp() 
        {
            base.SetUp();

            this.ExtractRepos();
            this.ExtractWorkingCopy();  
        }

        /// <summary>
        /// Tests moving a file in WC
        /// </summary>
        [Test]
        public void TestMoveWCFile() 
        {
            string srcPath = Path.Combine( this.WcPath, "Form.cs" );
            string dstPath = Path.Combine( this.WcPath, "renamedForm.cs" );
           
            CommitInfo info = this.Client.Move( srcPath, dstPath, false ); 

            Assert.IsTrue( File.Exists( dstPath ), "File wasn't moved" );
            Assert.IsTrue( !File.Exists( srcPath ), "Source File still exists" );

        }

        /// <summary>
        /// Tests moving a directory in a WC
        /// </summary>
        [Test]
        public void TestMoveWCDir() 
        {
            string srcPath = Path.Combine( this.WcPath, @"bin\Debug" );
            string dstPath = Path.Combine( this.WcPath, @"renamedDebug" );
           
            CommitInfo info = this.Client.Move( srcPath, dstPath, false ); 

            Assert.IsTrue( Directory.Exists( dstPath ), "Directory wasn't moved" );
            Assert.AreEqual( 'D', this.GetSvnStatus( srcPath), "Status is not 'D'" );
        }

        /// <summary>
        /// Tests moving a file in a Repos
        /// </summary>
        [Test]
        public void TestMoveReposFile() 
        {
            string srcPath = Path.Combine( this.ReposUrl, "Form.cs" );
            string dstPath = Path.Combine( this.ReposUrl, "renamedForm" );
           
            CommitInfo info = this.Client.Move( srcPath, dstPath, false ); 

            String cmd = this.RunCommand( "svn", "list " + this.ReposUrl );
            Assert.IsTrue( cmd.IndexOf( "Form.cs" ) == -1, "File wasn't moved" );
            Assert.IsTrue( cmd.IndexOf( "renamedForm" ) >= 0, "Moved file doens't exist" );
        }
    }
}
