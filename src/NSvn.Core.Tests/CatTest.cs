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
            Client.Cat( stream, path, Revision.Working, new ClientContext() );

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
            Client.Cat( stream, path, Revision.Head, new ClientContext() );

            string wrapperOutput = Encoding.ASCII.GetString( stream.ToArray() );
            Assertion.AssertEquals( "String from wrapper not the same as string from client",
                clientOutput, wrapperOutput );           

        }        
    }
}
