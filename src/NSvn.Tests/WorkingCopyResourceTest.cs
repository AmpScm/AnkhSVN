using System;
using System.IO;
using NUnit.Framework;
using NSvn.Core;

namespace NSvn.Tests
{
	/// <summary>
	/// Summary description for WorkingCopyItemTest.
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

            item.Commit( new LogMessageProvider( item.Path ), false );

            Assertion.AssertEquals( "Wrong status. Should be normal", StatusKind.Normal,
                item.Status.TextStatus );
           
        }
        #endregion

        [Ignore( "Doesn't work yet" )]
        #region TestCopy
        [Test]
        public void TestCopy()
        {
            //Tests copying a file to a directory within the working copy
            WorkingCopyResource itemWcSrc  = new WorkingCopyFile( Path.Combine( this.WcPath, "Form.cs" ) );
            WorkingCopyResource itemWcDst  = new WorkingCopyDirectory( Path.Combine( this.WcPath, @"obj" ) );
            
            Assertion.AssertEquals( "Wrong status. Cant copy an unversioned file", StatusKind.Normal,
                itemWcSrc.Status.TextStatus  );
          
            itemWcSrc.Copy( itemWcDst.Path ) ;

            Assertion.AssertEquals( "Wrong status. Should be added", StatusKind.Added,
                itemWcDst.Status.TextStatus );  
  
            //Tests copying a file from working copy to a file in repository
      /*      RepositoryResource itemReposDst  = new RepositoryFile( Path.Combine( this.ReposPath, "Form2.cs" ) );
            
            Assertion.AssertEquals( "Wrong status. Cant copy an unversioned file", StatusKind.Normal,
                itemWcSrc.Status.TextStatus  );
          
            itemWcSrc.Copy( "Copying a file to the repository", Revision.Head, itemReposDst.Url ) ;

            String cmd = this.RunCommand( "svn", "list " + this.ReposUrl );
            Assertion.Assert( "File wasn't imported ", cmd.IndexOf( "Form2.cs") >= 0 );		
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
            this.ExtractZipFile( wc2, this.WC_FILE );
            WorkingCopyResource wc2item = new WorkingCopyFile( Path.Combine( wc2, "Form.cs" ) );
            using( StreamWriter w = new StreamWriter( wc2item.Path, false ) )
                w.Write( "MOO" );
            wc2item.Commit( "Log message", false );

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
                item.Commit( "Should fail", false );
                Assertion.Fail( "Should not be able to commit a conflicted wc" );
            }
            catch( SvnClientException )
            { /*empty*/ }

            WorkingCopyResource folderItem = new WorkingCopyDirectory( this.WcPath );
            try
            {
                folderItem.Commit( "Should fail", true );
                Assertion.Fail( "Should not be able to commit a conflicted wc" );
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

	}
}
