using System;
using NUnit.Framework;
using NSvn.Common;
using NSvn.Core;

namespace NSvn.Tests
{
	/// <summary>
	/// Tests authentication issues.
    /// </summary>
    [TestFixture]
	public class AuthenticationTest
	{
        [Test]
        public void TestSimpleProvider()
        {
            SimpleCredential cred = new SimpleCredential( USER, PASS );
            IAuthenticationProvider provider = new SimpleProvider( cred );

            RepositoryDirectory dir = new RepositoryDirectory( AUTHREPOS );
            dir.AuthenticationProviders.Add( provider );

            RepositoryResourceDictionary dict = dir.GetChildren();
            Assertion.Assert( "Expected some more results", dict.Count > 0 );            
        }

        [Test]
        public void TestCustomProvider()
        {
            RepositoryDirectory dir = new RepositoryDirectory( AUTHREPOS );
            dir.AuthenticationProviders.Add( new AuthProvider() );

            RepositoryResourceDictionary dict = dir.GetChildren();
            Assertion.Assert( "Expected more results", dict.Count > 0 );
        }

        private class AuthProvider : IAuthenticationProvider
        {

        
            #region Implementation of IAuthenticationProvider
            public NSvn.Common.ICredential NextCredentials()
            {
                if ( this.time++ > 3 )
                    return new SimpleCredential( USER, PASS );
                else
                    return new SimpleCredential( "", "" );
            }

            public NSvn.Common.ICredential FirstCredentials()
            {
                // this is wrong
                return new SimpleCredential( "kung", "foo" );
            }

            public string Kind
            {
                get
                {
                    return SimpleCredential.AuthKind;
                }
            }
            private int time = 0;
            #endregion
        }


        private const string AUTHREPOS = "http://arild.no-ip.com:8088/svn/authtest";
        private const string USER = "foo";
        private const string PASS = "bar";
    }
}
