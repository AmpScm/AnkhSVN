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
           
            CommitInfo info = this.Client.Move( srcPath, Revision.Unspecified, dstPath, false ); 

            Assertion.Assert( " File wasn't moved ", File.Exists( dstPath ) );
            Assertion.Assert( " Source File still exists ", !File.Exists( srcPath ) );

        }

        /// <summary>
        /// Tests moving a directory in a WC
        /// </summary>
        [Test]
        public void TestMoveWCDir() 
        {
            string srcPath = Path.Combine( this.WcPath, @"bin\Debug" );
            string dstPath = Path.Combine( this.WcPath, @"renamedDebug" );
           
            CommitInfo info = this.Client.Move( srcPath, Revision.Unspecified, dstPath, false ); 

            Assertion.Assert( " Directory wasn't moved ", Directory.Exists( dstPath ) );
            Assertion.AssertEquals( " Status is not 'D'  ", 'D', this.GetSvnStatus( srcPath) );
        }

        /// <summary>
        /// Tests moving a file in a Repos
        /// </summary>
        [Test]
        public void TestMoveReposFile() 
        {
            string srcPath = Path.Combine( this.ReposUrl, "Form.cs" );
            string dstPath = Path.Combine( this.ReposUrl, "renamedForm" );
           
            CommitInfo info = this.Client.Move( srcPath, Revision.Head, dstPath, false ); 

            String cmd = this.RunCommand( "svn", "list " + this.ReposUrl );
            Assertion.Assert( " File wasn't moved ", cmd.IndexOf( "Form.cs" ) == -1 );
            Assertion.Assert( " Moved file doens't exist ", cmd.IndexOf( "renamedForm" ) >= 0 );
        }
    }
}
