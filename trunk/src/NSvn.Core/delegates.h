// $Id$
#pragma once
#using <mscorlib.dll>
#include "Notification.h"
#include "CommitItem.h"

namespace NSvn
{
    namespace Core
    {
        public __delegate void NotifyCallback( Notification* notification );
        public __delegate String* LogMessageCallback( CommitItem* items[] );
        public __delegate void LogMessageReceiver();
        public __delegate void PromptCallback();
    }
}
