// $Id$
#include "Client.h"
#include "StringHelper.h"
#include "Notification.h"

#include <svn_client.h>
#include <apr_general.h>
#include "SvnClientException.h"


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


// implementation of Client::Status
//TO-DO:   Core::Revision to be implemented
StringDictionary* NSvn::Core::Client::Status( long* youngest, String* path, bool descend,
                                bool getAll, bool update, bool noIgnore, ClientContext* ctx )
{
    apr_hash_t* statushash = 0;
    Pool pool;
    String* truePath = CanonicalizePath( path );
    

    HandleError( svn_client_status( &statushash, youngest, StringHelper( truePath ), descend,
        getAll, update, noIgnore, ctx->ToSvnContext( pool ), pool ) );

    if ( statushash != 0 )
        return new StringDictionary();
    else
        return 0;
}

void NSvn::Core::Client::PropSet(String* propName, Byte propval[], String* target, bool recurse)
{
    Pool pool;
    svn_string_t propv;
    ByteArrayToSvnString( &propv, propval, pool );    
    String* truePath = CanonicalizePath( target );
    HandleError( svn_client_propset( StringHelper(propName), &propv, StringHelper(truePath), recurse, pool) );
}



String* NSvn::Core::Client::CanonicalizePath( String* path )
{
    return path->Replace( "\\", "/" );
}

void NSvn::Core::Client::ByteArrayToSvnString( svn_string_t* string, Byte array[], const Pool& pool  )
{
    string->len = array.Length;
    string->data = static_cast<char*>(pool.Alloc( array.Length));
    Marshal::Copy( array, 0, const_cast<char*>(string->data), array.Length );

}






