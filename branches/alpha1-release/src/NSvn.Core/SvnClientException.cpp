// $Id$
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

    SvnClientException* CreateException( svn_error_t* err, SvnClientException* child = 0 )
    {
        switch( err->apr_err )
        {
        case SVN_ERR_RA_NOT_AUTHORIZED:
            return new AuthorizationFailedException( child );
            break;
        case SVN_ERR_WC_LOCKED:
            return new WorkingCopyLockedException( child );
            break;
        default:
            return new SvnClientException( FormatMessage( err ), child );
            break;
        }
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
        return CreateException( err, child );
    }
    else
        // nope, just include the message
        return CreateException( err );;
}
