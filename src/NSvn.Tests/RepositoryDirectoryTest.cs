using System;
using NSvn.Core;
using System.IO;
using NUnit.Framework;

namespace NSvn.Tests
{
	/// <summary>
	/// Tests the RepositoryDirectory class
	/// </summary>
	public class RepositoryDirectoryTest : TestBase
	{
        [SetUp]
        public override void SetUp()
        {
            this.ExtractRepos();
            this.localDir = this.FindDirName( @"\tmp\wc" );
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            try{ Directory.Delete( this.localDir ); }
            catch( Exception ){}
        }

        [Test]
        public void TestInstanceCheckout()
        {
            RepositoryDirectory dir = new RepositoryDirectory( this.ReposUrl );
            WorkingCopyDirectory wcDir = dir.Checkout( this.localDir, true );
            
            Assertion.Assert( "Directory not checked out", 
                Directory.Exists( this.localDir ) );
            Assertion.AssertEquals( "Status should be normal", 
                StatusKind.Normal, wcDir.Status.TextStatus );

            Assertion.Assert( "File not checked out", File.Exists(
                Path.Combine( wcDir.Path, "Form.cs" ) ) );
        }

        [Test]
        public void TestStaticCheckout()
        {
            WorkingCopyDirectory wcDir = RepositoryDirectory.Checkout(
                this.ReposUrl, this.localDir, Revision.FromNumber(2), true );

            Assertion.Assert( "Directory not checked out", 
                Directory.Exists( this.localDir ) );
            Assertion.AssertEquals( "Wrong revision in checked out wc",
                2, wcDir.Status.Entry.Revision );
        }

        private string localDir;
	}
}
