using System.IO;
using NAnt.Core;
using NUnit.Framework;

namespace Tests.SvnTasks
{
	/// <summary>
	/// Tests the checkout command 
	/// </summary>
	[TestFixture]
	public class CheckoutTaskTest : BaseSvnTaskTest 
	{	
		/// <summary>
		/// Create the directory needed for the test if it does not exist.
		/// </summary>
		[SetUp]
		protected override void SetUp () 
		{
			base.SetUp ();
			RemoveTestDirs();
			Directory.CreateDirectory(m_localTestDir);
            
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
		/// Tests that the directory for the subversion checkout gets created and
		/// that at least the checkfile file comes down from the 
		/// repository.
		/// </summary>
		[Test]
		public void TestSvnCheckout () 
		{
			object[] args = {m_localTestDir, m_testUrl};

			string checkFilePath = Path.Combine(m_localTestDir, m_checkFile );

			string result = this.RunBuild(FormatBuildFile(m_checkoutXml, args), Level.Debug);
			Assert.IsTrue(File.Exists(checkFilePath), 
                "File does not exist, checkout probably did not work.");
		}

       
	}
}
