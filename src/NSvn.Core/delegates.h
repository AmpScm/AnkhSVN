// $Id$
#pragma once
#using <mscorlib.dll>
#include "Notification.h"

namespace NSvn
{
    namespace Core
    {
        public __delegate void NotifyCallback( Notification* notification );
        public __delegate void LogMessageCallback();
        public __delegate void LogMessageReceiver();
        public __delegate void PromptCallback();
    }
}
