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
        case SVN_ERR_WC_NOT_DIRECTORY:
        case SVN_ERR_WC_NOT_FILE:
            return new NotVersionControlledException( child );
            break;
        case SVN_ERR_FS_TXN_OUT_OF_DATE:
            return new ResourceOutOfDateException( child );
            break;
        case SVN_ERR_ILLEGAL_TARGET:
            return new IllegalTargetException( child );
            break;
        case SVN_ERR_CANCELLED:
            return new OperationCancelledException( child );
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
    SvnClientException* exception = 0;

    if ( err->child != 0 )
    {
        // yes, create a nested exception
        SvnClientException* child = CreateExceptionsRecursively( err->child );
        exception = CreateException( err, child );
    }
    else
        // nope, just include the message
        exception = CreateException( err );;

    exception->errorCode = err->apr_err;
    exception->svnError = StringHelper( err->message );
    return exception;
}
