using System;
using System.IO;
using NUnit.Framework;

namespace NSvn.Core.Tests
{
	/// <summary>
	/// Summary description for CommitTest.
	/// </summary>
	[TestFixture]
	public class CommitTest : TestBase
	{
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractRepos();
            this.ExtractWorkingCopy();
        }

        /// <summary>
        /// Modifies a file in the working copy and commits it
        /// </summary>
        [Test]
        public void TestBasicCommit()
        {
            string filepath = Path.Combine( this.WcPath, "Form.cs" );
            using ( StreamWriter w = new StreamWriter( filepath ) )
                w.Write( "Moo" );

            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );
            Client.Commit( new string[]{ this.WcPath }, false, ctx );
           
            string output = this.RunCommand( "svn", "st " + this.WcPath ).Trim();
            Assertion.AssertEquals( "File not committed", "", 
                output );
        }

        [Test]
        public void TestCommitWithLogMessage()
        {
            this.filepath = Path.Combine( this.WcPath, "Form.cs" );
            using ( StreamWriter w = new StreamWriter( filepath ) )
                w.Write( "Moo" );
            ClientContext ctx = new ClientContext();
            ctx.LogMessageCallback = new LogMessageCallback( this.LogMessageCallback );
            CommitInfo info = Client.Commit( new string[]{ this.WcPath }, false, ctx );

            Assertion.AssertEquals( "Wrong username", Environment.UserName, info.Author );
            string output = this.RunCommand( "svn", "log " + this.filepath + " -r HEAD" );
            Assertion.Assert( "Log message not set", 
                output.IndexOf( "Moo is the log message" ) >= 0 );

        } 

        private string LogMessageCallback( CommitItem[] items )
        {
            Assertion.AssertEquals( "Wrong number of commit items", 1, items.Length );
            Assertion.Assert( "Wrong path", items[0].Path.IndexOf( 
                this.filepath.Replace( "\\", "/" ) ) >= 0 );
            Assertion.AssertEquals( "Wrong kind", NodeKind.File, items[0].Kind );
            Assertion.AssertEquals( "Wrong revision", 1, items[0].Revision );

            return "Moo is the log message";
        }

        private string filepath;
	}
}
