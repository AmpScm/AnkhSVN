using System.Globalization;
using System.IO;
using NAnt.Core;
using NUnit.Framework;
using Tests.NAnt.Core;

namespace Tests.SvnTasks
{
	/// <summary>
	/// Base Class for all SvnTask tests
	/// </summary>
	public class BaseSvnTaskTest : BuildTestBase
	{
		protected readonly string m_localTestDir = @"C:\testSvnTasks\";
		protected readonly string m_localExportDir = @"C:\testExportSvnTasks\";
		protected readonly string m_testUrl= "http://ankhsvn.com:8088/svn/test";
		
		protected readonly string m_checkFile = "SvnTasksTest.txt";

		
		protected readonly string m_checkoutXml = @"<?xml version='1.0'?>
            <project>
				<svncheckout 
						localDir='{0}'
						url='{1}'
						verbose='true' />
            </project>";

		/// <summary>
		/// Removes the readonly attribute recursively of the path specified
		/// </summary>
		/// <param name="path"></param>
		protected void MakeReadable(string path)
		{	
			foreach(string filename in Directory.GetFiles(path))
			{
				File.SetAttributes(filename, FileAttributes.Normal);
			}
			foreach(string dirname in Directory.GetDirectories(path))
			{
				MakeReadable(dirname);
			}
		}
		/// <summary>
		/// Performs a checkout on the testing dirs
		/// </summary>
		protected void CheckOutTestDir()
		{
			RemoveTestDirs();
			Directory.CreateDirectory(m_localTestDir);
			
			object[] args = {m_localTestDir, m_testUrl};

			string checkFilePath = Path.Combine(m_localTestDir, m_checkFile );

			string result = this.RunBuild(FormatBuildFile(m_checkoutXml, args), Level.Debug);
			Assert.IsTrue(File.Exists(checkFilePath), "File does not exist, checkout probably did not work.");
			
		}

		/// <summary>
		/// Deletes the testing dirs
		/// </summary>
		protected void RemoveTestDirs()
		{	
			if (Directory.Exists(m_localTestDir))
			{
				MakeReadable(m_localTestDir);
				Directory.Delete(m_localTestDir,true);
			}
			if (Directory.Exists(m_localExportDir))
			{
				MakeReadable(m_localExportDir);
				Directory.Delete(m_localExportDir,true);
			}	
		}
		/// <summary>
		/// Fommats the Build file path
		/// </summary>
		/// <param name="baseFile"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		protected string FormatBuildFile(string baseFile, object[] args) 
		{
			return string.Format(CultureInfo.InvariantCulture, baseFile, args);
		}
	}
}
