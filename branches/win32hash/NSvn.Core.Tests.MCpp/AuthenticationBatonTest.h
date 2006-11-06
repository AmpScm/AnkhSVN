// $Id$

#include "stdafx.h"
#using <NUnit.Framework.dll>
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
                public __gc class AuthenticationBatonTest
                {
                public:
                    [Test]
                    void TestSingleProvider();

                    [Test]
                    void TestParams();

                    [Test]
                    void TestDefaultUsernameAndPassword();

                    /*[Test]
                    public void TestMultipleProviders();*/
                private:
                    NSvn::Core::SimpleCredential* SimplePrompt( System::String* realm, 
                        System::String* username, bool maySave );
                };

            }
        }
    }
}