using System;
using NUnit.Framework;

namespace NSvn.Core.Tests
{
	/// <summary>
	/// A test for Client.Relocate
	/// </summary>
	[TestFixture]
	public class RelocateTest : TestBase
	{
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractWorkingCopy();
            this.ExtractRepos();
        }

        [Test]
        public void Test()
        {
            this.Client.Relocate( this.WcPath, "file", "file", true );
            string url = this.Client.SingleStatus( this.WcPath ).Entry.Url;
            Assertion.Assert( url.StartsWith( "file" ) );
        }
	}
}
