// $Id$
using System;
using System.IO;
using NUnit.Framework;
using NSvn.Core;

using Zip = Utils.Zip;

namespace NSvn.Tests
{
    /// <summary>
    /// Tests the WorkingCopyItem resource.
    /// </summary>
    [TestFixture]
    public class WorkingCopyResourceTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            this.ExtractRepos();
            this.ExtractWorkingCopy();
        }

        #region TestCommit
        /// <summary>
        /// Tests committing.
        /// </summary>
        [Test]
        public void TestCommit()
        {
            WorkingCopyResource item  = new WorkingCopyFile( Path.Combine( this.WcPath, "Form.cs" ) );
            
            Assertion.AssertEquals( "Wrong status. Should be normal", StatusKind.Normal,
                item.Status.TextStatus  );

            using( StreamWriter w = new StreamWriter( item.Path, true ) )
                w.Write( "Moo" );

            Assertion.AssertEquals( "Wrong status. Should be modified", StatusKind.Modified,
                item.Status.TextStatus );

            item.Commit( false );

            Assertion.AssertEquals( "Wrong status. Should be normal", StatusKind.Normal,
                item.Status.TextStatus );
           
        }

        /// <summary>
        /// Tests committing multiple targets at once.
        /// </summary>
        [Test]
        public void TestCommitMultipleTargets()
        {
            WorkingCopyResource item1 = WorkingCopyResource.FromPath( 
                Path.Combine( this.WcPath, "Form.cs" ) );
            WorkingCopyResource item2 = WorkingCopyResource.FromPath(
                Path.Combine( this.WcPath, "App.ico" ) );

            using( StreamWriter w1 = new StreamWriter( item1.Path, true ) )
                w1.Write( "Moo" );
            using( StreamWriter w2 = new StreamWriter( item2.Path, true ) )
                w2.Write( "Moo" );

            WorkingCopyResource.Commit( new WorkingCopyResource[]{ item1, item2 }, false );

            Assertion.AssertEquals( "Wrong status", StatusKind.Normal, 
                item1.Status.TextStatus );
            Assertion.AssertEquals( "Wrong status", StatusKind.Normal, 
                item2.Status.TextStatus );
        }
        #endregion

        //[Ignore( "Doesn't work yet" )]
        #region TestCopyTo
        [Test]
        public void TestCopyTo()
        {
            //Tests copying a file to a directory within the working copy
            WorkingCopyResource itemWcSrc  = new WorkingCopyFile( Path.Combine( this.WcPath, "Form.cs" ) );
            
            Assertion.AssertEquals( "Wrong status. Cant copy an unversioned file", StatusKind.Normal,
                itemWcSrc.Status.TextStatus  );
          
            WorkingCopyResource itemWcDst = itemWcSrc.CopyTo( new WorkingCopyFile( Path.Combine( this.WcPath, @"obj" )), Revision.Head);

            Assertion.AssertEquals( "Wrong type returned. Should be working copy file",
                typeof( WorkingCopyFile ), itemWcDst.GetType() );

            //     Assertion.AssertEquals( "Wrong status. Should be added", StatusKind.Added,
            //          itemWcDst.Status.TextStatus );  
  
            //Tests copying a file from working copy to a file in repository
            /*    RepositoryResource itemReposSrc  = new RepositoryFile( Path.Combine( this.ReposPath, "Form2.cs" ) );
            
                  RepositoryResource itemReposDst = itemWcSrc.CopyTo( itemReposSrc, "Copying a file to the repository", Revision.Head ) ;

                  String cmd = this.RunCommand( "svn", "list " + this.ReposUrl );
                  Assertion.Assert( "File wasn't copied ", cmd.IndexOf( "Form2.cs") >= 0 );		
                  */             
  
        }
        #endregion


        #region LogMessageProvider
        private class LogMessageProvider : ILogMessageProvider
        {
            public LogMessageProvider( string path )
            { 
                this.path = path;
            }
        
            #region Implementation of ILogMessageProvider
            public string GetLogMessage(NSvn.Core.CommitItem[] targets)
            {
                Assertion.AssertEquals( "Wrong number of commit items", 1, targets.Length );
                Assertion.AssertEquals( "Wrong path", this.path, targets[0].Path );

                return "Log message";
            }
        
            #endregion
            private string path;
        }
        #endregion

        #region TestUpdate
        [Test]
        public void TestUpdate()
        {
            // create a second wc and change a file
            string wc2 = this.FindDirName( @"\tmp\wc2" );
            Zip.ExtractZipResource( wc2, this.GetType(), this.WC_FILE );
            WorkingCopyResource wc2item = new WorkingCopyFile( Path.Combine( wc2, "Form.cs" ) );
            using( StreamWriter w = new StreamWriter( wc2item.Path, false ) )
                w.Write( "MOO" );
            wc2item.Commit( false );

            // try to update the primary wc
            WorkingCopyResource item = new WorkingCopyFile( Path.Combine( this.WcPath, "Form.cs" ) );
            using( StreamWriter w2 = new StreamWriter( item.Path ) )
                w2.Write( "BLAH" );
            item.Update();

            Assertion.AssertEquals( "Item should be conflicted", StatusKind.Conflicted, 
                item.Status.TextStatus );

            // make sure you cannot commit a conflicted wc
            try
            {
                item.Commit( false );
                Assertion.Fail( "Should not be able to commit a conflicted file" );
            }
            catch( SvnClientException )
            { /*empty*/ }

            WorkingCopyResource folderItem = new WorkingCopyDirectory( this.WcPath );
            try
            {
                folderItem.Commit(  true );
                Assertion.Fail( "Should not be able to commit a conflicted directory" );
            }
            catch( SvnClientException )
            { /* empty */ }

            
        }
        #endregion

        #region TestStatus
        [Test]
        public void TestStatus()
        {
            // text status has been tested in other tests
            WorkingCopyResource file = new WorkingCopyFile(Path.Combine(this.WcPath, "Form.cs") );

            //retrieve the status first to force it to be cached
            Assertion.AssertEquals( "Property status should be none",
                StatusKind.None, file.Status.PropertyStatus );

            // then set a property and check if it correctly detects the change in status
            this.RunCommand( "svn",  "ps foo moo " + file.Path );
            Assertion.AssertEquals( "property status should have been modified", 
                StatusKind.Modified, file.Status.PropertyStatus );

            //TODO: more here?
        }
        #endregion

        [Test]
        public void TestFromPath()
        {
            ILocalResource resDir = WorkingCopyResource.FromPath( this.WcPath );
            Assertion.AssertEquals( "Wrong type resource", typeof(WorkingCopyDirectory), 
                resDir.GetType() );

            ILocalResource resFile = WorkingCopyResource.FromPath( Path.Combine(
                this.WcPath, "Form.cs" ) );
            Assertion.AssertEquals( "Wrong type resource", typeof(WorkingCopyFile),
                resFile.GetType() );
        }

    }
}
