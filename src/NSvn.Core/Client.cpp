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
// implementation of Client::PropSet
void NSvn::Core::Client::PropSet(String* propName, Byte propval[], String* target, bool recurse)
{
    Pool pool;
    svn_string_t propv;
    ByteArrayToSvnString( &propv, propval, pool );    
    String* truePath = CanonicalizePath( target );
    HandleError( svn_client_propset( StringHelper(propName), &propv, StringHelper(truePath), recurse, pool) );
}

// implementation of Client::Checkout
void NSvn::Core::Client::Checkout( String* url, String* path, Revision* revision, 
                                  bool recurse, ClientContext* ctx )
{
    Pool pool;
    String* truePath = CanonicalizePath( path );
    HandleError( svn_client_checkout( StringHelper( url ), StringHelper( truePath ), 
        revision->ToSvnOptRevision( pool ), recurse, ctx->ToSvnContext( pool ), pool ) );
}

// implementation of Client::Update
void NSvn::Core::Client::Update( String* path, Revision* revision, bool recurse, 
                                ClientContext* ctx )
{
    Pool pool;
    String* truePath = CanonicalizePath( path );
    HandleError( svn_client_update( StringHelper(truePath), revision->ToSvnOptRevision( pool ),
        recurse, ctx->ToSvnContext( pool ), pool ) );
}

// implementation of Client::Commit
NSvn::Core::CommitInfo* NSvn::Core::Client::Commit( String* targets[], bool nonRecursive, ClientContext* ctx )
{
    Pool pool;
    apr_array_header_t* aprArrayTargets = StringArrayToAprArray( targets, pool );
    svn_client_commit_info_t* commitInfoPtr;

    HandleError( svn_client_commit( &commitInfoPtr, aprArrayTargets, nonRecursive, 
        ctx->ToSvnContext( pool ), pool ) );

    return new CommitInfo( commitInfoPtr );
}

apr_array_header_t* NSvn::Core::Client::StringArrayToAprArray( String* strings[], Pool& pool )
{
    apr_array_header_t* array = apr_array_make( pool, strings->Length, sizeof( char* ) );
    
    // put the strings in the apr array
    for( int i = 0; i < strings->Length; ++i )
    {
        *((char**)apr_array_push(array)) = StringHelper( strings[i] ).CopyToPool( pool );
    }

    return array;
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






