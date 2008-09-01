// $Id$

#include "stdafx.h"
#include "svnenums.h"

namespace NSvn
{
    namespace Core
    {

        public __gc class CancelEventArgs : public System::EventArgs
        {
        public:
            CancelEventArgs() : cancel( false )
            {;}
           
            __property bool get_Cancel()
            { return this->cancel; }

            __property void set_Cancel( bool cancel )
            { this->cancel = cancel; }

        private:
            bool cancel;
        };
    }
}