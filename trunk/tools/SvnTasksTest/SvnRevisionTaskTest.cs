using NAnt.Core;
using NUnit.Framework;

namespace Tests.SvnTasks
{
	/// <summary>
	/// Tests the Revision command
	/// </summary>
	[TestFixture]
	public class SvnRevisionTaskTest : BaseSvnTaskTest
	{
		protected readonly string m_revisionXml = @"<?xml version='1.0'?>
            <project>
				<svnrevision 
						url='{0}' />
            </project>";

		/// <summary>
		/// Create the directory needed for the test if it does not exist.
		/// </summary>
		[SetUp]
		protected override void SetUp () 
		{
			base.SetUp ();
			//CheckOutTestDir();
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
		/// Test that the revision is correctly obtained from the repository
		/// </summary>
		[Test]
		public void TestGetRevision()
		{
			object[] args = {m_testUrl + "/" + m_checkFile};
			Project project = CreateFilebasedProject( 
					FormatBuildFile(m_revisionXml, args), Level.Debug);
			
			string result = ExecuteProject(project);
			string revision = project.Properties["svn.revision"];
			Assert.IsTrue(int.Parse(revision) > 0);
		}
				
	}
}
