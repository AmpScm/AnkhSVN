// $Id$

#include "stdafx.h"

#include "ClientContext.h"
#include "CommitItem.h"
#include "ParameterDictionary.h"

#include <svn_subst.h>
//#include <svn_client.h>
//#include <apr_pools.h>
#include "Notification.h"
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
}

namespace NSvn
{
    namespace Core
    {

        svn_client_ctx_t* ClientContext::ToSvnContext( const Pool& pool )
        {
            // PCalloc zeros out the returned mem
            svn_client_ctx_t* ctx = static_cast<svn_client_ctx_t*>(
                pool.PCalloc( sizeof( svn_client_ctx_t ) ) );

            // is there a notify callback? (usually is)
            if ( this->NotifyCallback != 0 )
            {
                ctx->notify_func = notify_func;
                ctx->notify_baton = pool.AllocateObject( 
                    ManagedPointer<NSvn::Core::NotifyCallback*>( this->NotifyCallback ) );
            }

            // is there an auth baton? (should be)
            if ( this->authBaton != 0 )
                ctx->auth_baton = this->authBaton->GetAuthBaton();

            // log message callback?
            if ( this->LogMessageCallback != 0 )
            {
                ctx->log_msg_func = log_msg_func;
                ctx->log_msg_baton = pool.AllocateObject( 
                    ManagedPointer<NSvn::Core::LogMessageCallback*>( 
                    this->LogMessageCallback ) );
            }

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
        Notification* notification = new Notification( path, action, kind,
            mime_type, content_state, prop_state, revision );
        NotifyCallback* callback = 
            *(static_cast<ManagedPointer<NotifyCallback*>* >(baton) );
        callback->Invoke( notification );

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

        LogMessageCallback* callback = *(static_cast<
            ManagedPointer<LogMessageCallback*>* >( baton ) );

        // get the log message
        StringHelper logMessage( callback->Invoke( items ) );

        // a null indicates a canceled commit
        if ( static_cast<const char*>(logMessage) != 0 )
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
