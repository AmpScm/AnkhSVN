// $Id$
using NUnit.Framework;
using System.IO;
using System.Text.RegularExpressions;

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
            CommitInfo info = this.Client.MakeDir( new string[]{ path } );
            
            Assert.AreEqual( CommitInfo.Invalid, info,
                "MakeDir should return CommitInfo::Invalid for local operations" );
            Assert.AreEqual( 'A', this.GetSvnStatus( path ), "Wrong status code" );
        }

        /// <summary>
        /// Tests creating a directory in the repository
        /// </summary>
        [Test]
        public void TestMakeRepositoryDir()
        {
            string url = this.ReposUrl + "mooNewDirectory";
            CommitInfo info = this.Client.MakeDir( new string[]{ url } );

            string output = this.RunCommand( "svn", "ls " + this.ReposUrl );
            Assert.IsTrue( Regex.IsMatch( output, @"mooNewDirectory/" ), "No new dir found: " + output );

        }
    }
}
