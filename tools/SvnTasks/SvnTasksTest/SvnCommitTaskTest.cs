using System;
using System.IO;
using NAnt.Core;
using NUnit.Framework;


namespace Tests.SvnTasks
{
	/// <summary>
	/// Tests the Commit command
	/// </summary>
	[TestFixture]
	public class SvnCommitTaskTest : BaseSvnTaskTest
	{
		protected readonly string m_commitXml = @"<?xml version='1.0'?>
            <project>
				<svncommit 
						localDir='{0}'
						recursive='true' />
            </project>";

		/// <summary>
		/// Create the directory needed for the test if it does not exist.
		/// </summary>
		[SetUp]
		protected override void SetUp () 
		{
			base.SetUp ();
			CheckOutTestDir();
		}

		/// <summary>
		/// Remove the directory created by the checkout/ update.
		/// </summary>
		[TearDown]
		protected override void TearDown () 
		{
			base.TearDown ();
			RemoveTestDirs();
		}
		/// <summary>
		/// Tests that the checkfile file is committed to the repository
		/// </summary>
		[Test]
		public void TestCommit()
		{
			string checkFilePath = Path.Combine(m_localTestDir, m_checkFile);

			StreamWriter checkFile = new StreamWriter( File.OpenWrite(checkFilePath));
			checkFile.WriteLine("Commit Test Last Run: " + DateTime.Now.ToString());
			checkFile.Close();

			object[] args = {m_localTestDir};


			string result = this.RunBuild(FormatBuildFile(m_commitXml, args), Level.Debug);
			//don't know how to assert this worked yet
			
		}
		
	}
}
