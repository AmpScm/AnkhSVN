// $Id$
using System;
using NUnit.Framework;
using System.IO;
using NSvn.Common;
using System.Text;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests Client::Cat
    /// </summary>
    [TestFixture]
    public class CatTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractRepos();
            this.ExtractWorkingCopy();
        }

        /// <summary>
        /// Attemts to do a cat on a local working copy item
        /// </summary>
        [Test]
        public void TestCatFromWorkingCopy()
        {
            string path = Path.Combine( this.WcPath, "Form.cs" );

            string clientOutput = this.RunCommand( "svn", "cat " + path );

            MemoryStream stream = new MemoryStream();
            this.Client.Cat( stream, path, Revision.Working );

            string wrapperOutput = Encoding.ASCII.GetString( stream.ToArray() );
            Assertion.AssertEquals( "String from wrapper not the same as string from client",
                clientOutput, wrapperOutput );           

        }

        /// <summary>
        /// Calls cat on a repository item
        /// </summary>
        [Test]
        public void TestCatFromRepository()
        {
            string path = Path.Combine( this.ReposUrl, "Form.cs" );

            string clientOutput = this.RunCommand( "svn", "cat " + path );

            MemoryStream stream = new MemoryStream();
            this.Client.Cat( stream, path, Revision.Head );

            string wrapperOutput = Encoding.ASCII.GetString( stream.ToArray() );
            Assertion.AssertEquals( "String from wrapper not the same as string from client",
                clientOutput, wrapperOutput );      
        }        

        [Test]
        public void TestCatPeg()
        {
            string path = Path.Combine( this.ReposUrl, "Form.cs" );
            string toPath = Path.Combine( this.ReposUrl, "Moo.cs" );

            CommitInfo info = this.Client.Move( path, Revision.Head, toPath, false );
            
            string clientOutput = this.RunCommand( "svn", 
                string.Format( "cat {0}@{1} -r {2}", toPath, info.Revision, info.Revision-1 ) );

            MemoryStream stream = new MemoryStream();
            this.Client.Cat( stream, toPath, Revision.FromNumber(info.Revision),
                Revision.FromNumber(info.Revision-1) );

            string wrapperOutput = Encoding.ASCII.GetString( stream.ToArray() );
            Assertion.AssertEquals( "String from wrapper not the same as string from client",
                clientOutput, wrapperOutput ); 

        }
    }
}
