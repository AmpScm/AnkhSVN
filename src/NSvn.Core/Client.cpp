// $Id$
#include "Client.h"
#include "StringHelper.h"
#include "Notification.h"

#include <svn_client.h>
#include <svn_path.h>
#include <apr_general.h>
#include "SvnClientException.h"


// implementation of Client::Add
void NSvn::Core::Client::Add( String* path, bool recursive, ClientContext* context )
{
    Pool pool;

    const char* truePath = CanonicalizePath( path, pool );
    HandleError( svn_client_add( truePath, recursive, context->ToSvnContext( pool ), pool ) );
}
// implementation of Client::MakeDir
NSvn::Core::CommitInfo* NSvn::Core::Client::MakeDir( String* path, ClientContext* context )
{
    Pool pool;

    const char* truePath = CanonicalizePath( path, pool );
    svn_client_commit_info_t* commitInfo = 0;
    HandleError( svn_client_mkdir( &commitInfo, truePath, context->ToSvnContext( pool ), 
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
    const char* truePath = CanonicalizePath( directory, pool );
    HandleError( svn_client_cleanup( truePath, pool ) );
}

// implementation of Client::Revert
void NSvn::Core::Client::Revert(String* path, bool recursive, ClientContext* context )
{
    Pool pool;
    const char* truePath = CanonicalizePath( path, pool );

    HandleError( svn_client_revert( truePath, recursive, 
        context->ToSvnContext (pool), pool));
}    

// implementation of Client::Resolve
void NSvn::Core::Client::Resolve(String* path, bool recursive, ClientContext* context )
{
    Pool pool;
    const char* truePath = CanonicalizePath( path, pool );

    HandleError( svn_client_resolve( truePath, recursive, 
        context->ToSvnContext (pool), pool));
}  


// implementation of Client::Status
//TODO: StringDictionary to be reconsidered
StringDictionary* NSvn::Core::Client::Status( long* youngest, String* path, bool descend,
                                bool getAll, bool update, bool noIgnore, ClientContext* context )
{
    apr_hash_t* statushash = 0;
    Pool pool;
    const char* truePath = CanonicalizePath( path, pool );
    
    HandleError( svn_client_status( &statushash, youngest, StringHelper( truePath ), descend,
        getAll, update, noIgnore, context->ToSvnContext( pool ), pool ) );

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
    const char* truePath = CanonicalizePath( target, pool );
    HandleError( svn_client_propset( StringHelper(propName), &propv, truePath, recurse, pool) );
}

// implementation of Client::Checkout
void NSvn::Core::Client::Checkout( String* url, String* path, Revision* revision, 
                                  bool recurse, ClientContext* context )
{
    Pool pool;
    const char* truePath = CanonicalizePath( path, pool );
    HandleError( svn_client_checkout( StringHelper( url ), truePath, 
        revision->ToSvnOptRevision( pool ), recurse, context->ToSvnContext( pool ), pool ) );
}

// implementation of Client::Update
void NSvn::Core::Client::Update( String* path, Revision* revision, bool recurse, 
                                ClientContext* context )
{
    Pool pool;
    const char* truePath = CanonicalizePath( path, pool );
    HandleError( svn_client_update( truePath, revision->ToSvnOptRevision( pool ),
        recurse, context->ToSvnContext( pool ), pool ) );
}

// implementation of Client::Commit
NSvn::Core::CommitInfo* NSvn::Core::Client::Commit( String* targets[], bool nonRecursive, ClientContext* context )
{
    Pool pool;
    apr_array_header_t* aprArrayTargets = StringArrayToAprArray( targets, pool );
    svn_client_commit_info_t* commitInfoPtr;

    HandleError( svn_client_commit( &commitInfoPtr, aprArrayTargets, nonRecursive, 
        context->ToSvnContext( pool ), pool ) );

    return new CommitInfo( commitInfoPtr );
}
// implementation of Client::Move
NSvn::Core::CommitInfo* NSvn::Core::Client::Move( String* srcPath, 
                                                         Revision* srcRevision, String* dstPath, 
                                                         bool force, ClientContext* context )
{
    Pool pool;
    const char* trueSrcPath = CanonicalizePath( srcPath, pool );
    const char* trueDstPath = CanonicalizePath( dstPath, pool );

    svn_client_commit_info_t* commitInfoPtr = 0;

    HandleError( svn_client_move ( &commitInfoPtr, trueSrcPath, 
        srcRevision->ToSvnOptRevision( pool ), trueDstPath, force, 
        context->ToSvnContext( pool ), pool ) );

     if ( commitInfoPtr != 0 )
        return new CommitInfo( commitInfoPtr );
    else
        return CommitInfo::Invalid;
}
// implementation of Client::Export
void NSvn::Core::Client::Export(String* from, String* to, Revision* revision, ClientContext* context)
{
    Pool pool;
    const char* trueSrcPath = CanonicalizePath( from, pool );
    const char* trueDstPath = CanonicalizePath( to, pool );
    
    HandleError( svn_client_export ( trueSrcPath, trueDstPath, 
        revision->ToSvnOptRevision( pool ), context->ToSvnContext( pool ), pool ) );
}

// Converts array of .NET strings to apr array of const char
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

// Canonicalizes a path to the correct format expected by SVN
const char* NSvn::Core::Client::CanonicalizePath( String* path, Pool& pool )
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
void NSvn::Core::Client::ByteArrayToSvnString( svn_string_t* string, Byte array[], const Pool& pool  )
{
    string->len = array.Length;
    string->data = static_cast<char*>(pool.Alloc( array.Length));
    Marshal::Copy( array, 0, const_cast<char*>(string->data), array.Length );

}