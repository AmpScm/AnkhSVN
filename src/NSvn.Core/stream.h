#pragma once
#include "stdafx.h"
#include <svn_io.h>
#include "Pool.h"

namespace NSvn
{
    namespace Core
    {
        svn_stream_t* CreateSvnStream( System::IO::Stream* stream, Pool& pool );
    }
}
