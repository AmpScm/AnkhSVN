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
	public class WorkingCopyItemTest : TestBase
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
            WorkingCopyItem item  = new WorkingCopyFile( Path.Combine( this.WcPath, "Form.cs" ) );
            
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
            WorkingCopyItem wc2item = new WorkingCopyFile( Path.Combine( wc2, "Form.cs" ) );
            using( StreamWriter w = new StreamWriter( wc2item.Path, false ) )
                w.Write( "MOO" );
            wc2item.Commit( "Log message", false );

            // try to update the primary wc
            WorkingCopyItem item = new WorkingCopyFile( Path.Combine( this.WcPath, "Form.cs" ) );
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

            WorkingCopyItem folderItem = new WorkingCopyDirectory( this.WcPath );
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
            WorkingCopyItem file = new WorkingCopyFile(Path.Combine(this.WcPath, "Form.cs") );
            this.RunCommand( "svn",  "ps foo moo " + file.Path );
            Assertion.AssertEquals( "property status should have been modified", 
                StatusKind.Modified, file.Status.PropertyStatus );

            //TODO: more here?
        }
        #endregion

	}
}
