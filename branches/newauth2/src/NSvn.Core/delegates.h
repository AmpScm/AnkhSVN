// $Id$
#pragma once
#using <mscorlib.dll>

#include "credentials.h"


namespace NSvn
{
    namespace Core
    {
        public __gc class Notification;
        public __gc class CommitItem;
        public __gc class LogMessage;
        public __gc class Status;

        public __delegate void NotifyCallback( Notification* notification );
        public __delegate System::String* LogMessageCallback( CommitItem* items[] );
        public __delegate void LogMessageReceiver( LogMessage* logMessage );
        public __delegate void PromptCallback();

        public __delegate SimpleCredential* SimplePromptDelegate( 
            System::String* realm, String* username );

        public __delegate void StatusCallback( System::String* path, Status* status );
    }
}
