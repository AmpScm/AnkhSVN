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
#include "LogMessageEventArgs.h"
#include "CancelEventArgs.h"
//#include "ManagedPointer.h"

namespace
{
    void notify_func( void *baton, const char *path, 
        svn_wc_notify_action_t action, svn_node_kind_t kind, 
        const char *mime_type, svn_wc_notify_state_t content_state, 
        svn_wc_notify_state_t prop_state, svn_revnum_t revision );
    
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

        svn_client_ctx_t* ClientContext::ToSvnContext( const Pool& pool )
        {
            // PCalloc zeros out the returned mem

            svn_client_ctx_t* ctx;
            HandleError( svn_client_create_context( &ctx, pool ) );

            void* clientBaton = pool.AllocateObject(
                ManagedPointer<NSvn::Core::Client*>(
                this->client) );

            ctx->notify_func = notify_func;
            ctx->notify_baton = clientBaton;

            // is there an auth baton? (should be)
            if ( this->authBaton != 0 )
                ctx->auth_baton = this->authBaton->GetAuthBaton();

            
            ctx->log_msg_func = log_msg_func;
            ctx->log_msg_baton = clientBaton;


            ctx->cancel_func = cancel_func;
            ctx->cancel_baton = clientBaton;

            // client configuration
            if ( this->ClientConfig == 0 )
                this->ClientConfig = new NSvn::Core::ClientConfig();
            
            ctx->config = this->clientConfig->GetHash();
           
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

namespace
{
    using namespace NSvn::Core;

    void notify_func( void *baton, const char *path, 
        svn_wc_notify_action_t action, svn_node_kind_t kind, 
        const char *mime_type, svn_wc_notify_state_t content_state, 
        svn_wc_notify_state_t prop_state, svn_revnum_t revision )
    {
        NotificationEventArgs* args = new NotificationEventArgs( path, action, kind,
            mime_type, content_state, prop_state, revision );
        Client* client = 
            *(static_cast<ManagedPointer<Client*>* >(baton) );
        client->OnNotification( args );
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
        client->OnLogMessage( args );
        const char* logMessage = StringHelper( args->Message ).CopyToPool(pool);

        // a null indicates a canceled commit
        if ( logMessage != 0 )
        {
            svn_string_t *logMsgString = svn_string_create ("", pool);

            logMsgString->data = logMessage;
            logMsgString->len = strlen(logMessage);

            svn_string_t* encodedString = svn_string_create( "", pool );

            HandleError( svn_subst_translate_string( &encodedString, 
                logMsgString, NULL, pool ) );

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
        client->OnCancel( args );

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
