#pragma once
#include "stdafx.h"
#include "apr_time.h"

namespace NSvn
{
    namespace Core
    {
        System::DateTime AprTimeToDateTime( apr_time_t aprTime );
        apr_time_t DateTimeToAprTime( System::DateTime& dateTime );
    }
}