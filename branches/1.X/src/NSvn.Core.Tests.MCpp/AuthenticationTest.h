#pragma once
#using <nunit.framework.dll>
#using <mscorlib.dll>

#include "../NSvn.Core/credentials.h"

namespace NSvn
{
    namespace Core
    {
        namespace Tests
        {
            namespace MCpp
            {
                using namespace NUnit::Framework;

                [TestFixture]
                public __gc class AuthenticationTest
                {
                public:
                    [Test]
                    void TestGetSimplePromptProvider();

                    [Test]
                    void TestGetUsernameProvider();

                    [Test]
                    void TestGetSslClientCertFileProvider();

                    [Test]
                    void TestGetSslClientCertPasswordFileProvider();

                    [Test]
                    void TestGetSslClientCertPromptProvider();

                    [Test]
                    void TestGetSslClientCertPasswordPromptProvider();
                private:
                    NSvn::Core::SimpleCredential* SimplePrompt( System::String* realm, 
                        System::String* username, bool maySave );
                    NSvn::Core::SimpleCredential* NullSimplePrompt( System::String* realm, 
                        System::String* username, bool maySave );

                    NSvn::Core::SslClientCertificateCredential* CertificatePrompt( 
                        String* realm, bool maySave );
                    NSvn::Core::SslClientCertificateCredential* NullCertificatePrompt(
                        String* realm, bool maySave );

                    NSvn::Core::SslClientCertificatePasswordCredential* PasswordPrompt( 
                        String* realm, bool maySave);
                    NSvn::Core::SslClientCertificatePasswordCredential* NullPasswordPrompt( 
                        String* realm, bool maySave );
                };
            }
        }
    }
}

