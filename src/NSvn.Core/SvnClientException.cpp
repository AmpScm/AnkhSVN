#include "StdAfx.h"
#include "svnclientexception.h"
#include "StringHelper.h"

#using <mscorlib.dll>


using namespace NSvn::Core;

namespace
{
    String* FormatMessage( svn_error_t* err )
    {
        return String::Format( "svn client error: {0}\r\nat {1} : line {2}", 
            StringHelper( err->message ),
            StringHelper( err->file ), __box( err->line ) );
    }
}

SvnClientException* NSvn::Core::SvnClientException::FromSvnError( svn_error_t* err )
{
    // we need to get the child errors as well
    return CreateExceptionsRecursively( err );
}

SvnClientException* NSvn::Core::SvnClientException::CreateExceptionsRecursively( svn_error_t* err )
{
    // is there a child error?
    if ( err->child != 0 )
    {
        // yes, create a nested exception
        SvnClientException* child = CreateExceptionsRecursively( err->child );
        return new SvnClientException( FormatMessage( err ), child );
    }
    else
        // nope, just include the message
        return new SvnClientException( FormatMessage( err ) );
}
