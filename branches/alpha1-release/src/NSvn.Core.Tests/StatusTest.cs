// $Id$
using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

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
            this.ExtractWorkingCopy();
            this.ExtractRepos();
        }

        /// <summary>Compares the status from Client::Status with the output from 
        /// commandline client</summary>
        [Test]
        public void TestLocalStatus()
        {
            string unversioned = this.CreateTextFile( "unversioned.cs" );
            string added = this.CreateTextFile( "added.cs" );
            this.RunCommand( "svn", "add " + added );

            string changed = this.CreateTextFile( "Form.cs" );

            string propChange = Path.Combine( this.WcPath, "App.ico" );

            this.RunCommand( "svn", "ps foo bar " + propChange );

            int youngest;
            StatusDictionary dict = Client.Status( out youngest, unversioned, false, false, false, 
                false, new ClientContext() );
            Assertion.AssertEquals( "Wrong text status on " + unversioned, 
                dict.Get(unversioned).TextStatus, StatusKind.Unversioned );

            dict = Client.Status( out youngest, added, false, false, false, 
                false, new ClientContext() );
            Assertion.AssertEquals( "Wrong text status on " + added, 
                dict.Get(added).TextStatus, StatusKind.Added );

            dict = Client.Status( out youngest, changed, false, false, false, 
                false, new ClientContext() );
            Assertion.AssertEquals( "Wrong text status " + changed, 
                dict.Get(changed).TextStatus, StatusKind.Modified );

            dict = Client.Status( out youngest, propChange, 
                false, false, false, 
                false, new ClientContext() );
            Assertion.AssertEquals( "Wrong property status " + propChange, 
                dict.Get(propChange).PropertyStatus, StatusKind.Modified );
            
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
                        
            int youngest;
            StatusDictionary dict = Client.Status( out youngest, form, 
                false, false, false, false, 
                new ClientContext() );

            info.CheckEquals( dict.Get(form).Entry );
        }
       

        /// <summary>
        /// Tests an update where we contact the repository
        /// </summary>
        [Test]
        public void TestRepositoryStatus()
        {
            // modify the file in another working copy and commit
            string wc2 = this.FindDirName( "wc2" );
            this.ExtractZipFile( wc2, WC_FILE );
            using( StreamWriter w = new StreamWriter( 
                       Path.Combine(wc2, "Form.cs"), true ) )
                w.Write( "Hell worl" );
            this.RunCommand( "svn", "ci -m \"\" " + wc2 );

            // find the status in our own wc
            int youngest;
            string form = Path.Combine( this.WcPath, "Form.cs" );
            StatusDictionary dict = Client.Status( out youngest, 
                form, false, false, true, true, new ClientContext() );

            Assertion.AssertEquals( "Wrong status", 
                dict.Get(form).RepositoryTextStatus, StatusKind.Modified );
            this.RecursiveDelete( wc2 );
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

            Status status = Client.SingleStatus( unversioned );
            Assertion.AssertEquals( "Wrong text status on " + unversioned, 
                status.TextStatus, StatusKind.Unversioned );

            status = Client.SingleStatus( added );
            Assertion.AssertEquals( "Wrong text status on " + added, 
                status.TextStatus, StatusKind.Added );

            status = Client.SingleStatus( changed );
            Assertion.AssertEquals( "Wrong text status " + changed, 
                status.TextStatus, StatusKind.Modified );

            status = Client.SingleStatus( propChange );
            Assertion.AssertEquals( "Wrong property status " + propChange, 
                status.PropertyStatus, StatusKind.Modified );

        }

        private class Info
        {
            private static readonly Regex INFO = new Regex(@"Path:\s(?'path'\S+)\s+Name:\s(?'name'\S+)\s+Url:\s(?'url'\S+)\s+Revision:\s(?'revision'\S+)\s+Node Kind:\s(?'nodekind'\S+)\s+Schedule:\s(?'schedule'\S+)\s+Last Changed Author:\s+(?'lastchangedauthor'\S+)", 
                (RegexOptions) 0);

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
                Assertion.AssertEquals( "Schedule differs",
                    match.Groups["schedule"].Value.ToLower(), entry.Schedule.ToString().ToLower() );
                Assertion.AssertEquals( "Last changed author differs", 
                    match.Groups["lastchangedauthor"].Value, entry.CommitAuthor );
            }

            private string output;
        }
	}
}
