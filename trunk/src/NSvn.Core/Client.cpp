// $Id$
#include "Client.h"
#include "StringHelper.h"
#include "Notification.h"


#include <svn_client.h>
#include <svn_path.h>
#include <svn_utf.h>
#include <apr_general.h>
#include <apr_hash.h>
#include "SvnClientException.h"
#include "stream.h"
#include <svn_io.h>

using namespace System::Collections;


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
NSvn::Core::StatusDictionary* NSvn::Core::Client::Status( 
                [System::Runtime::InteropServices::Out]System::Int32* youngest, 
                String* path, bool descend, bool getAll, bool update,  
                bool noIgnore, ClientContext* context )
{
    apr_hash_t* statushash = 0;
    Pool pool;
    const char* truePath = CanonicalizePath( path, pool );
    
    svn_revnum_t revnum;
    HandleError( svn_client_status( &statushash, &revnum, truePath, descend,
        getAll, update, noIgnore, context->ToSvnContext( pool ), pool ) );

    *youngest = revnum;

    // convert to a StatusDictionary
    return StatusDictionary::FromStatusHash( statushash, pool );
}
// implementation of Client::PropSet
void NSvn::Core::Client::PropSet(Property* property, String* target, bool recurse)
{
    Pool pool;
    svn_string_t propv;
    ByteArrayToSvnString( &propv, property->Data, pool );    
    const char* truePath = CanonicalizePath( target, pool );
    HandleError( svn_client_propset( StringHelper(property->Name), &propv, truePath, recurse, pool) );
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
//TODO: Implement the variable optionalAdmAccess
// implementation of Client::Copy
NSvn::Core::CommitInfo* NSvn::Core::Client::Copy(String* srcPath, Revision* srcRevision, String* dstPath,
                ClientContext* context)
{
    Pool pool;
    const char* trueSrcPath = CanonicalizePath( srcPath, pool );
    const char* trueDstPath = CanonicalizePath( dstPath, pool );

    svn_client_commit_info_t* commitInfoPtr = 0;

    HandleError( svn_client_copy ( &commitInfoPtr, trueSrcPath , 
        srcRevision->ToSvnOptRevision( pool ), trueDstPath, 0,  
        context->ToSvnContext( pool ), pool ) );

     if ( commitInfoPtr != 0 )
        return new CommitInfo( commitInfoPtr );
    else
        return CommitInfo::Invalid;
}

// implementation of Client::Merge
void NSvn::Core::Client::Merge(String* url1, Revision* revision1, String* url2, Revision* revision2, 
                String* targetWcPath, bool recurse, bool force, bool dryRun, ClientContext* context)
{
    Pool pool;
    const char* trueSrcPath1 = CanonicalizePath( url1, pool );
    const char* trueSrcPath2 = CanonicalizePath( url2, pool );
    const char* trueDstPath = CanonicalizePath( targetWcPath, pool );

    HandleError( svn_client_merge ( trueSrcPath1 , revision1->ToSvnOptRevision( pool ),
        trueSrcPath2, revision2->ToSvnOptRevision( pool ),
        trueDstPath, recurse, force, dryRun,
        context->ToSvnContext( pool ), pool ) );
}

NSvn::Common::PropertyDictionary* NSvn::Core::Client::PropGet(String* propName, 
                                                         String* target, Revision* revision, 
                                                        bool recurse, ClientContext* context)
{
    Pool pool;
    apr_hash_t* propertyHash;

    const char* trueTarget = CanonicalizePath( target, pool );

    svn_error_t* err = svn_client_propget( &propertyHash, StringHelper( propName ), trueTarget,
        revision->ToSvnOptRevision( pool ), recurse, context->ToSvnContext( pool ), pool );
    HandleError( err );

    return ConvertToPropertyDictionary( propertyHash, propName, pool );

}
//TODO: Implement the variable admAccessBaton
// implementation of Client::Delete
NSvn::Core::CommitInfo* NSvn::Core::Client::Delete(String* path, bool force, 
                ClientContext* context)
{
	Pool pool;
    const char* trueSrcPath = CanonicalizePath( path, pool );
    
    svn_client_commit_info_t* commitInfoPtr = 0;

    HandleError( svn_client_delete( &commitInfoPtr, trueSrcPath,  0, false, 
        context->ToSvnContext( pool ), pool ) );

     if ( commitInfoPtr != 0 )
        return new CommitInfo( commitInfoPtr );
    else
        return CommitInfo::Invalid;
}

NSvn::Core::CommitInfo* NSvn::Core::Client::Import(String* path, String* url, String* newEntry, bool nonRecursive, 
                ClientContext* context)
{
	Pool pool;
    const char* trueSrcPath = CanonicalizePath( path, pool );
    const char* trueDstUrl = CanonicalizePath( url, pool );
	const char* trueNewEntry = CanonicalizePath( newEntry, pool );

    svn_client_commit_info_t* commitInfoPtr = 0;

    HandleError( svn_client_import( &commitInfoPtr, trueSrcPath, trueDstUrl, trueNewEntry, nonRecursive,
        context->ToSvnContext( pool ), pool ) );

     if ( commitInfoPtr != 0 )
        return new CommitInfo( commitInfoPtr );
    else
        return CommitInfo::Invalid;
}

// implementation of Client::Cat
void NSvn::Core::Client::Cat( Stream* out, String* path, Revision* revision, ClientContext* context )
{
    Pool pool; 

    const char* truePath = CanonicalizePath( path, pool );
    svn_stream_t* svnStream = CreateSvnStream( out, pool );

    HandleError( svn_client_cat( svnStream, truePath, revision->ToSvnOptRevision( pool ), 
        context->ToSvnContext( pool ), pool ) );
}

// implementation of Client::Switch
void NSvn::Core::Client::Switch( String* path, String* url, Revision* revision, bool recurse, 
                                ClientContext* context)
{
    Pool pool;

    const char* truePath = CanonicalizePath( path, pool );
    const char* trueUrl = CanonicalizePath( url, pool );

    HandleError( svn_client_switch( truePath, trueUrl, revision->ToSvnOptRevision( pool ), recurse,
        context->ToSvnContext( pool ), pool ) );

}

// implementation of Client::PropList
NSvn::Common::PropListItem* NSvn::Core::Client::PropList( String* path, Revision* revision, bool recurse, 
                                  ClientContext* context ) []
{
    Pool pool;

    const char* truePath = CanonicalizePath( path, pool );
    apr_array_header_t* propListItems;

    HandleError( svn_client_proplist( &propListItems, truePath, 
        revision->ToSvnOptRevision( pool ), recurse, context->ToSvnContext( pool ),
        pool ) );

    return ConvertPropListArray( propListItems, pool );
}

// Implementation of Client::RevPropList
NSvn::Common::PropertyDictionary* NSvn::Core::Client::RevPropList( String* path,
    Revision* revision, System::Int32* revisionNumber, ClientContext* context ) 
{
    Pool pool;
    
    const char* truePath = CanonicalizePath( path, pool );
    apr_hash_t* propListItems;

    svn_revnum_t revNo;
    HandleError( svn_client_revprop_list( &propListItems, truePath, 
        revision->ToSvnOptRevision( pool ), &revNo, 
        context->ToSvnContext( pool ), pool ) );

    *revisionNumber = revNo;

    return ConvertToPropertyDictionary( propListItems, 0, pool );
}

// Implementation of Client::List
NSvn::Core::DirectoryEntry* NSvn::Core::Client::List(String* path, Revision* revision, 
    bool recurse, ClientContext* context) []
{
    Pool pool;

    const char* truePath = CanonicalizePath( path, pool );
    apr_hash_t* entriesHash;

    HandleError( svn_client_ls( &entriesHash, truePath, 
        revision->ToSvnOptRevision( pool ), recurse,
        context->ToSvnContext( pool ), pool ) );

    ArrayList* entries = new ArrayList();

    apr_hash_index_t* idx = apr_hash_first( pool, entriesHash );
    while( idx != 0 )
    {
        const char* path;
        apr_ssize_t keyLength;
        svn_dirent* dirent;

        apr_hash_this( idx, reinterpret_cast<const void**>(&path), &keyLength,
            reinterpret_cast<void**>(&dirent) );

        entries->Add( new DirectoryEntry( path, dirent ) );

        idx = apr_hash_next( idx );
    }

    return static_cast<DirectoryEntry*[]>( 
        entries->ToArray( __typeof(DirectoryEntry) ) );
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

struct apr_hash_index_t
{};

// converts a apr_hash_t of const char* -> svn_string_t mappings
// if propertyName is not null, use propertyName as the name of the property
// else use the key as the name of the property
NSvn::Common::PropertyDictionary* NSvn::Core::Client::ConvertToPropertyDictionary( 
    apr_hash_t* propertyHash, String* propertyName, Pool& pool )
{
    PropertyDictionary* mapping = new PropertyDictionary();

    // iterate over the items in the hash
    apr_hash_index_t* idx = apr_hash_first( pool, propertyHash );
    while( idx != 0 )
    {
        const char* key;
        apr_ssize_t keyLength;
        svn_string_t* propVal;

        apr_hash_this( idx, reinterpret_cast<const void**>(&key), &keyLength,
            reinterpret_cast<void**>(&propVal) );

        // copy the bytes into managed space
        Byte bytes[] = new Byte[ propVal->len ];
        Marshal::Copy( const_cast<char*>(propVal->data), bytes, 0, propVal->len );

        // Add it to the mapping collection as a Property object        
        if ( propertyName != 0 )
        {
            Property* property = new Property( propertyName, bytes );
            mapping->Add( ToNativePath( key, pool ), property );
        }
        else 
        {
            String* name = StringHelper( key );
            Property* property = new Property( name, bytes );
            mapping->Add( name, property );
        }

        idx = apr_hash_next( idx );
    }

    return mapping;
}

// converts an array of proplist_item's to an array of PropListItem objects
NSvn::Common::PropListItem* NSvn::Core::Client::ConvertPropListArray( 
    apr_array_header_t* propListItems, Pool& pool ) []
{
    ArrayList* propList = new ArrayList();

    for( int i = 0; i < propListItems->nelts; i++ )
    {
        svn_client_proplist_item_t* item = 
            ((svn_client_proplist_item_t**)propListItems->elts)[i];

        PropertyDictionary* dict = ConvertToPropertyDictionary( 
            item->prop_hash, 0, pool );


        // TODO: is node_name->data always nullterminated?
        String* nodeName = StringHelper( item->node_name->data );
        propList->Add( new PropListItem( nodeName, dict ) );
    }

    return static_cast<PropListItem*[]>(
        propList->ToArray( __typeof(NSvn::Common::PropListItem) ) );
}

String* NSvn::Core::Client::ToNativePath( const char* path, Pool& pool )
{
    // convert to a native path    
    const char* cstringPath;
    HandleError( svn_utf_cstring_from_utf8( &cstringPath, path, pool ) );
    const char* nativePath = svn_path_local_style( cstringPath, pool );
    return StringHelper( nativePath );
}