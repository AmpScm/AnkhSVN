// $Id$

#include "stdafx.h"

#include "Client.h"

#include "StringHelper.h"


#include "Status.h"
#include "Revision.h"
#include "CommitInfo.h"
#include "DirectoryEntry.h"
#include "Status.h"
#include "StatusDictionary.h"
#include <svn_client.h>
#include <svn_wc.h>
#include <svn_path.h>
#include <svn_subst.h>
#include <svn_utf.h>
#include <apr_general.h>
#include <apr_hash.h>
#include "SvnClientException.h"
#include "AprFileAdapter.h"
#include "LogMessage.h"
#include "ManagedPointer.h"
#include "stream.h"
#include <svn_io.h>
#include "LogMessageEventArgs.h"
#include "CancelEventArgs.h"
#include "NotificationEventArgs.h"
#include "AuthenticationBaton.h"
#include "ClientContext.h"
#include <windows.h>
#include <stdlib.h>

#include "Status.h"

//TODO: clean up includes in general(not just here)

using namespace System::Collections;

// used to persist the pool and the status callback
struct StatusBaton
{
    NSvn::Core::Pool* pool;
    NSvn::Core::ManagedPointer <NSvn::Core::StatusCallback*>* statusCallback;
};



 /// callback function for Client::Log
svn_error_t* svn_log_message_receiver(void *baton, 
                                      apr_hash_t *changed_paths, svn_revnum_t revision, 
                                      const char *author, const char *date, const char *message, 
                                      apr_pool_t *pool); 

/// callback function for Client::Status
void svn_status_func( void* baton, const char* path, svn_wc_status_t* status );

NSvn::Core::Client::Client()
{
    this->context = new ClientContext( this,
        new AuthenticationBaton(), 
        new ClientConfig() );
} 

NSvn::Core::Client::Client( String* configDir )
{
    this->context = new ClientContext( this,
        new AuthenticationBaton(),
        new ClientConfig( configDir ) );
    
    this->context->AuthBaton->SetParameter( AuthenticationBaton::ParamConfigDir, configDir );
}

NSvn::Core::AuthenticationBaton* NSvn::Core::Client::get_AuthBaton()
{
    return this->context->AuthBaton;
}

// Retrieve the name of the administrative subdirectory.
String* NSvn::Core::Client::get_AdminDirectoryName()
{
    return StringHelper( SVN_WC_ADM_DIR_NAME );
}
#if defined(ALT_ADMIN_DIR)
// Set the name of the administative subdirectory.
// This functionality depends on a specially compiled Subversion.
void NSvn::Core::Client::set_AdminDirectoryName( String* name )
{
    svn_wc_set_adm_dir_name( StringHelper(name) );
}
#endif

// implementation of Client::Add
void NSvn::Core::Client::Add( String* path, bool recursive )
{
    Pool pool;

    const char* truePath = CanonicalizePath( path, pool );
    HandleError( svn_client_add( truePath, recursive, this->context->ToSvnContext( pool ), pool ) );
}
// implementation of Client::MakeDir
NSvn::Core::CommitInfo* NSvn::Core::Client::MakeDir( String* paths[] )
{
    Pool pool;

    apr_array_header_t* aprPaths = StringArrayToAprArray( paths, true, pool );

    svn_client_commit_info_t* commitInfo = 0;
    HandleError( svn_client_mkdir( &commitInfo, aprPaths, this->context->ToSvnContext( pool ), 
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
    HandleError( svn_client_cleanup( truePath, this->context->ToSvnContext( pool ), pool ) );
}

// implementation of Client::Revert
void NSvn::Core::Client::Revert(String* paths[], bool recursive )
{
    Pool pool;
    
    const apr_array_header_t* aprArray = StringArrayToAprArray( paths, true, pool );

    HandleError( svn_client_revert( aprArray, recursive, 
        this->context->ToSvnContext (pool), pool));
}    

// implementation of Client::Resolve
void NSvn::Core::Client::Resolved(String* path, bool recursive )
{
    Pool pool;
    const char* truePath = CanonicalizePath( path, pool );

    HandleError( svn_client_resolved( truePath, recursive, 
        this->context->ToSvnContext (pool), pool));
}  


// implementation of Client::Status
void NSvn::Core::Client::Status( 
    [System::Runtime::InteropServices::Out]System::Int32* youngest, 
    String* path, Revision* revision, StatusCallback* statusCallback, bool descend, bool getAll, bool update,  
    bool noIgnore )
{
    Pool pool;
    StatusBaton baton;

    ManagedPointer<StatusCallback*> statusBaton(statusCallback);

    baton.pool = &pool;
    baton.statusCallback = &statusBaton;

    const char* truePath = CanonicalizePath( path, pool );

    svn_revnum_t revnum;
    HandleError( svn_client_status( &revnum, truePath, 
        revision->ToSvnOptRevision( pool ), svn_status_func, &baton, descend,
        getAll, update, noIgnore, this->context->ToSvnContext( pool ), pool ) );

    *youngest = revnum;
}

// implementation of Client::SingleStatus
NSvn::Core::Status* NSvn::Core::Client::SingleStatus( String* path )
{
    // used to hold the result of the status call
    struct StatusHolder
    {
        static void Callback( void* baton, const char* path, svn_wc_status_t* status )
        {
            StatusHolder* holder = static_cast<StatusHolder*>(baton);
            holder->Status = status;
        }
        svn_wc_status_t* Status;
    } holder;

    Pool pool;
    const char* truePath = CanonicalizePath( path, pool );
    svn_revnum_t youngest;    

    HandleError( svn_client_status( &youngest, truePath, 
        Revision::Unspecified->ToSvnOptRevision( pool ),
        StatusHolder::Callback, &holder, false, true, false, true, 
        this->context->ToSvnContext( pool ), pool ) );

    return new NSvn::Core::Status( holder.Status );
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


// implementation of Client::RevPropSet
void NSvn::Core::Client::RevPropSet(Property* property, String* url, Revision* revision, 
                                    [System::Runtime::InteropServices::Out]System::Int32*
                                    revisionNumber, bool force)
{
    Pool pool;

    const char* truePath = CanonicalizePath( url, pool );
    svn_string_t propv;
    ByteArrayToSvnString( &propv, property->Data, pool );
    svn_revnum_t setRev; 

    HandleError( svn_client_revprop_set(  StringHelper(property->Name), &propv, 
        truePath, revision->ToSvnOptRevision( pool ), &setRev, 
        force, this->context->ToSvnContext (pool), pool));

    *revisionNumber = setRev;
}

// implementation of Client::Checkout
int NSvn::Core::Client::Checkout( String* url, String* path, Revision* revision, 
                                  bool recurse )
{
    Pool pool;
    const char* truePath = CanonicalizePath( path, pool );
    svn_revnum_t rev;
    HandleError( svn_client_checkout( &rev, StringHelper( url ), truePath, 
        revision->ToSvnOptRevision( pool ), recurse, this->context->ToSvnContext( pool ), pool ) );

    return rev;
}

// implementation of Client::Update
int NSvn::Core::Client::Update( String* path, Revision* revision, bool recurse )
{
    Pool pool;
    const char* truePath = CanonicalizePath( path, pool );
    svn_revnum_t rev;
    HandleError( svn_client_update( &rev, truePath, revision->ToSvnOptRevision( pool ),
        recurse, this->context->ToSvnContext( pool ), pool ) );

    return rev;
}

// implementation of Client::Commit
NSvn::Core::CommitInfo* NSvn::Core::Client::Commit( String* targets[], bool nonRecursive )
{
    Pool pool;
    apr_array_header_t* aprArrayTargets = StringArrayToAprArray( targets, true, pool );
    svn_client_commit_info_t* commitInfoPtr = 0;

    HandleError( svn_client_commit( &commitInfoPtr, aprArrayTargets, nonRecursive, 
        this->context->ToSvnContext( pool ), pool ) );
    
    if ( commitInfoPtr && commitInfoPtr->revision != SVN_INVALID_REVNUM )
    {
        return new CommitInfo( commitInfoPtr );
    }
    else 
    {
        return CommitInfo::Invalid;
    }
}
// implementation of Client::Move
NSvn::Core::CommitInfo* NSvn::Core::Client::Move( String* srcPath, 
                                                 Revision* srcRevision, String* dstPath, 
                                                 bool force )
{
    Pool pool;
    const char* trueSrcPath = CanonicalizePath( srcPath, pool );
    const char* trueDstPath = CanonicalizePath( dstPath, pool );

    svn_client_commit_info_t* commitInfoPtr = 0;

    HandleError( svn_client_move ( &commitInfoPtr, trueSrcPath, 
        srcRevision->ToSvnOptRevision( pool ), trueDstPath, force, 
        this->context->ToSvnContext( pool ), pool ) );

    if ( commitInfoPtr != 0 )
        return new CommitInfo( commitInfoPtr );
    else
        return CommitInfo::Invalid;
}
// implementation of Client::Export
int NSvn::Core::Client::Export(String* from, String* to, Revision* revision, bool force)
{
    Pool pool;
    const char* trueSrcPath = CanonicalizePath( from, pool );
    const char* trueDstPath = CanonicalizePath( to, pool );

    svn_revnum_t rev;
    HandleError( svn_client_export ( &rev, trueSrcPath, trueDstPath, 
        revision->ToSvnOptRevision( pool ), force, this->context->ToSvnContext( pool ), pool ) );

    return rev;
}   
//TODO: Implement the variable optionalAdmAccess
// implementation of Client::Copy
NSvn::Core::CommitInfo* NSvn::Core::Client::Copy(String* srcPath, Revision* srcRevision, String* dstPath)
{
    Pool pool;
    const char* trueSrcPath = CanonicalizePath( srcPath, pool );
    const char* trueDstPath = CanonicalizePath( dstPath, pool );

    svn_client_commit_info_t* commitInfoPtr = 0;

    HandleError( svn_client_copy ( &commitInfoPtr, trueSrcPath , 
        srcRevision->ToSvnOptRevision( pool ), trueDstPath, 
        this->context->ToSvnContext( pool ), pool ) );

    if ( commitInfoPtr != 0 )
        return new CommitInfo( commitInfoPtr );
    else
        return CommitInfo::Invalid;
}

// implementation of Client::Merge
void NSvn::Core::Client::Merge(String* url1, Revision* revision1, String* url2, Revision* revision2, 
                               String* targetWcPath, bool recurse, bool ignoreAncestry,
                               bool force, bool dryRun)
{
    Pool pool;
    const char* trueSrcPath1 = CanonicalizePath( url1, pool );
    const char* trueSrcPath2 = CanonicalizePath( url2, pool );
    const char* trueDstPath = CanonicalizePath( targetWcPath, pool );

    HandleError( svn_client_merge ( trueSrcPath1 , revision1->ToSvnOptRevision( pool ),
        trueSrcPath2, revision2->ToSvnOptRevision( pool ),
        trueDstPath, recurse, ignoreAncestry, force, dryRun,
        this->context->ToSvnContext( pool ), pool ) );
}
// implementation of Client::PropGet
NSvn::Common::PropertyDictionary* NSvn::Core::Client::PropGet(String* propName, 
                                                              String* target, Revision* revision, 
                                                              bool recurse)
{
    Pool pool;
    apr_hash_t* propertyHash;

    const char* trueTarget = CanonicalizePath( target, pool );

    svn_error_t* err = svn_client_propget( &propertyHash, StringHelper( propName ), trueTarget,
        revision->ToSvnOptRevision( pool ), recurse, this->context->ToSvnContext( pool ), pool );
    HandleError( err );

    return ConvertToPropertyDictionary( propertyHash, propName, pool );

}
// implementation of Client::RevPropGet
NSvn::Common::Property*  NSvn::Core::Client::RevPropGet(String* propName, String* url, Revision* revision,
                                                        System::Int32* revisionNumber)
{
    Pool pool;

    const char* truePath = CanonicalizePath( url, pool );
    svn_string_t* propv;
    svn_revnum_t setRev; 

    HandleError( svn_client_revprop_get(  StringHelper( propName ), &propv, 
        truePath, revision->ToSvnOptRevision( pool ), &setRev, 
        this->context->ToSvnContext (pool), pool));

    *revisionNumber = setRev;
    Byte array[] = SvnStringToByteArray( propv );

    return new Property( propName, array );

}	

//TODO: Implement the variable admAccessBaton
// implementation of Client::Delete
NSvn::Core::CommitInfo* NSvn::Core::Client::Delete(String* paths[], bool force)
{
    Pool pool;

    svn_client_commit_info_t* commitInfoPtr = 0;

    apr_array_header_t* aprPaths = StringArrayToAprArray( paths, true, pool );

    HandleError( svn_client_delete( &commitInfoPtr, aprPaths, false, 
        this->context->ToSvnContext( pool ), pool ) );

    if ( commitInfoPtr != 0 )
        return new CommitInfo( commitInfoPtr );
    else
        return CommitInfo::Invalid;
}
// implementation of Client::Import
NSvn::Core::CommitInfo* NSvn::Core::Client::Import(String* path, String* url, bool nonRecursive)
{
    Pool pool;
    const char* trueSrcPath = CanonicalizePath( path, pool );
    const char* trueDstUrl = CanonicalizePath( url, pool );

    svn_client_commit_info_t* commitInfoPtr = 0;

    HandleError( svn_client_import( &commitInfoPtr, trueSrcPath, trueDstUrl, nonRecursive,
        this->context->ToSvnContext( pool ), pool ) );

    if ( commitInfoPtr != 0 )
        return new CommitInfo( commitInfoPtr );
    else
        return CommitInfo::Invalid;
}

// implementation of Client::Cat
void NSvn::Core::Client::Cat( Stream* out, String* path, Revision* revision )
{
    Pool pool; 

    const char* truePath = CanonicalizePath( path, pool );
    svn_stream_t* svnStream = CreateSvnStream( out, pool );

    HandleError( svn_client_cat( svnStream, truePath, revision->ToSvnOptRevision( pool ), 
        this->context->ToSvnContext( pool ), pool ) );
}

// implementation of Client::Switch
int NSvn::Core::Client::Switch( String* path, String* url, Revision* revision, bool recurse)
{
    Pool pool;

    const char* truePath = CanonicalizePath( path, pool );
    const char* trueUrl = CanonicalizePath( url, pool );

    svn_revnum_t rev;
    HandleError( svn_client_switch( &rev, truePath, trueUrl, revision->ToSvnOptRevision( pool ), recurse,
        this->context->ToSvnContext( pool ), pool ) );

    return rev;

}

// implementation of Client::PropList
NSvn::Common::PropListItem* NSvn::Core::Client::PropList( String* path, Revision* revision, bool recurse ) []
{
    Pool pool;

    const char* truePath = CanonicalizePath( path, pool );
    apr_array_header_t* propListItems;

    HandleError( svn_client_proplist( &propListItems, truePath, 
        revision->ToSvnOptRevision( pool ), recurse, this->context->ToSvnContext( pool ),
        pool ) );

    return ConvertPropListArray( propListItems, pool );
}

// Implementation of Client::RevPropList
NSvn::Common::PropertyDictionary* NSvn::Core::Client::RevPropList( String* path,
                                                                  Revision* revision, System::Int32* revisionNumber ) 
{
    Pool pool;

    const char* truePath = CanonicalizePath( path, pool );
    apr_hash_t* propListItems;

    svn_revnum_t revNo;
    HandleError( svn_client_revprop_list( &propListItems, truePath, 
        revision->ToSvnOptRevision( pool ), &revNo, 
        this->context->ToSvnContext( pool ), pool ) );

    *revisionNumber = revNo;

    return ConvertToPropertyDictionary( propListItems, 0, pool );
}

// Implementation of Client::List
NSvn::Core::DirectoryEntry* NSvn::Core::Client::List(String* path, Revision* revision, 
                                                     bool recurse) []
{
    Pool pool;

    const char* truePath = CanonicalizePath( path, pool );
    apr_hash_t* entriesHash;

    HandleError( svn_client_ls( &entriesHash, truePath, 
        revision->ToSvnOptRevision( pool ), recurse,
        this->context->ToSvnContext( pool ), pool ) );

    ArrayList* entries = new ArrayList();

    apr_hash_index_t* idx = apr_hash_first( pool, entriesHash );
    while( idx != 0 )
    {
        const char* path;
        apr_ssize_t keyLength;
        svn_dirent_t* dirent;

        apr_hash_this( idx, reinterpret_cast<const void**>(&path), &keyLength,
            reinterpret_cast<void**>(&dirent) );

        entries->Add( new DirectoryEntry( path, dirent ) );

        idx = apr_hash_next( idx );
    }

    return static_cast<DirectoryEntry*[]>( 
        entries->ToArray( __typeof(DirectoryEntry) ) );
}

// implementation of Client::Diff
void NSvn::Core::Client::Diff( String* diffOptions[], String* path1, Revision* revision1,
                              String* path2, Revision* revision2, bool recurse, bool ignoreAncestry, bool noDiffDeleted, 
                              Stream* outfile, Stream* errfile )
{
    Pool pool;

    apr_array_header_t* diffOptArray = StringArrayToAprArray( diffOptions, false, pool );
    const char* truePath1 = CanonicalizePath( path1, pool );
    const char* truePath2 = CanonicalizePath( path2, pool );

    AprFileAdapter* outAdapter = new AprFileAdapter(outfile);
    AprFileAdapter* errAdapter = new AprFileAdapter(errfile);
    apr_file_t* aprOut = outAdapter->Start( pool );
    apr_file_t* aprErr = errAdapter->Start( pool );    

    HandleError( svn_client_diff( diffOptArray, truePath1, 
        revision1->ToSvnOptRevision( pool ), truePath2, 
        revision2->ToSvnOptRevision(pool), recurse, ignoreAncestry, noDiffDeleted,
        aprOut, aprErr, this->context->ToSvnContext(pool), pool ) );

    apr_file_close( aprOut );
    apr_file_close( aprErr );

    outAdapter->WaitForExit();
    errAdapter->WaitForExit();
}

String* NSvn::Core::Client::GetPristinePath(String* path)
{
    Pool pool;

    const char* realPath = CanonicalizePath( path, pool );
    const char* pristinePath = NULL;
    HandleError(svn_wc_get_pristine_copy_path(realPath, &pristinePath, pool));

    if ( pristinePath ) 
        return (String*)StringHelper( pristinePath );
    else
        return 0;
}

// Implementation of Client::Log
void NSvn::Core::Client::Log( String* targets[], Revision* start, Revision* end, bool discoverChangePath, 
                             bool strictNodeHistory, LogMessageReceiver* receiver )
{
    Pool pool;

    apr_array_header_t* aprTargets = StringArrayToAprArray( targets, true, pool );

    ManagedPointer<NSvn::Core::LogMessageReceiver*> ptr(receiver);

    HandleError( svn_client_log( aprTargets, start->ToSvnOptRevision(pool), 
        end->ToSvnOptRevision(pool), discoverChangePath, 
        strictNodeHistory, svn_log_message_receiver, &ptr,
        this->context->ToSvnContext(pool), pool ) ); 
}

// Implementation of Client::Relocate
void NSvn::Core::Client::Relocate( String* dir, String* from, String* to,
                                  bool recurse )
{
    Pool pool;
    HandleError( svn_client_relocate( 
        CanonicalizePath( dir, pool ),
        CanonicalizePath( from, pool ),
        CanonicalizePath( to, pool ),
        recurse,
        this->context->ToSvnContext(pool), pool ) );
}

// implementation of Client::UrlFromPath
String* NSvn::Core::Client::UrlFromPath( String* path )
{
    Pool pool;

    const char* realPath = CanonicalizePath( path, pool );

    const char * url;

    HandleError( svn_client_url_from_path( &url, realPath, pool ) );

    if ( url ) 
        return (String*)StringHelper( url );
    else
        return 0;
}

// implementation of Client::UuidFromUrl
String* NSvn::Core::Client::UuidFromUrl( String* url )
{
    Pool pool;

    const char* realUrl = CanonicalizePath( url, pool );
    const char* uuid;

    HandleError( svn_client_uuid_from_url( &uuid, realUrl, this->context->ToSvnContext( pool ),
        pool ) );

    return (String*)StringHelper( uuid );
}


bool NSvn::Core::Client::HasBinaryProp( String* path )
{
    Pool pool;
    const char* realPath = CanonicalizePath( path, pool );

    // lock the directory
    svn_wc_adm_access_t* admAccess;
    svn_error_t* err = svn_wc_adm_probe_open( &admAccess, 0, CanonicalizePath( path, pool ), 
        false, false, pool );    

    if( err != 0 && err->apr_err == SVN_ERR_WC_NOT_DIRECTORY )
        return false;
    else
        HandleError( err );
    try
    {
        svn_boolean_t hasBinaryProp = 0;
        HandleError(svn_wc_has_binary_prop( &hasBinaryProp, realPath, admAccess, pool ));
        return hasBinaryProp != 0;
    }
    __finally
    {
        svn_wc_adm_close( admAccess );
    }
}

void NSvn::Core::Client::OnNotification( NotificationEventArgs* args )
{
    if ( this->Notification != 0 )
        this->Notification->Invoke( this, args );
}

void NSvn::Core::Client::OnCancel( CancelEventArgs* args )
{
    if ( this->Cancel != 0 )
        this->Cancel->Invoke( this, args );
}

void NSvn::Core::Client::OnLogMessage( LogMessageEventArgs* args )
{
    if ( this->LogMessage != 0 )
        this->LogMessage->Invoke( this, args );
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

/// marshall the callback into managed space
svn_error_t* svn_log_message_receiver(void *baton, 
                                                  apr_hash_t *changed_paths, svn_revnum_t revision, 
                                                  const char *author, const char *date, const char *message, 
                                                  apr_pool_t *pool)
{
    using namespace NSvn::Core;
    LogMessageReceiver* receiver = *(static_cast<ManagedPointer<LogMessageReceiver*>* >
        (baton) );

    // convert message to native EOL style
    const char* nativeMessage;
    HandleError(svn_subst_translate_cstring (message, &nativeMessage,
        APR_EOL_STR, /* the 'native' eol */
        FALSE,       /* no need to repair */
        NULL,        /* no keywords */
        FALSE,       /* no expansion */
        pool));

    LogMessage* logMessage = new LogMessage( changed_paths, revision,
        author, date, nativeMessage, pool );

    receiver->Invoke( logMessage );

    return SVN_NO_ERROR;
}

/// marshall the status callback into managed space
void svn_status_func(void* baton, const char* path, svn_wc_status_t* status)
{
    using namespace NSvn::Core;

    StatusBaton* statusBaton = static_cast<StatusBaton*>(baton);
    StatusCallback* statusCallback = *(static_cast<ManagedPointer<StatusCallback*>*>(statusBaton->statusCallback));

    String* nativePath = ToNativePath( path, *(statusBaton->pool) );


    statusCallback->Invoke( nativePath, new Status( status ) );
}

