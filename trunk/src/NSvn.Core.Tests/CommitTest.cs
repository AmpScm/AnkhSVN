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

        [Ignore("We need to implement this" )]
        [Test]
        public void TestCommitWithLogMessage()
        {
        } 
	}
}
