#include "utils.h"
#include "SvnClientException.h"
#include <svn_path.h>
#include <svn_utf.h>
#include "StringHelper.h"

using namespace System;
using namespace System::Runtime::InteropServices;

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

// Converts array of .NET strings to apr array of const char
apr_array_header_t* NSvn::Core::StringArrayToAprArray( String* strings[], Pool& pool )
{
    apr_array_header_t* array = apr_array_make( pool, strings->Length, sizeof( char* ) );
    
    // put the strings in the apr array
    for( int i = 0; i < strings->Length; ++i )
    {
        *((char**)apr_array_push(array)) = StringHelper( strings[i] ).CopyToPool( pool );
    }

    return array;
}

// Canonicalizes a path to the correct format expected by SVN
const char* NSvn::Core::CanonicalizePath( String* path, Pool& pool )
{
     const char* utf8path = StringHelper( path ).CopyToPoolUtf8( pool );

     // is this an URL?
     if ( !svn_path_is_url( utf8path ) )
     {
         // no we need to canonicalize and stuff
         // (most of this stuff was ripped from libsvn_subr/opt.c)
         const char* aprTarget;
         char* trueNamedTarget;
         // now we convert to the native APR encoding before canonicalizing the path
         HandleError( svn_path_cstring_from_utf8( &aprTarget, utf8path, pool ) );
         apr_status_t err = apr_filepath_merge( &trueNamedTarget, "", aprTarget, 
             APR_FILEPATH_TRUENAME, pool );

         // ENOENT means the file doesnt exist - we don't care
         if( err && !APR_STATUS_IS_ENOENT(err) )
             // TODO: fix this
             throw new SvnClientException( path );

         HandleError( svn_path_cstring_to_utf8( &utf8path, trueNamedTarget, pool ) );
     }

     return svn_path_canonicalize( utf8path, pool );
}

// Converts Byte array to svn_string_t
void NSvn::Core::ByteArrayToSvnString( svn_string_t* string, Byte array[], const Pool& pool  )
{
    string->len = array.Length;
    string->data = static_cast<char*>(pool.Alloc( array.Length));
    Marshal::Copy( array, 0, const_cast<char*>(string->data), array.Length );

}


String* NSvn::Core::ToNativePath( const char* path, Pool& pool )
{
    // convert to a native path    
    const char* cstringPath;
    HandleError( svn_utf_cstring_from_utf8( &cstringPath, path, pool ) );
    const char* nativePath = svn_path_local_style( cstringPath, pool );
    return StringHelper( nativePath );
}