#include "utils.h"

using namespace System;

DateTime NSvn::Core::AprTimeToDateTime( apr_time_t aprTime )
{
    TimeSpan t( aprTime* 10 );
    //apr_time_t is the number of microseconds since the epoch
    return DateTime( 1970, 1, 1 ).Add( t );
}

apr_time_t NSvn::Core::DateTimeToAprTime( DateTime& dateTime )
{
    DateTime epoch( 1970, 1, 1 );
    TimeSpan t = dateTime - epoch;
    //apr_time_t is the number of microseconds since the epoch
    return static_cast<apr_time_t>(t.TotalMilliseconds * 1000);
}

