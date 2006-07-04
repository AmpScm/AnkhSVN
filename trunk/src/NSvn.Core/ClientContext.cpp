// $Id$

#include "stdafx.h"

#include "ClientContext.h"
#include "Client.h"
#include "CommitItem.h"
#include "ParameterDictionary.h"

#include <svn_subst.h>
//#include <svn_client.h>
//#include <apr_pools.h>
#include "NotificationEventArgs.h"
#include "ProgressEventArgs.h"
#include "LogMessageEventArgs.h"
#include "CancelEventArgs.h"
//#include "ManagedPointer.h"
#include "Utils.h"

namespace
{
    void notify_func( void *baton, const svn_wc_notify_t* notify, 
        apr_pool_t* pool );
    
    void progress_func( apr_off_t progress, apr_off_t total, 
        void *baton, apr_pool_t *pool );

    svn_error_t* log_msg_func( const char **log_msg, 
        const char **tmp_file, 
        apr_array_header_t *commit_items, 
        void *baton, 
        apr_pool_t *pool);  

    svn_error_t* cancel_func( void* baton );
}

namespace NSvn
{
    namespace Core
    {

        svn_client_ctx_t* ClientContext::ToSvnContext( )
        {
            // already initialized?
            if ( this->context != 0 )
                return this->context;

            svn_client_ctx_t* ctx;
            Pool* pool = this->client->GetPool();
            HandleError( svn_client_create_context( &ctx, *pool ) );

            void* clientBaton = pool->AllocateObject(
                ManagedPointer<NSvn::Core::Client*>(
                this->client) );

            ctx->notify_func2 = notify_func;
            ctx->notify_baton2 = clientBaton;

            // is there an auth baton? (should be)
            if ( this->authBaton != 0 )
                ctx->auth_baton = this->authBaton->GetAuthBaton();

            ctx->progress_func = progress_func;
            ctx->progress_baton = clientBaton;

            
            ctx->log_msg_func = log_msg_func;
            ctx->log_msg_baton = clientBaton;


            ctx->cancel_func = cancel_func;
            ctx->cancel_baton = clientBaton;

            // client configuration
            if ( this->ClientConfig == 0 )
                this->ClientConfig = new NSvn::Core::ClientConfig();
            
            ctx->config = this->clientConfig->GetHash();
           
            this->context = ctx;
            return ctx; 
        }        
    }
}

// necessary to get the managed compiler to generate metadata for the types
struct apr_pool_t
{};

struct svn_auth_baton_t
{};

struct apr_hash_t
{};

struct svn_config_t
{};

struct apr_allocator_t
{};

namespace
{
    using namespace NSvn::Core;

    void notify_func( void *baton, const svn_wc_notify_t* notify, apr_pool_t* pool )
    {
         String* nativePath = ToNativePath( notify->path, pool );

        NotificationEventArgs* args = new NotificationEventArgs( nativePath, 
            notify->action, notify->kind,
            notify->mime_type, notify->content_state, notify->prop_state, 
            notify->revision, pool );
        Client* client = 
            *(static_cast<ManagedPointer<Client*>* >(baton) );

        try
        {
            client->OnNotification( args );
        }
        catch( Exception* ex )
        {
            // Swallow - we cannot let it propagate back into
            // Subversion code
            System::Diagnostics::Trace::WriteLine( ex );
        }
    }

    void progress_func( apr_off_t progress, apr_off_t total, void *baton, apr_pool_t *pool )
    {

        Client* client = 
            *(static_cast<ManagedPointer<Client*>* >(baton) );

        ProgressEventArgs* args = new ProgressEventArgs(static_cast<long>(progress), static_cast<long>(total));

        try
        {
            client->OnProgress( args );
        }
        catch( Exception* ex )
        {
            // Swallow - we cannot let it propagate back into
            // Subversion code
            System::Diagnostics::Trace::WriteLine( ex );
            Console::WriteLine(ex);
        }
    }


    // delegate the log message callback back into managed space
    svn_error_t* log_msg_func( const char **log_msg, 
        const char **tmp_file, 
        apr_array_header_t *commit_items, 
        void *baton, 
        apr_pool_t *pool)
    {
        // convert all the commit items to CommitItem objects
        CommitItem* items[] = new CommitItem*[ commit_items->nelts ];
        for( int i = 0; i < commit_items->nelts; i++ )
        {
            svn_client_commit_item_t* item = ((svn_client_commit_item_t**)
                (commit_items->elts))[i];
            items[ i ] = new CommitItem( item, pool );
        }

        //TODO: should we support tmp_file?
        *tmp_file = 0;

        Client* client = *(static_cast<
            ManagedPointer<Client*>* >( baton ) );

        LogMessageEventArgs* args = new LogMessageEventArgs( items );

        // get the log message
        try
        {
            client->OnLogMessage( args );
        }
        catch( Exception* ex )
        {
            // Swallow - we can't let it propagate back into Subversion code
            System::Diagnostics::Trace::WriteLine( ex );
            return svn_error_create( SVN_ERR_CL_BAD_LOG_MESSAGE, NULL, 
                "Exception thrown by managed event handler" );
        }
        const char* logMessage = StringToUtf8( args->Message, pool );

        // a null indicates a canceled commit
        if ( logMessage != 0 )
        {
            svn_string_t *logMsgString = svn_string_create ("", pool);

            logMsgString->data = logMessage;
            logMsgString->len = strlen(logMessage);

            svn_string_t* encodedString = svn_string_create( "", pool );

            HandleError( svn_subst_translate_string( &encodedString, 
                logMsgString, "UTF-8", pool ) );

            *log_msg = encodedString->data;
        }
        else
            *log_msg = 0;

        return SVN_NO_ERROR;
    }

    // Delegate the callback function into managed space
    svn_error_t* cancel_func( void* baton )
    {
        Client* client = *(static_cast<ManagedPointer<Client*>*>(
            baton) );
        CancelEventArgs* args = new CancelEventArgs();

        try
        {
            client->OnCancel( args );
        }
        catch( Exception* ex )
        {
            // Swallow - we cannot let it propagate back into Subversion
            System::Diagnostics::Trace::WriteLine( ex );
            // TODO: return some other error?
            return svn_error_create( SVN_ERR_BASE, NULL, 
                "Exception thrown by managed event handler" );;
        }

        if ( args->Cancel ) 
            return svn_error_create( SVN_ERR_CANCELLED, NULL, "caught SIGINT" );
        else
            return SVN_NO_ERROR;
    }


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
