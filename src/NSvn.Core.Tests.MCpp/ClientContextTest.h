#pragma once
#include "stdafx.h"
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

                    /// <summary>Test that a ClientContext can be converted to a 
                    /// svn_client_ctx_t</summary>
                    [Test]
                    //[Ignore("Skip for now")]
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
