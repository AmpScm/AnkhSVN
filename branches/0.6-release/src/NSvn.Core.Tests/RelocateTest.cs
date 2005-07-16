using System;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;

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
            // start a svnserve process on this repos
            Process svnserve = this.StartSvnServe( this.ReposPath );
            try
            {
                this.Client.Relocate( 
                    this.WcPath, 
                    "file:///tmp/repos", 
                    String.Format("svn://localhost:{0}/", PortNumber), 
                    true );
                string url = this.Client.SingleStatus( this.WcPath ).Entry.Url;
                Assertion.Assert( url.StartsWith( "svn://localhost" ) );
            }
            finally
            {
                svnserve.Kill();
            }
        }

        
	}
}
