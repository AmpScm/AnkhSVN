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
    public class CopyTest : TestBase 
    {
        [SetUp]
        public override void SetUp() 
        {
            base.SetUp();

            this.ExtractRepos();
            this.ExtractWorkingCopy(); 
        }

        /// <summary>
        /// Tests copying a file in WC -> WC
        /// </summary>
        [Test]
        public void TestCopyWCWCFile() 
        {
            string srcPath = Path.Combine( this.WcPath, "Form.cs" );
            string dstPath = Path.Combine( this.WcPath, "renamedForm.cs" );
           
            ClientContext ctx = new ClientContext() ;
            CommitInfo info = Client.Copy( srcPath, Revision.Head, dstPath, ctx ); 

            Assertion.Assert( " File wasn't copied ", File.Exists( dstPath ) );
            Assertion.Assert( " Source File don't exists ", File.Exists( srcPath ) );

        }

        /// <summary>
        /// Tests copying a directory in a WC -> WC
        /// </summary>
        [Test]
        public void TestCopyWCWCDir() 
        {
            string srcPath = Path.Combine( this.WcPath, @"bin\Debug" );
            string dstPath = Path.Combine( this.WcPath, @"copyDebug" );
           
            ClientContext ctx = new ClientContext() ;
            CommitInfo info = Client.Copy( srcPath, Revision.Head, dstPath, ctx ); 

            Assertion.Assert( "Directory don't exist ", Directory.Exists( dstPath ) );
            Assertion.AssertEquals( " Status is not 'A'  ", 'A', this.GetSvnStatus( dstPath) );
        }

        /// <summary>
        /// Tests copying a directory in a WC -> URL (repository)
        /// </summary>
        [Test]
        public void TestCopyWCReposDir() 
        {
            string srcPath = Path.Combine( this.WcPath, @"bin\Debug" );
            string dstPath = Path.Combine( this.ReposUrl, @"copyDebug" );
           
            ClientContext ctx = new ClientContext() ;
            CommitInfo info = Client.Copy( srcPath, Revision.Head, dstPath, ctx ); 

            Assertion.Assert( " Directory don't exist ", !Directory.Exists( dstPath ) );
            String cmd = this.RunCommand( "svn", "list " + this.ReposUrl );
            Assertion.Assert( " File wasn't copied ", cmd.IndexOf( "copyDebug" ) >= 0 );
        }

        /// <summary>
        /// Tests copying a from a Repository to WC: URL -> WC
        /// </summary>
        [Test]
        public void TestCopyReposWCFile() 
        {
            string srcPath = Path.Combine( this.ReposUrl, "Form.cs" );
            string dstPath = Path.Combine( this.WcPath, "copyForm" );
           
            ClientContext ctx = new ClientContext();
            CommitInfo info = Client.Copy( srcPath, Revision.Head, dstPath, ctx ); 

            Assertion.AssertEquals( " File is not copied  ", 'A', this.GetSvnStatus( dstPath) );
        }

        /// <summary>
        /// Tests copying a file within a Repos: URL -> URL
        /// </summary>
        [Test]
        public void TestCopyReposReposFile() 
        {
            string srcPath = Path.Combine( this.ReposUrl, "Form.cs" );
            string dstPath = Path.Combine( this.ReposUrl, "copyForm" );
           
            ClientContext ctx = new ClientContext();
            CommitInfo info = Client.Copy( srcPath, Revision.Head, dstPath, ctx ); 

            String cmd = this.RunCommand( "svn", "list " + this.ReposUrl );
            Assertion.Assert( " File wasn't copied ", cmd.IndexOf( "Form.cs" ) >= 0 );
            Assertion.Assert( " Copied file doens't exist ", cmd.IndexOf( "copyForm" ) >= 0 );
        }
    }
}
