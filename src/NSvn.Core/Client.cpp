// $Id$
#include "Client.h"
#include "StringHelper.h"
#include "Notification.h"

#include <svn_client.h>
#include <apr_general.h>
#include "SvnClientException.h"

namespace
{
    inline void HandleError( svn_error_t* err )
    {
        if ( err != 0 )
            throw NSvn::Core::SvnClientException::FromSvnError( err );
    }
}

// implementation of Client::Add
void NSvn::Core::Client::Add( String* path, bool recursive, ClientContext* ctx )
{
    Pool pool;

    String* truePath = CanonicalizePath( path );
    HandleError( svn_client_add( StringHelper( truePath ), recursive, ctx->ToSvnContext( pool ), pool ) );
}
// implementation of Client::MakeDir
NSvn::Core::CommitInfo* NSvn::Core::Client::MakeDir( String* path, ClientContext* ctx )
{
    Pool pool;

    String* truePath = CanonicalizePath( path );
    svn_client_commit_info_t* commitInfo = 0;
    HandleError( svn_client_mkdir( &commitInfo, StringHelper( truePath ), ctx->ToSvnContext( pool ), 
        pool ) );

    if ( commitInfo != 0 )
        return new CommitInfo( commitInfo );
    else
        return CommitInfo::Invalid;
}
// implemenentation of Client::Cleanup
void NSvn::Core::Client::Cleanup( String* directory )
{
    Pool pool;
    String* truePath = CanonicalizePath( directory );
    HandleError( svn_client_cleanup( StringHelper( truePath ), pool ) );
}

// implementation of Client::Revert
void NSvn::Core::Client::Revert(String* path, bool recursive, ClientContext* ctx )
{
    Pool pool;
    String* truePath = CanonicalizePath( path );

    HandleError( svn_client_revert( StringHelper( truePath ), recursive, 
        ctx->ToSvnContext (pool), pool));
}    

// implementation of Client::Resolve
void NSvn::Core::Client::Resolve(String* path, bool recursive, ClientContext* ctx )
{
    Pool pool;
    String* truePath = CanonicalizePath( path );

    HandleError( svn_client_resolve( StringHelper( truePath ), recursive, 
        ctx->ToSvnContext (pool), pool));
}    


String* NSvn::Core::Client::CanonicalizePath( String* path )
{
    return path->Replace( "\\", "/" );
}




