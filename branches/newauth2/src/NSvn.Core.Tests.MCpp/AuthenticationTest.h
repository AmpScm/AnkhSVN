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
                private:
                    NSvn::Core::SimpleCredential* SimplePrompt( System::String* realm, 
                        System::String* username );
                };
            }
        }
    }
}

