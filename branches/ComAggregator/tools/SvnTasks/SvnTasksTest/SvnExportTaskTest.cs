using System.IO;
using NAnt.Core;
using NUnit.Framework;

namespace Tests.SvnTasks
{
	/// <summary>
	/// Tests the Export command
	/// </summary>
	[TestFixture]
	public class SvnExportTaskTest : BaseSvnTaskTest
	{
		protected readonly string m_exportXml = @"<?xml version='1.0'?>
            <project>
				<svnexport 
						localDir='{0}'
						url='{1}'
						verbose='true'
						force='true' />
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
		/// Tests that the directory for the subversion export gets created and
		/// that at least the checkfile file comes down from the 
		/// repository.
		/// </summary>
		[Test]
		public void TestExport()
		{
			object[] args = {m_localExportDir, m_testUrl};

			string checkFilePath = Path.Combine(m_localExportDir, m_checkFile);

			string result = this.RunBuild(FormatBuildFile(m_exportXml, args), Level.Debug);
			Assert.IsTrue(Directory.Exists(m_localExportDir));
			Assert.IsTrue(File.Exists(checkFilePath), 
                "File does not exist, export probably did not work." );
		}
		
	}
}
