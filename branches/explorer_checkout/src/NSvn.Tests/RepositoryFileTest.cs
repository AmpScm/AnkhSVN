// $Id: RepositoryFile.cs 594 2003-05-22 15:32:54Z Arild $
using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using NSvn.Core;


namespace NSvn.Tests
{
    /// <summary>
    /// Tests the RepositoryFile class.
    /// </summary>
    [TestFixture]
    public class RepositoryFileTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractRepos();
        }

        [Test]
        public void TestCat()
        {
            string formUrl = Path.Combine( this.ReposUrl, "Form.cs" );

            string clientOutput = this.RunCommand( "svn", "cat " + formUrl );
            
            RepositoryFile file = new RepositoryFile( formUrl );
            MemoryStream stream = new MemoryStream( 1000 );
            file.Cat( stream );

            string apiOutput = Encoding.UTF8.GetString( stream.ToArray() );

            Assertion.AssertEquals( "Output from svn cat and API not the same", clientOutput, apiOutput );

            // try an older revision
            clientOutput = this.RunCommand( "svn", "cat -r 3 " + formUrl );
            file.Revision = Revision.FromNumber( 3 );

            stream.Seek( 0L, SeekOrigin.Begin );
            file.Cat( stream );

            apiOutput = Encoding.UTF8.GetString( stream.ToArray() );

            Assertion.AssertEquals( "Output from svn cat and API not the same at rev 3", clientOutput, apiOutput );
        }


    }  
}
