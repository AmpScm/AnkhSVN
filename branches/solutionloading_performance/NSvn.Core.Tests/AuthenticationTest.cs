using System;
using System.IO;
using NUnit.Framework;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests the Authentication class.
    /// </summary>
    public class AuthenticationTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

       
        /// <summary>
        ///  Test connecting to an SSL repository and failing.
        /// </summary>
        [Test]
        public void TestSslServerPromptFail()
        {
            this.Client.AuthBaton.Add( AuthenticationProvider.GetSslServerTrustPromptProvider( new 
                SslServerTrustPromptDelegate( this.SslServerTrustFailCallback  ) ) );           

            try
            {
                DirectoryEntry[] entries = 
                    this.Client.List( SSLTESTREPOS, Revision.Head, false );
                Assertion.Fail( "Client.List should have thrown an exception" ); 
            }
            catch( SvnClientException )
            {}

            Assertion.Assert( "Callback not called", this.callbackCalled );
        }

        /// <summary>
        /// Test a successfull connection to an SSL repository.
        /// </summary>
        [Test]
        public void TestSslServerPromptSuccess()
        {
            this.Client.AuthBaton.Add( AuthenticationProvider.GetSslServerTrustPromptProvider( new 
                SslServerTrustPromptDelegate( this.SslServerTrustAcceptCallback  ) ) );
                      
            DirectoryEntry[] entries = 
                this.Client.List( SSLTESTREPOS, Revision.Head, false );
           
            Assertion.Assert( "No entries returned", entries.Length > 0 );
            Assertion.Assert( "Callback not called", this.callbackCalled );
        }

        [Test]
        public void TestSslServerFileSuccess()
        {
            this.Client.AuthBaton.Add( 
                AuthenticationProvider.GetSslServerTrustFileProvider() );
            this.Client.AuthBaton.Add( 
                AuthenticationProvider.GetSslServerTrustPromptProvider( new 
                SslServerTrustPromptDelegate( this.SslServerTrustAcceptCallback  ) ) );
            

            // first get the certificate to be trusted
            this.acceptedFailures = SslFailures.CertificateAuthorityUnknown |
                SslFailures.CertificateNameMismatch | 
                SslFailures.Other;
            this.maySave = true;

            
            this.Client.List( SSLTESTREPOS, Revision.Head, false );

            // now try to get them to take it from the config dir
            DirectoryEntry[] entries = this.Client.List( SSLTESTREPOS,
                Revision.Head, false );
            Assertion.Assert( "No entries returned", entries.Length > 0 );        

            
        }


        SslServerTrustCredential SslServerTrustFailCallback( string realm, 
            SslFailures failures, SslServerCertificateInfo info, bool maySave )
        {
            this.callbackCalled = true;
            return null;//new SslServerTrustCredential();
        }

        SslServerTrustCredential SslServerTrustAcceptCallback( string realm,
            SslFailures failures, SslServerCertificateInfo info, bool maySave )
        {
            Assertion.Assert( "Certificate authority should have been unknown", 
                (failures & SslFailures.CertificateAuthorityUnknown) != 0 );
            Assertion.Assert( "There should have been a mismatch.", 
                (failures & SslFailures.CertificateNameMismatch) != 0 );

            SslServerTrustCredential cred = new SslServerTrustCredential();
            cred.AcceptedFailures = this.acceptedFailures;
            cred.MaySave = this.maySave;

            this.callbackCalled = true;

            return cred;
        }

        private SslFailures acceptedFailures = 0;
        private bool maySave = false;
        private bool callbackCalled = false;
        private const string SSLTESTREPOS=@"https://ankhsvn.com/svn/test";
    }
}
