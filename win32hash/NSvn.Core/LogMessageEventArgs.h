// $Id$

#include "stdafx.h"

namespace NSvn
{
    namespace Core
    {
        public __gc class CommitItem;

        public __gc class LogMessageEventArgs : public System::EventArgs
        {
        public:
            LogMessageEventArgs( CommitItem* items[] ) : items(items), message(0)
            {;}

            __property CommitItem* get_CommitItems() []
            { return this->items; }

            __property System::String* get_Message()
            { return this->message; }

            __property void set_Message( System::String* msg )
            { this->message = msg; }

        private:
            CommitItem* items[];
            System::String* message;
        };
    }
}