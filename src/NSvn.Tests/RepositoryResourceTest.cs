// $Id$
using System;
using NUnit.Framework;
using NSvn.Core;

namespace NSvn.Tests
{
    /// <summary>
    /// Tests the RepositoryResource class.
    /// </summary>
    [TestFixture]
    public class RepositoryResourceTest
    {
        /// <summary>
        /// Tests the Name property.
        /// </summary>
        [Test]
        public void TestName()
        {
            RepositoryResource file = new RepositoryFile( "http://www.porn.com/foo/bar.txt" );
            file.Context.AddAuthenticationProvider( AuthenticationProvider.GetUsernameProvider() );

            Assertion.AssertEquals( "Wrong file name", "bar.txt", file.Name );

            Assertion.AssertEquals( "Wrong dir name", "bar", 
                new RepositoryDirectory( "http://www.porn.com/foo/bar/" ).Name );
            Assertion.AssertEquals( "Wrong dir name", "com", 
                new RepositoryDirectory( "file:///J:/foo/bar/com" ).Name );
            Assertion.AssertEquals( "Wrong dir name", "moo",
                new RepositoryDirectory( "http://www.porn.com:666/foo/shoo/moo" ).Name );
        }

    }
}
