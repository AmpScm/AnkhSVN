#pragma once
#using <mscorlib.dll>
#using <nunit.framework.dll>
#include "ClientContext.h"

using namespace NUnit::Framework;

namespace NSvn
{
    namespace Core
    {

        namespace Tests
        {
            namespace MCpp
            {

                [TestFixture]
                public __gc class ClientContextTest
                {
                public:
                    [SetUp]
                    void Init()
                    { 
                        this->notification = 0;
                    }

                    [Test]
                    void TestSvnContextTConversion();

                private:
                    void NotifyCallback( Notification* notification );
                    bool flag;
                    Notification* notification;



                };

                
            }
        }
    }
}
