#pragma once
#include "stdafx.h"
#include <apr_time.h>
#include <apr_tables.h>
#include <svn_string.h>
#include "Pool.h"

namespace NSvn
{
    namespace Core
    {
        using namespace System;

        System::DateTime AprTimeToDateTime( apr_time_t aprTime );
        apr_time_t DateTimeToAprTime( System::DateTime& dateTime );
        void ByteArrayToSvnString( svn_string_t* string, Byte array[], 
            const Pool& pool );
        const char* CanonicalizePath( String* path, Pool& pool );

        String* ToNativePath( const char* path, Pool& pool );

        apr_array_header_t* StringArrayToAprArray( String* strings[], 
            Pool& pool );
    }
}