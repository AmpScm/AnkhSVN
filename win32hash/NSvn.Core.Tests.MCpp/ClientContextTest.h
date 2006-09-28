// $Id$
#pragma once
#include "stdafx.h"
#include "ClientContext.h"
#include "Client.h"
#include "SvnClientException.h"

using namespace NUnit::Framework;

namespace NSvn
{
    namespace Core
    {
        public __gc class NotificationEventArgs;
        public __gc class LogMessageEventArgs;

        namespace Tests
        {
            namespace MCpp
            {
                [TestFixture]
                public __gc class ClientContextTest : public Client
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
                    void TestNotification();


                    /// <summary>Test the log message stuff</summary>
                    [Test]
                    void TestLogMessage();

                    /// <summary>Guess what we're testing here?</summary>
                    [Test]
                    void TestCancel();

                protected public:
                    virtual void OnNotification( NotificationEventArgs* notification );
                    virtual void OnLogMessage( LogMessageEventArgs* args );
                    virtual void OnCancel( CancelEventArgs* args );
                private:
                    bool flag;
                    NotificationEventArgs* notification;



                };


            }
        }
    }
}

