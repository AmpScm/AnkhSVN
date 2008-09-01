using System;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;

using NSvn.Common;

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
                    Recurse.Full );
                string url = this.Client.SingleStatus( this.WcPath ).Entry.Url;
                Assert.IsTrue( url.StartsWith( "svn://localhost" ) );
            }
            finally
            {
                svnserve.CloseMainWindow();
                System.Threading.Thread.Sleep(500);
                if(!svnserve.HasExited)
                {
                    svnserve.Kill();
                    svnserve.WaitForExit();
                }
            }
        }

        
    }
}
