// $Id$
#pragma once
#include "stdafx.h"
//#include "svnenums.h"

namespace NSvn
{
    namespace Core
    {
        using namespace System;

        public __gc class ProgressEventArgs : public EventArgs
        {
        public:
            ProgressEventArgs(long progress, long total)
            {
                this->progress = progress;
                this->total = total;
            }

            ///<summary>The number of bytes already transferred</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property long get_Progress()
            { return this->progress; }

            ///<summary>The total number of bytes to transfer or -1 if it's not known</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property long get_Total()
            { return this->total; }

        private:
            long total;
            long progress;
        };

    }
}

