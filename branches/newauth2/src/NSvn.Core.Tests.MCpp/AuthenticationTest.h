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
                private:
                    NSvn::Core::SimpleCredential* SimplePrompt( System::String* realm, 
                        System::String* username );
                    NSvn::Core::SimpleCredential* NullSimplePrompt( System::String* realm, 
                        System::String* username );
                };
            }
        }
    }
}

