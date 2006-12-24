// $Id$

#include "stdafx.h"


#include "Client.h"



#include "Revision.h"
#include "CommitInfo.h"
#include "DirectoryEntry.h"
#include "Status.h"
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
#include "ProgressEventArgs.h"
#include "AuthenticationBaton.h"
#include "ClientContext.h"
#include <windows.h>
#include <stdlib.h>

#ifndef POST_DOTNET11
#include <_vcclrit.h>
#endif
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
void svn_status_func2( void* baton, const char* path, svn_wc_status2_t* status );

svn_error_t* svn_blame_func( void *baton, apr_int64_t line_no, svn_revnum_t revision, 
                            const char *author, const char *date, 
                            const char *line, apr_pool_t *pool );


// Initialize Native C++ runtime outside the dll loader lock
void NSvn::Core::Client::InitializeCrt()
{
    System::Threading::Monitor::Enter(_lock);
    __try
    {
        if(!_initialized)
        {
#ifndef POST_DOTNET11
            __crt_dll_initialize();
#endif

            utf8InitializePool = new GCPool();
            svn_utf_initialize(utf8InitializePool->ToAprPool());
            _initialized = true;
        }
    }
    __finally
    {
        System::Threading::Monitor::Exit(_lock);
    }
}

NSvn::Core::Client::Client()
{
    InitializeCrt();
    this->rootPool = new Pool();
    this->context = new ClientContext( this,
        new AuthenticationBaton(), 
        new ClientConfig() );
} 

NSvn::Core::Client::Client( String* configDir )
{
    InitializeCrt();
    this->rootPool = new Pool();
    this->context = new ClientContext( this,
        new AuthenticationBaton(),
        new ClientConfig( configDir ) );
    
    this->context->AuthBaton->SetParameter( AuthenticationBaton::ParamConfigDir, configDir );
}

NSvn::Core::Client::~Client()
{
    this->Dispose( false );
}

void NSvn::Core::Client::Dispose()
{
    this->Dispose( true );
}

void NSvn::Core::Client::Dispose( bool disposing )
{
    delete this->rootPool;

    if ( disposing )
        GC::SuppressFinalize( this );

}

NSvn::Core::AuthenticationBaton* NSvn::Core::Client::get_AuthBaton()
{
    return this->context->AuthBaton;
}

// Retrieve the name of the administrative subdirectory.
String* NSvn::Core::Client::get_AdminDirectoryName()
{
    Pool pool;
    return Utf8ToString( svn_wc_get_adm_dir(pool), pool );
}

// Set the name of the administative subdirectory.
void NSvn::Core::Client::set_AdminDirectoryName( String* name )
{
    Pool pool;
    svn_wc_set_adm_dir( StringToUtf8( name, pool ), pool );
}


// implementation of Client::Add
void NSvn::Core::Client::Add( String* path, Recurse recurse )
{
    this->Add( path, recurse, false );
}

// implementation of Client::Add
void NSvn::Core::Client::Add( String* path, Recurse recurse, bool force )
{
    SubPool pool(*(this->rootPool));

    const char* truePath = CanonicalizePath( path, pool );
    HandleError( svn_client_add2( truePath, recurse == Recurse::Full, force, this->context->ToSvnContext(), pool ) );
}

// implementation of Client::MakeDir
NSvn::Core::CommitInfo* NSvn::Core::Client::MakeDir( String* paths[] )
{
    SubPool pool(*(this->rootPool));;

    apr_array_header_t* aprPaths = StringArrayToAprArray( paths, true, pool );

    svn_client_commit_info_t* commitInfo = 0;
    HandleError( svn_client_mkdir( &commitInfo, aprPaths, this->context->ToSvnContext(), 
        pool ) );

    if ( commitInfo != 0 )
        return new CommitInfo( commitInfo, pool );
    else
        return CommitInfo::Invalid;
}
// implemenentation of Client::Cleanup
void NSvn::Core::Client::Cleanup( String* directory )
{
    SubPool pool(*(this->rootPool));;
    const char* truePath = CanonicalizePath( directory, pool );
    HandleError( svn_client_cleanup( truePath, this->context->ToSvnContext(), pool ) );
}

// implementation of Client::Blame
void NSvn::Core::Client::Blame( String* pathOrUrl, Revision* start, Revision* end, 
                BlameReceiver* receiver )
{
    SubPool pool(*(this->rootPool));;
    ManagedPointer<BlameReceiver*> receiverBaton(receiver);
    const char* truePath = CanonicalizePath( pathOrUrl, pool );

    HandleError( svn_client_blame( truePath, start->ToSvnOptRevision( pool ), 
        end->ToSvnOptRevision( pool ), svn_blame_func, &receiverBaton, 
        this->context->ToSvnContext(), pool) );
}

// implementation of Client::Revert
void NSvn::Core::Client::Revert(String* paths[], Recurse recurse )
{
    SubPool pool(*(this->rootPool));;

    
    const apr_array_header_t* aprArray = StringArrayToAprArray( paths, true, pool );

    HandleError( svn_client_revert( aprArray, recurse == Recurse::Full, 
        this->context->ToSvnContext(), pool));
}    

// implementation of Client::Resolve
void NSvn::Core::Client::Resolved(String* path, Recurse recurse )
{
    SubPool pool(*(this->rootPool));;
    const char* truePath = CanonicalizePath( path, pool );

    HandleError( svn_client_resolved( truePath, recurse == Recurse::Full, 
        this->context->ToSvnContext(), pool));
}  




void NSvn::Core::Client::Status(
                [System::Runtime::InteropServices::Out]System::Int32* youngest, 
                String* path, Revision* revision, 
                StatusCallback* statusCallback, bool descend, 
                bool getAll,
                bool update,  bool noIgnore, bool ignoreExternals )
{
    SubPool pool(*(this->rootPool));;
    StatusBaton baton;

    ManagedPointer<StatusCallback*> statusBaton(statusCallback);

    baton.pool = &pool;
    baton.statusCallback = &statusBaton;

    const char* truePath = CanonicalizePath( path, pool );

    svn_revnum_t revnum;
    HandleError( svn_client_status2( &revnum, truePath, 
        revision->ToSvnOptRevision( pool ), svn_status_func2, &baton, descend,
        getAll, update, noIgnore, ignoreExternals, this->context->ToSvnContext(), pool ) );

    *youngest = revnum;
}

// implementation of Client::Status
void NSvn::Core::Client::Status( 
    [System::Runtime::InteropServices::Out]System::Int32* youngest, 
    String* path, Revision* revision, StatusCallback* statusCallback, bool descend, bool getAll, bool update,  
    bool noIgnore )
{
    this->Status( youngest, path, revision, statusCallback, descend, getAll, update, noIgnore, false );
}

NSvn::Core::Status* NSvn::Core::Client::SingleStatus( String* path )
{
    svn_wc_adm_access_t* admAccess;
    SubPool pool(*(this->rootPool));;

    // lock the directory
    svn_error_t* err = svn_wc_adm_probe_open( &admAccess, 0, CanonicalizePath( path, pool ), 
        false, false, pool );

    if( err && err->apr_err == SVN_ERR_WC_NOT_DIRECTORY )
        return Status::None;
    else
        HandleError( err );

    try
    {
        //retrieve the status
        svn_wc_status2_t* status;    
        const char* truePath = CanonicalizePath( path, pool );

        HandleError( svn_wc_status2( &status, truePath, admAccess, pool ) );

        return new NSvn::Core::Status( status, pool );
    }
    __finally
    {

        // and unlock again
        HandleError( svn_wc_adm_close( admAccess ) );
    }    
}

// implementation of Client::Lock
void NSvn::Core::Client::Lock(String __gc* targets[], String __gc* comment, bool stealLock)
{
    SubPool pool(*(this->rootPool));
    apr_array_header_t* aprArrayTargets = StringArrayToAprArray( targets, true, pool );

    HandleError( svn_client_lock( aprArrayTargets, StringToUtf8( comment, pool ), stealLock,
        this->context->ToSvnContext(), pool ) );
}

// implemtation of Client::Unlock
void NSvn::Core::Client::Unlock( String __gc* targets[], bool breakLock )
{
    SubPool pool(*(this->rootPool));
    apr_array_header_t* aprArrayTargets = StringArrayToAprArray( targets, true, pool );

    HandleError( svn_client_unlock( aprArrayTargets, breakLock, 
        this->context->ToSvnContext(), pool ) );
}

// implementation of Client::PropSet
void NSvn::Core::Client::PropSet(Property* property, String* target, Recurse recurse)
{
    SubPool pool(*(this->rootPool));;
    svn_string_t propv;
    ByteArrayToSvnString( &propv, property->Data, pool );    
    const char* truePath = CanonicalizePath( target, pool );
    HandleError( svn_client_propset( StringToUtf8( property->Name, pool ), &propv, 
		truePath, recurse == Recurse::Full, pool) );
}


// implementation of Client::RevPropSet
void NSvn::Core::Client::RevPropSet(Property* property, String* url, Revision* revision, 
                                    [System::Runtime::InteropServices::Out]System::Int32*
                                    revisionNumber, bool force)
{
    SubPool pool(*(this->rootPool));;

    const char* truePath = CanonicalizePath( url, pool );
    svn_string_t propv;
    ByteArrayToSvnString( &propv, property->Data, pool );
    svn_revnum_t setRev; 

    HandleError( svn_client_revprop_set(  StringToUtf8( property->Name, pool ), &propv, 
        truePath, revision->ToSvnOptRevision( pool ), &setRev, 
        force, this->context->ToSvnContext(), pool));

    *revisionNumber = setRev;
}

// implementation of Client::Checkout
int NSvn::Core::Client::Checkout( String* url, String* path, Revision* revision, 
                                  Recurse recurse )
{
    return this->Checkout( url, path, Revision::Unspecified, revision, recurse, false );
}

int NSvn::Core::Client::Checkout( String* url, String* path, Revision* pegRevision,
                                  Revision* revision, Recurse recurse, bool ignoreExternals)
{
    SubPool pool(*(this ->rootPool));
    const char* truePath = CanonicalizePath( path, pool );
    const char* trueUrl = CanonicalizePath( url, pool );
    svn_revnum_t rev;
    HandleError( svn_client_checkout2( &rev, trueUrl, truePath,
        pegRevision->ToSvnOptRevision( pool ), revision->ToSvnOptRevision( pool ),
        recurse == Recurse::Full, ignoreExternals, this->context->ToSvnContext( ), pool ) );

    return rev;
}


// implementation of Client::Update
int NSvn::Core::Client::Update( String* path, Revision* revision, Recurse recurse )
{
    String* paths[] = new String*[1];
    paths[0] = path;
    Int32 revnums[] = this->Update( paths, revision, recurse, false );
    if ( revnums != 0 && revnums->Length > 0 )
        return revnums[0];
    else
        return SVN_INVALID_REVNUM;
}

Int32 NSvn::Core::Client::Update(String* paths[], Revision* revision, 
                               Recurse recurse, bool ignoreExternals ) __gc []
{
    SubPool pool(*(this->rootPool));;
    apr_array_header_t* truePaths = StringArrayToAprArray( paths, true, pool );
    apr_array_header_t* revnums;
    HandleError( svn_client_update2( &revnums, truePaths, 
        revision->ToSvnOptRevision(pool), recurse == Recurse::Full, ignoreExternals, 
        this->context->ToSvnContext(), pool ) );

    if ( revnums != 0 )
        return AprArrayToIntArray( revnums );
    else
        return 0;

}

// implementation of Client::Commit
NSvn::Core::CommitInfo* NSvn::Core::Client::Commit( String* targets[], Recurse recurse)
{
    return this->Commit( targets, recurse, true );
}

NSvn::Core::CommitInfo* NSvn::Core::Client::Commit(String* targets[], Recurse recurse, 
                                                   bool keepLocks )
{
    SubPool pool(*(this->rootPool));;
    apr_array_header_t* aprArrayTargets = StringArrayToAprArray( targets, true, pool );
    svn_client_commit_info_t* commitInfoPtr = 0;

    HandleError( svn_client_commit2( &commitInfoPtr, aprArrayTargets, (recurse == Recurse::Full), 
        keepLocks, this->context->ToSvnContext(), pool ) );
    
    if ( commitInfoPtr && commitInfoPtr->revision != SVN_INVALID_REVNUM )
    {
        return new CommitInfo( commitInfoPtr, pool );
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
    return this->Move( srcPath, dstPath, force );
}

NSvn::Core::CommitInfo* NSvn::Core::Client::Move( String* srcPath, String* dstPath, 
                                                 bool force )
{
    SubPool pool(*(this->rootPool));;
    const char* trueSrcPath = CanonicalizePath( srcPath, pool );
    const char* trueDstPath = CanonicalizePath( dstPath, pool );

    svn_client_commit_info_t* commitInfoPtr = 0;

    HandleError( svn_client_move2 ( &commitInfoPtr, trueSrcPath, 
        trueDstPath, force, 
        this->context->ToSvnContext(), pool ) );

    if ( commitInfoPtr != 0 )
        return new CommitInfo( commitInfoPtr, pool );
    else
        return CommitInfo::Invalid;
}
// implementation of Client::Export
int NSvn::Core::Client::Export(String* from, String* to, Revision* revision, bool overwrite)
{
    return this->Export( from, to, Revision::Unspecified, revision, overwrite, false, Recurse::Full,
        0 );
}

int NSvn::Core::Client::Export(String* from, String* to, Revision* pegRevision, 
                Revision* revision, bool overwrite, bool ignoreExternals, Recurse recurse,
                String* nativeEol )
{

    SubPool pool(*(this->rootPool));;
    const char* trueSrcPath = CanonicalizePath( from, pool );
    const char* trueDstPath = CanonicalizePath( to, pool );

    svn_revnum_t rev;
    HandleError( svn_client_export3 ( &rev, trueSrcPath, trueDstPath,
        pegRevision->ToSvnOptRevision( pool ),
        revision->ToSvnOptRevision( pool ), overwrite,
        ignoreExternals, recurse == Recurse::Full, StringToUtf8( nativeEol, pool ),
        this->context->ToSvnContext(), pool ) );

    return rev;
}   

//TODO: Implement the variable optionalAdmAccess
// implementation of Client::Copy
NSvn::Core::CommitInfo* NSvn::Core::Client::Copy(String* srcPath, Revision* srcRevision, String* dstPath)
{
    SubPool pool(*(this->rootPool));;
    const char* trueSrcPath = CanonicalizePath( srcPath, pool );
    const char* trueDstPath = CanonicalizePath( dstPath, pool );

    svn_client_commit_info_t* commitInfoPtr = 0;

    HandleError( svn_client_copy ( &commitInfoPtr, trueSrcPath , 
        srcRevision->ToSvnOptRevision( pool ), trueDstPath, 
        this->context->ToSvnContext(), pool ) );

    if ( commitInfoPtr != 0 )
        return new CommitInfo( commitInfoPtr, pool );
    else
        return CommitInfo::Invalid;
}

// implementation of Client::Merge
void NSvn::Core::Client::Merge(String* url1, Revision* revision1, String* url2, Revision* revision2, 
                               String* targetWcPath, Recurse recurse, bool ignoreAncestry,
                               bool force, bool dryRun)
{
    SubPool pool(*(this->rootPool));;
    const char* trueSrcPath1 = CanonicalizePath( url1, pool );
    const char* trueSrcPath2 = CanonicalizePath( url2, pool );
    const char* trueDstPath = CanonicalizePath( targetWcPath, pool );

    HandleError( svn_client_merge ( trueSrcPath1 , revision1->ToSvnOptRevision( pool ),
        trueSrcPath2, revision2->ToSvnOptRevision( pool ),
        trueDstPath, recurse == Recurse::Full, ignoreAncestry, force, dryRun,
        this->context->ToSvnContext(), pool ) );
}
// implementation of Client::PropGet
NSvn::Common::PropertyDictionary* NSvn::Core::Client::PropGet(String* propName, 
                                                              String* target, Revision* revision, 
                                                              Recurse recurse)
{
    SubPool pool(*(this->rootPool));;
    apr_hash_t* propertyHash;

    const char* trueTarget = CanonicalizePath( target, pool );

    svn_error_t* err = svn_client_propget( &propertyHash, StringToUtf8(  propName, pool ) , trueTarget,
        revision->ToSvnOptRevision( pool ), recurse == Recurse::Full, this->context->ToSvnContext(), pool );
    HandleError( err );

    return ConvertToPropertyDictionary( propertyHash, propName, pool );

}
// implementation of Client::RevPropGet
NSvn::Common::Property*  NSvn::Core::Client::RevPropGet(String* propName, String* url, Revision* revision,
                                                        System::Int32* revisionNumber)
{
    SubPool pool(*(this->rootPool));;

    const char* truePath = CanonicalizePath( url, pool );
    svn_string_t* propv;
    svn_revnum_t setRev; 

    HandleError( svn_client_revprop_get(  StringToUtf8(  propName, pool ), &propv, 
        truePath, revision->ToSvnOptRevision( pool ), &setRev, 
        this->context->ToSvnContext(), pool));

    *revisionNumber = setRev;
    Byte array[] = SvnStringToByteArray( propv );

    return new Property( propName, array );

}	

//TODO: Implement the variable admAccessBaton
// implementation of Client::Delete
NSvn::Core::CommitInfo* NSvn::Core::Client::Delete(String* paths[], bool force)
{
    SubPool pool(*(this->rootPool));;

    svn_client_commit_info_t* commitInfoPtr = 0;

    apr_array_header_t* aprPaths = StringArrayToAprArray( paths, true, pool );

    HandleError( svn_client_delete( &commitInfoPtr, aprPaths, static_cast<svn_boolean_t>(force), 
        this->context->ToSvnContext(), pool ) );

    if ( commitInfoPtr != 0 )
        return new CommitInfo( commitInfoPtr, pool );
    else
        return CommitInfo::Invalid;
}
// implementation of Client::Import
NSvn::Core::CommitInfo* NSvn::Core::Client::Import(String* path, String* url, Recurse recurse)
{
    SubPool pool(*(this->rootPool));;
    const char* trueSrcPath = CanonicalizePath( path, pool );
    const char* trueDstUrl = CanonicalizePath( url, pool );

    svn_client_commit_info_t* commitInfoPtr = 0;

    HandleError( svn_client_import( &commitInfoPtr, trueSrcPath, trueDstUrl, recurse == Recurse::None,
        this->context->ToSvnContext(), pool ) );

    if ( commitInfoPtr != 0 )
        return new CommitInfo( commitInfoPtr, pool );
    else
        return CommitInfo::Invalid;
}

// implementation of Client::Cat
void NSvn::Core::Client::Cat( Stream* out, String* path, Revision* revision )
{
    this->Cat( out, path, revision, revision );
}

void NSvn::Core::Client::Cat( Stream* out, String* path, 
                             Revision* pegRevision, Revision* revision )
{
    SubPool pool(*(this->rootPool));; 

    const char* truePath = CanonicalizePath( path, pool );
    svn_stream_t* svnStream = CreateSvnStream( out, pool );

    HandleError( svn_client_cat2( svnStream, truePath, 
        pegRevision->ToSvnOptRevision( pool ),
        revision->ToSvnOptRevision( pool ), 
        this->context->ToSvnContext(), pool ) );
}

// implementation of Client::Switch
int NSvn::Core::Client::Switch( String* path, String* url, Revision* revision, Recurse recurse)
{
    SubPool pool(*(this->rootPool));;

    const char* truePath = CanonicalizePath( path, pool );
    const char* trueUrl = CanonicalizePath( url, pool );

    svn_revnum_t rev;
    HandleError( svn_client_switch( &rev, truePath, trueUrl, revision->ToSvnOptRevision( pool ), recurse == Recurse::Full,
        this->context->ToSvnContext(), pool ) );

    return rev;

}

// implementation of Client::PropList
NSvn::Common::PropListItem* NSvn::Core::Client::PropList( String* path, Revision* revision, Recurse recurse ) []
{
    SubPool pool(*(this->rootPool));;

    const char* truePath = CanonicalizePath( path, pool );
    apr_array_header_t* propListItems;

    HandleError( svn_client_proplist( &propListItems, truePath, 
        revision->ToSvnOptRevision( pool ), recurse == Recurse::Full, this->context->ToSvnContext(),
        pool ) );

    return ConvertPropListArray( propListItems, pool );
}

// Implementation of Client::RevPropList
NSvn::Common::PropertyDictionary* NSvn::Core::Client::RevPropList( String* path,
                                                                  Revision* revision, System::Int32* revisionNumber ) 
{
    SubPool pool(*(this->rootPool));;

    const char* truePath = CanonicalizePath( path, pool );
    apr_hash_t* propListItems;

    svn_revnum_t revNo;
    HandleError( svn_client_revprop_list( &propListItems, truePath, 
        revision->ToSvnOptRevision( pool ), &revNo, 
        this->context->ToSvnContext(), pool ) );

    *revisionNumber = revNo;

    return ConvertToPropertyDictionary( propListItems, 0, pool );
}

// Implementation of Client::List
NSvn::Core::DirectoryEntry* NSvn::Core::Client::List(String* path, Revision* revision, 
                                                     Recurse recurse) []
{
    return this->List( path, revision, revision, recurse );
}

NSvn::Core::DirectoryEntry* NSvn::Core::Client::List(String* path, Revision* pegRevision,
                                                     Revision* revision, 
                                                     Recurse recurse) []
{
    SubPool pool(*(this->rootPool));;

    const char* truePath = CanonicalizePath( path, pool );
    apr_hash_t* entriesHash;

    HandleError( svn_client_ls2( &entriesHash, truePath, 
        pegRevision->ToSvnOptRevision( pool ),
        revision->ToSvnOptRevision( pool ), recurse == Recurse::Full,
        this->context->ToSvnContext(), pool ) );

    ArrayList* entries = new ArrayList();

    apr_hash_index_t* idx = apr_hash_first( pool, entriesHash );
    while( idx != 0 )
    {
        const char* path;
        apr_ssize_t keyLength;
        svn_dirent_t* dirent;

        apr_hash_this( idx, reinterpret_cast<const void**>(&path), &keyLength,
            reinterpret_cast<void**>(&dirent) );

        entries->Add( new DirectoryEntry( path, dirent, pool ) );

        idx = apr_hash_next( idx );
    }

    return static_cast<DirectoryEntry*[]>( 
        entries->ToArray( __typeof(DirectoryEntry) ) );
}

// implementation of Client::Diff
void NSvn::Core::Client::Diff( String* diffOptions[], String* path1, Revision* revision1,
                              String* path2, Revision* revision2, Recurse recurse, bool ignoreAncestry, bool noDiffDeleted, 
                              Stream* outfile, Stream* errfile )
{
    this->Diff( diffOptions, path1, revision1, path2, revision2, recurse, ignoreAncestry,
        noDiffDeleted, false, outfile, errfile );
}

void NSvn::Core::Client::Diff( String* diffOptions[], String* path1, Revision* revision1,
                              String* path2, Revision* revision2, Recurse recurse, bool ignoreAncestry, bool noDiffDeleted, 
                              bool ignoreContentType, Stream* outfile, Stream* errfile )
{
    SubPool pool(*(this->rootPool));;

    apr_array_header_t* diffOptArray = StringArrayToAprArray( diffOptions, false, pool );
    const char* truePath1 = CanonicalizePath( path1, pool );
    const char* truePath2 = CanonicalizePath( path2, pool );

    AprFileAdapter* outAdapter = new AprFileAdapter(outfile, pool);
    AprFileAdapter* errAdapter = new AprFileAdapter(errfile, pool);
    apr_file_t* aprOut = outAdapter->Start();
    apr_file_t* aprErr = errAdapter->Start();

    try
    {

    HandleError( svn_client_diff2( diffOptArray, truePath1, 
        revision1->ToSvnOptRevision( pool ), truePath2, 
        revision2->ToSvnOptRevision(pool), recurse == Recurse::Full, ignoreAncestry, noDiffDeleted,
        ignoreContentType,
        aprOut, aprErr, this->context->ToSvnContext(), pool ) );
    }
    __finally
    {
        apr_file_close( aprOut );
        apr_file_close( aprErr );
    }

    outAdapter->WaitForExit();
    errAdapter->WaitForExit();
}

String* NSvn::Core::Client::GetPristinePath(String* path)
{
    SubPool pool(*(this->rootPool));;

    const char* realPath = CanonicalizePath( path, pool );
    const char* pristinePath = NULL;
    HandleError(svn_wc_get_pristine_copy_path(realPath, &pristinePath, pool));

    if ( pristinePath ) 
        return Utf8ToString(  pristinePath, pool );
    else
        return 0;
}

// Implementation of Client::Log
void NSvn::Core::Client::Log( String* targets[], Revision* start, Revision* end, bool discoverChangePath, 
                             bool strictNodeHistory, LogMessageReceiver* receiver )
{
    SubPool pool(*(this->rootPool));;

    apr_array_header_t* aprTargets = StringArrayToAprArray( targets, true, pool );

    ManagedPointer<NSvn::Core::LogMessageReceiver*> ptr(receiver);

    HandleError( svn_client_log( aprTargets, start->ToSvnOptRevision(pool), 
        end->ToSvnOptRevision(pool), discoverChangePath, 
        strictNodeHistory, svn_log_message_receiver, &ptr,
        this->context->ToSvnContext(), pool ) ); 
}

// Implementation of Client::Relocate
void NSvn::Core::Client::Relocate( String* dir, String* from, String* to,
                                  Recurse recurse )
{
    SubPool pool(*(this->rootPool));;
    HandleError( svn_client_relocate( 
        CanonicalizePath( dir, pool ),
        CanonicalizePath( from, pool ),
        CanonicalizePath( to, pool ),
        recurse == Recurse::Full,
        this->context->ToSvnContext(), pool ) );
}

// implementation of Client::UrlFromPath
String* NSvn::Core::Client::UrlFromPath( String* path )
{
    SubPool pool(*(this->rootPool));;

    const char* realPath = CanonicalizePath( path, pool );

    const char * url;

    HandleError( svn_client_url_from_path( &url, realPath, pool ) );

    if ( url ) 
        return Utf8ToString(  url, pool );
    else
        return 0;
}

// implementation of Client::UuidFromUrl
String* NSvn::Core::Client::UuidFromUrl( String* url )
{
    SubPool pool(*(this->rootPool));;

    const char* realUrl = CanonicalizePath( url, pool );
    const char* uuid;

    HandleError( svn_client_uuid_from_url( &uuid, realUrl, this->context->ToSvnContext(),
        pool ) );

    return Utf8ToString(  uuid, pool );
}


bool NSvn::Core::Client::HasBinaryProp( String* path )
{
    SubPool pool(*(this->rootPool));;
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

__gc class StatusHolder
{
public:
    StatusHolder() : Path(0), Status(0)
    {
    }

    void Callback( String* path, NSvn::Core::Status* status )
    {
        // we assume that paths to files are always longer than their parent dir
        if ( (this->Path == 0) || (path->Length < this->Path->Length) )
        {
            this->Path = path;
            this->Status = status;
        }
    }
    String* Path;
    NSvn::Core::Status* Status;
};

bool NSvn::Core::Client::IsIgnored( String* path )
{
    StatusHolder* holder = new StatusHolder();

    StatusCallback * callBack = new StatusCallback( holder, &StatusHolder::Callback );

    // Status() takes care of canonicalizing etc
    int youngest;
    this->Status( &youngest, path, Revision::Working, 
        callBack , false, true,
        false, false, false );

    if ( holder->Status != 0 )
        return holder->Status->TextStatus == StatusKind::Ignored;
    else
        return false;
}

void NSvn::Core::Client::OnNotification( NotificationEventArgs* args )
{
    if ( this->Notification != 0 )
        this->Notification->Invoke( this, args );
}

void NSvn::Core::Client::OnProgress( ProgressEventArgs* args )
{
    if ( this->Progress != 0 )
        this->Progress->Invoke( this, args );
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
		System::Runtime::InteropServices::Marshal::Copy( 
			const_cast<char*>(propVal->data), bytes, 0, propVal->len );

        // Add it to the mapping collection as a Property object        
        if ( propertyName != 0 )
        {
            Property* property = new Property( propertyName, bytes );
            mapping->Add( ToNativePath( key, pool ), property );
        }
        else 
        {
            String* name = Utf8ToString(  key, pool );
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
        String* nodeName = Utf8ToString(  item->node_name->data, pool );
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
void svn_status_func2(void* baton, const char* path, svn_wc_status2_t* status)
{
    using namespace NSvn::Core;

    StatusBaton* statusBaton = static_cast<StatusBaton*>(baton);
    StatusCallback* statusCallback = *(static_cast<ManagedPointer<StatusCallback*>*>(statusBaton->statusCallback));

    String* nativePath = ToNativePath( path, *(statusBaton->pool) );

    statusCallback->Invoke( nativePath, new Status( status, *(statusBaton->pool) ) );
}

// marshall the blame callback into managed space
svn_error_t* svn_blame_func(void *baton, apr_int64_t line_no, svn_revnum_t revision, 
                    const char *author, const char *date, 
                    const char *line, apr_pool_t *pool )
{
    using namespace NSvn::Core;
    BlameReceiver* receiver = *(static_cast<ManagedPointer<BlameReceiver*>*>(baton));

    // The date's a ISO-8601 timestamp
    DateTime dt;
    try
    {
        if ( date != 0 )
        {
            dt = DateTime::ParseExact( Utf8ToString( date, pool ),
                "yyyy-M-d\\TH:m:s.ffffff\\Z",
                System::Globalization::CultureInfo::InvariantCulture ).ToLocalTime();
        }
        else
            dt = DateTime::MinValue;
    }
    catch( FormatException* ex )
    {
        String* msg = String::Concat( ex->Message, 
            String::Concat( Environment::NewLine, Utf8ToString(date, pool) ) );
        return svn_error_create( SVN_ERR_BAD_DATE, NULL, 
			StringToUtf8(msg, pool) );
    }

    try
    {
        String* authorString = Utf8ToString( author, pool );
        if ( authorString == 0 )
            authorString = "";

        receiver->Invoke( line_no, revision, authorString, dt, 
            Utf8ToString(line, pool) );
    }
    catch( Exception* ex )
    {
        return svn_error_create( SVN_ERR_BASE, NULL, StringToUtf8(ex->Message, pool) );
    }  
    return SVN_NO_ERROR;
}


