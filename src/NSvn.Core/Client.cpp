// $Id$
#include "Client.h"
#include "StringHelper.h"
#include "Notification.h"

#include <svn_client.h>
#include <apr_general.h>
#include "SvnClientException.h"

namespace
{
    // global object that ensures that APR is initialized
    class Initializer
    {
    public:
        Initializer()
        {
            apr_initialize();
        }

    } initializerDummy;
}

// implementation of Client::Add
void NSvn::Core::Client::Add( String* path, bool recursive, ClientContext* ctx )
{
    Pool pool;
    String* truePath = CanonicalizePath( path );
    svn_error_t* err = svn_client_add( StringHelper( truePath ), recursive, ctx->ToSvnContext( pool ), pool );
    if ( err != 0 )
        throw SvnClientException::FromSvnError( err );
    
}


String* NSvn::Core::Client::CanonicalizePath( String* path )
{
    return path->Replace( "\\", "/" );
}




