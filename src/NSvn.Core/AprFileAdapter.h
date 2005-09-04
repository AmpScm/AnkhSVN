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
            AprFileAdapter( System::IO::Stream* stream, apr_pool_t* pool  ) :
              stream(stream), errorMessage( 0 ), pool(pool)
              {;}
              apr_file_t* Start();
              void WaitForExit();            
        private:
            void Read();
            System::Threading::Thread* thread;
            System::IO::Stream* stream;
            apr_file_t __nogc* outfile;
            static const int BUFSIZE = 500;
            System::String* errorMessage;
			apr_pool_t* pool;
        };
    }
}
