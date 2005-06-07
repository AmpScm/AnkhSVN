using System;
using System.IO;
using NUnit.Framework;

namespace NSvn.Core.Tests
{
	/// <summary>
	/// Summary description for UnlockTest.
	/// </summary>
	[TestFixture]
    public class UnlockTest : TestBase
	{
        [SetUp]
        public override void SetUp()
        {
            base.SetUp ();
            this.ExtractRepos();
            this.ExtractWorkingCopy();
        }

        [Test]
        public void BasicUnlockTest()
        {
            string filepath = Path.Combine( this.WcPath, "Form.cs" );

            this.RunCommand("svn", "lock " + filepath);
            
            char lockStatus;
            lockStatus = this.RunCommand("svn", "status " + filepath)[5];
            Assertion.Assert("file not locked", 'K' == lockStatus);
            
            this.Client.Unlock( new string[] { filepath }, false );

            Assertion.Assert("file not unlocked", this.RunCommand("svn", "status " + filepath).Length == 0);
        }
	}
}
