// $Id$
#pragma once
#include "stdafx.h"
#include "Pool.h"
#include <apr_file_io.h>

namespace NSvn
{
    namespace Core
    {
        private __gc class AprFileAdapter
        {
        public:
            AprFileAdapter( System::IO::Stream* stream  ) :
              stream(stream), errorMessage( 0 )
              {;}
              apr_file_t* Start(Pool& pool);
              void WaitForExit();            
        private:
            void Read();
            System::Threading::Thread* thread;
            System::IO::Stream* stream;
            apr_file_t __nogc* outfile;
            static const BUFSIZE = 500;
            System::String* errorMessage;
        };
    }
}
