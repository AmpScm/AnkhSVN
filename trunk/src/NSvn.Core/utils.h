// $Id$
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

        //TODO: doc comments here
        using namespace System;

        System::DateTime AprTimeToDateTime( apr_time_t aprTime );

        apr_time_t DateTimeToAprTime( System::DateTime& dateTime );

        System::DateTime ParseDate( const char* date, apr_pool_t* pool );


        void ByteArrayToSvnString( svn_string_t* string, Byte array[], 
            const Pool& pool );

        Byte SvnStringToByteArray( svn_string_t* string )[];

        const char* CanonicalizePath( String* path, Pool& pool );

        String* ToNativePath( const char* path, apr_pool_t* pool );

        apr_array_header_t* StringArrayToAprArray( String* strings[], 
            bool isPath, Pool& pool );

        String* AprArrayToStringArray( apr_array_header_t* aprArray, apr_pool_t* pool ) [];

        int AprArrayToIntArray( apr_array_header_t* aprArray ) __gc [];

		String* Utf8ToString( const char* charPtr, apr_pool_t* pool );
		const char* StringToUtf8( String* string, apr_pool_t* pool );

    }
}