// $Id$
using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Utils;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests Client::Status
    /// </summary>
    [TestFixture]
    public class StatusTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.ExtractWorkingCopy();
            this.ExtractRepos();
        }

        /// <summary>Compares the status from Client::Status with the output from 
        /// commandline client</summary>
        [Test]
        public void TestLocalStatus()
        {
            int youngest;      

            string unversioned = this.CreateTextFile( "unversioned.cs" );
            string added = this.CreateTextFile( "added.cs" );
            this.RunCommand( "svn", "add " + added );
            string changed = this.CreateTextFile( "Form.cs" );
            string propChange = Path.Combine( this.WcPath, "App.ico" );
            this.RunCommand( "svn", "ps foo bar " + propChange );

            this.Client.Status( out youngest, unversioned, Revision.Unspecified, 
                new StatusCallback( this.StatusFunc ), false, false, false, 
                false );
            Assertion.AssertEquals( "Wrong text status on " + unversioned, 
                this.currentStatus.TextStatus, StatusKind.Unversioned );
            Assertion.AssertEquals( unversioned, this.currentPath );

            this.Client.Status( out youngest, added,  Revision.Unspecified, 
                new StatusCallback( this.StatusFunc ), false, false, false, 
                false );
            Assertion.AssertEquals( "Wrong text status on " + added, 
                this.currentStatus.TextStatus, StatusKind.Added );
            Assertion.AssertEquals( added, this.currentPath );

            this.Client.Status( out youngest, changed, Revision.Unspecified,
                new StatusCallback( this.StatusFunc ), false, false, false, 
                false );
            Assertion.AssertEquals( "Wrong text status " + changed, 
                this.currentStatus.TextStatus, StatusKind.Modified );
            Assertion.AssertEquals( changed, this.currentPath );

            this.Client.Status( out youngest, propChange, Revision.Unspecified,
                new StatusCallback( this.StatusFunc ), false, false, false, 
                false );
            Assertion.AssertEquals( "Wrong property status " + propChange, 
                this.currentStatus.PropertyStatus, StatusKind.Modified );
            Assertion.AssertEquals( propChange, this.currentPath );
        }

        /// <summary>
        /// Tests that the Entry property is somewhat correct
        /// </summary>
        [Test]
        public void TestEntry()
        {
            string form = Path.Combine( this.WcPath, "Form.cs" );
            string output = this.RunCommand( "svn", "info " + form );
            Info info = new Info(output);

            Status s = this.Client.SingleStatus( form );
                        
            int youngest;
            this.Client.Status( out youngest, form,
                Revision.Unspecified, new StatusCallback( this.StatusFunc ),
                false, true, false, false );

            info.CheckEquals( this.currentStatus.Entry );
        }
       

        /// <summary>
        /// Tests an update where we contact the repository
        /// </summary>
        [Test]
        public void TestRepositoryStatus()
        {
            // modify the file in another working copy and commit
            string wc2 = this.FindDirName( "wc2" );
            try
            {                
                Zip.ExtractZipResource( wc2, this.GetType(), WC_FILE );
                this.RenameAdminDirs( wc2 );
                using( StreamWriter w = new StreamWriter( 
                           Path.Combine(wc2, "Form.cs"), true ) )
                    w.Write( "Hell worl" );
                this.RunCommand( "svn", "ci -m \"\" " + wc2 );

                // find the status in our own wc
                int youngest;
                string form = Path.Combine( this.WcPath, "Form.cs" );
                this.Client.Status( out youngest, 
                    form, Revision.Head, new StatusCallback( this.StatusFunc ),
                    false, false, true, true );

                Assertion.AssertEquals( "Wrong status", 
                    this.currentStatus.RepositoryTextStatus, StatusKind.Modified );
            }
            finally
            {
                this.RecursiveDelete( wc2 );
            }
        }

        [Test]
        public void TestSingleStatus()
        {
            string unversioned = this.CreateTextFile( "unversioned.cs" );
            string added = this.CreateTextFile( "added.cs" );
            this.RunCommand( "svn", "add " + added );

            string changed = this.CreateTextFile( "Form.cs" );

            string propChange = Path.Combine( this.WcPath, "App.ico" );

            this.RunCommand( "svn", "ps foo bar " + propChange );

            Status status = this.Client.SingleStatus( unversioned );
            Assertion.AssertEquals( "Wrong text status on " + unversioned, 
                status.TextStatus, StatusKind.Unversioned );

            status = this.Client.SingleStatus( added );
            Assertion.AssertEquals( "Wrong text status on " + added, 
                status.TextStatus, StatusKind.Added );

            status = this.Client.SingleStatus( changed );
            Assertion.AssertEquals( "Wrong text status " + changed, 
                status.TextStatus, StatusKind.Modified );

            status = this.Client.SingleStatus( propChange );
            Assertion.AssertEquals( "Wrong property status " + propChange, 
                status.PropertyStatus, StatusKind.Modified );

        }

        [Test]
        public void TestStatusEquals()
        {
            string form = Path.Combine( this.WcPath, "Form.cs" );
            Status status1 = this.Client.SingleStatus( form );
            Status status2 = this.Client.SingleStatus( form );
            Assertion.AssertEquals( "Should be equal", status1, status2 );
            Assertion.AssertEquals( "Should be equal", status1.Entry, status2.Entry );

            using( StreamWriter w = new StreamWriter( form, true ) )
                w.WriteLine( "Moo" );

            status2 = this.Client.SingleStatus( form );
            Assertion.Assert( "Should be non-equal", !status1.Equals( status2 ) );
            Assertion.AssertEquals( "Should be equal", status1.Entry, status2.Entry );

            // unversioned items have no .Entry
            string unversioned = Path.Combine( this.WcPath, "Unversioned.txt" );
            using( StreamWriter w = new StreamWriter( unversioned, false ) )
                w.WriteLine( "Moo" );
            status2 = this.Client.SingleStatus( unversioned );
            Assertion.AssertNull( ".Entry should be null", status2.Entry );
            Assertion.Assert( "Should not be similar", !status2.Equals( status1 ) );
        }

        private void StatusFunc( string path, Status status )
        {
            this.currentPath = path;
            this.currentStatus = status;
        }

        private class Info
        {
            private static readonly Regex INFO = new Regex(@"Path:\s(?'path'.+?)\s+Name:\s(?'name'\S+)\s+Url:\s(?'url'\S+)\s+Repository UUID:\s(?'reposuuid'\S+)\s+Revision:\s(?'revision'\S+)\s+Node Kind:\s(?'nodekind'\S+)\s+Schedule:\s(?'schedule'\S+)\s+Last Changed Author:\s+(?'lastchangedauthor'\S+)", 
                RegexOptions.IgnoreCase );

            public Info( string output )
            {
                this.output = output;                
            }

            public void CheckEquals( Entry entry )
            {
                if ( !INFO.IsMatch(this.output) )
                    throw new Exception( "No match" );

                Match match = INFO.Match(this.output);

                Assertion.AssertEquals( "Url differs", match.Groups["url"].Value, entry.Url );
                Assertion.AssertEquals( "Name differs", match.Groups["name"].Value, entry.Name );
                Assertion.AssertEquals( "Revision differs", 
                    int.Parse(match.Groups["revision"].Value), entry.Revision );
                Assertion.AssertEquals( "Node kind differs", 
                    match.Groups["nodekind"].Value.ToLower(), entry.Kind.ToString().ToLower() );
                Assertion.AssertEquals( "Repository UUID differs", 
                    match.Groups["reposuuid"].Value, entry.Uuid );
                Assertion.AssertEquals( "Schedule differs",
                    match.Groups["schedule"].Value.ToLower(), entry.Schedule.ToString().ToLower() );
                Assertion.AssertEquals( "Last changed author differs", 
                    match.Groups["lastchangedauthor"].Value, entry.CommitAuthor );
            }

            private string output;            
        }

        private string currentPath;
        private Status currentStatus;
    }
}
