// $Id$
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

        [Test]
        public void TestGetChildren()
        {
            this.ExtractWorkingCopy();
            WorkingCopyDirectory wc = new WorkingCopyDirectory( this.WcPath );

            RepositoryDirectory repos = new RepositoryDirectory( this.ReposUrl );

            RepositoryResourceDictionary children = repos.GetChildren();

            Assertion.AssertEquals( "Wrong number of children", wc.Children.Count,
                children.Count );

            // assert that all the nodes in the wc are found in the repos
            foreach( string path in wc.Children.Keys )
            {
                Assertion.Assert( path + " not found", children.ContainsKey( path ) );
                if( wc.Children[path].IsDirectory )
                    Assertion.AssertEquals( "Wrong type of node for " + path,
                        typeof(RepositoryDirectory), children[path].GetType() );
                else if ( !wc.Children[path].IsDirectory)
                    Assertion.AssertEquals( "Wrong type of node for " + path,
                        typeof(RepositoryFile), children[path].GetType() );
            }
        }

        [Test]
        public void TestName()
        {
            RepositoryDirectory dir = new RepositoryDirectory( "file:///tmp/repos" );
            Assertion.AssertEquals( "Directory name is wrong", "repos", dir.Name );

            dir = new RepositoryDirectory( "http://moo.porn.com/svn/trunk/directory" );
            Assertion.AssertEquals( "Directory name is wrong", "directory", dir.Name );

            dir = new RepositoryDirectory( "http://moo.porn.com:8088/svn/moo/foo" );
            Assertion.AssertEquals( "Directory name is wrong", "foo", dir.Name );
        }

        private string localDir;
    }
}
