using NUnit.Framework;
using System.IO;

namespace NSvn.Core.Tests 
{
    /// <summary>
    /// Tests the NSvn.Client.MakeDir method
    /// </summary>
    [TestFixture]
    public class MakeDirTest : TestBase
    {
        [SetUp]
        public override void SetUp() 
        {
            base.SetUp();

            this.ExtractRepos();
            this.ExtractWorkingCopy();            
        }

        /// <summary>
        /// Tests creating a directory in the working copy
        /// </summary>
        [Test]
        public void TestMakeLocalDir()
        {
            string path = Path.Combine( this.WcPath, "foo" );
            ClientContext ctx = new ClientContext(( new NotifyCallback( this.NotifyCallback ) ) );
            CommitInfo info = Client.MakeDir( path, ctx );
            
            Assertion.AssertEquals( "MakeDir should return CommitInfo::Invalid for local operations",
               CommitInfo.Invalid, info );
            Assertion.AssertEquals( "Wrong number of notifications", 1, this.Notifications.Length );
            Assertion.AssertEquals( "Wrong status code", 'A', this.GetSvnStatus( path ) );
        }

        /// <summary>
        /// Tests creating a directory in the repository
        /// </summary>
        [Ignore("This doesn't work :-(" ) ]
        [Test]
        public void TestMakeRepositoryDir()
        {
            string url = this.ReposUrl + "moo";
            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );
            CommitInfo info = Client.MakeDir( url, ctx );

            Assertion.AssertEquals( "Wrong number of notifications", 1, this.Notifications.Length );
            Assertion.AssertEquals( "Author not anonymous", "anonymous", info.Author );
            //Assertion.AssertEquals( "
        }
    }
}
