using System.IO;
using NAnt.Core;
using NUnit.Framework;


namespace Tests.SvnTasks
{
	/// <summary>
	/// Tests the Update command
	/// </summary>
	[TestFixture]
	public class SvnUpdateTaskTest : BaseSvnTaskTest
	{
		protected readonly string m_updateXml = @"<?xml version='1.0'?>
            <project>
				<svnupdate 
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
		/// Tests that the checkfile is updated from the repository
		/// </summary>
		[Test]
		public void TestUpdate()
		{
			object[] args = {m_localTestDir};
			string result = this.RunBuild(FormatBuildFile(m_updateXml, args), Level.Debug);
			string checkFilePath = Path.Combine(m_localTestDir, m_checkFile);
			Assert.IsTrue(Directory.Exists(m_localTestDir));
			Assert.IsTrue(File.Exists(checkFilePath), 
                "File does not exist, update probably did not work.");
		}
		
	}
}
