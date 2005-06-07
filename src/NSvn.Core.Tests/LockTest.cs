using System;
using System.IO;
using NUnit.Framework;

namespace NSvn.Core.Tests
{
	/// <summary>
	/// Summary description for LockTest.
	/// </summary>
	[TestFixture]
    public class LockTest : TestBase
	{
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractRepos();
            this.ExtractWorkingCopy();
        }

        [Test]
        public void TestBasicLock()
        {
            string filepath = Path.Combine( this.WcPath, "Form.cs" );
            this.Client.Lock(new string[]{ filepath }, "Moo", false);

            char lockStatus = this.RunCommand("svn", "status " + filepath)[5];
            Assertion.Assert("File not locked", lockStatus == 'K');
        }
	}
}
