// $Id$
using System;
using NUnit.Framework;
using NSvn.Common;
using NSvn.Core;
using System.Collections;

namespace NSvn.Tests
{
    /// <summary>
    /// Tests authentication issues.
    /// </summary>
    [TestFixture]
    public class AuthenticationTest
    {
        [Test]
        public void TestSimplePromptProvider()
        {
            AuthenticationProvider provider = AuthenticationProvider.GetSimplePromptProvider(
                new SimplePromptDelegate( this.SuccessPrompt ), 1 );
            RepositoryDirectory dir = new RepositoryDirectory( AUTHREPOS );
            dir.Context.AddAuthenticationProvider( provider );

            RepositoryResourceDictionary dict = dir.GetChildren();
            Assertion.Assert( "Expected some more results", dict.Count > 0 );            
        }       

        [Test]
        [ExpectedException(typeof(AuthorizationFailedException))]
        public void TestFailedAuthentication()
        {
            RepositoryDirectory dir = new RepositoryDirectory( AUTHREPOS );
            dir.GetChildren();
        }

        public SimpleCredential SuccessPrompt( string realm, string username )
        {
            return new SimpleCredential( "foo", "bar" );
        }


        private const string AUTHREPOS = "http://arild.no-ip.com:8088/svn/authtest";
        private const string USER = "foo";
        private const string PASS = "bar";
    }
}
