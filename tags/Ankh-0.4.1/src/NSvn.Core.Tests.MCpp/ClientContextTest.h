// $Id$
#pragma once
#include "stdafx.h"
#include "ClientContext.h"
#include "SvnClientException.h"

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
                    /// svn_client_ctx_t with a working notify callback</summary>
                    [Test]
                    void TestNotifyCallback();


                    /// <summary>Test the log message stuff</summary>
                    [Test]
                    void TestLogMessageCallback();

                private:
                    void NotifyCallback( Notification* notification );
                    System::String* LogMsgCallback( CommitItem* items[] );
                    bool flag;
                    Notification* notification;



                };


            }
        }
    }
}

