using System;
using System.IO;
using NUnit.Framework;

namespace NSvn.Core.Tests
{
	/// <summary>
	/// Tests for the Client::Checkout method
	/// </summary>
	[TestFixture]
	public class DeleteTest : TestBase
	{
		[SetUp]
		public override void SetUp() 
		{
			base.SetUp();
			this.ExtractRepos();
			this.ExtractWorkingCopy();
			this.ExtractWorkingCopy();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
		}

		
		/// <summary>
		/// Tests deleting a file in a working copy
		/// </summary>
		/// //TODO: Implement the variable admAccessBaton
		[Test]
		public void TestDeleteWCFile()
		{
			string path = Path.Combine( this.WcPath, "Form.cs" );
			ClientContext ctx = new ClientContext( );

			CommitInfo info = Client.Delete( path, false, ctx );

			Assertion.Assert( "File not deleted", !File.Exists( path ) );
			Assertion.AssertEquals( "File not deleted", 'D', this.GetSvnStatus( path ) );
           
		}

		/// <summary>
		/// Tests deleting a directory in the repository
		/// </summary>
		//TODO: Implement the variable admAccessBaton
		[Test]
		public void TestDeleteReposDir()
		{
			string path = Path.Combine( this.ReposUrl, @"obj" );
			ClientContext ctx = new ClientContext( );

			CommitInfo info = Client.Delete( path, false, ctx );

			String cmd = this.RunCommand( "svn", "list " + this.ReposUrl );
			Assertion.Assert( "Directory wasn't deleted ", cmd.IndexOf( "obj" ) == -1 );
		}

	}
}
