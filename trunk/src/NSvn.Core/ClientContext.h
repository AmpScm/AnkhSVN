#pragma once
#using <mscorlib.dll>
#using <NSvn.Common.dll>

#include "delegates.h"
#include "AdminAccessBaton.h"
#include "ClientConfig.h"
#include "ManagedPointer.h"
#include "Pool.h"
#include <svn_client.h>
#include <apr_pools.h>

using namespace NSvn::Common;



namespace
{
    void notify_func( void *baton, const char *path, 
        svn_wc_notify_action_t action, svn_node_kind_t kind, 
        const char *mime_type, svn_wc_notify_state_t content_state, 
        svn_wc_notify_state_t prop_state, svn_revnum_t revision );
}

namespace NSvn
{
    namespace Core
    {
        // .NET representation of the svn_client_ctx_t structure
        public __gc class ClientContext
        {
        public:
            ClientContext( NotifyCallback* callback ) : notifyCallback( callback )
            {;}

            ClientContext( NotifyCallback* callback, AuthenticationBaton* authBaton ) :
                notifyCallback( callback ), authBaton( authBaton )
                {;}

            ClientContext( NotifyCallback* callback, AuthenticationBaton* authBaton, 
                ClientConfig* config ) :
                notifyCallback( callback ), authBaton( authBaton ), clientConfig( config ) 
                {;}

            // An authentication baton
            __property AuthenticationBaton* get_AuthBaton()
            { return this->authBaton; }
            __property void set_AuthBaton( AuthenticationBaton* value )
            { this->authBaton = value; }

            // A callback delegate for prompts
            __property PromptCallback* get_PromptCallback()
            { return this->promptCallback; }
            __property void set_PromptCallback( NSvn::Core::PromptCallback* value )
            { this->promptCallback = value; }

            // Callback delegate for notifications
            __property NotifyCallback* get_NotifyCallback()
            { return this->notifyCallback; }
            __property void set_NotifyCallback( NSvn::Core::NotifyCallback* value )
            { this->notifyCallback = value; }

            // Callback delegate for log messages
            __property LogMessageCallback* get_LogMessageCallback()
            { return this->logMessageCallback; }
            __property void set_LogMessageCallback( NSvn::Core::LogMessageCallback* value )
            { this->logMessageCallback = value; }

            // The client configuration
            __property ClientConfig* get_ClientConfig()
            { return this->clientConfig; }
            __property void set_ClientConfig( NSvn::Core::ClientConfig* value )
            { this->clientConfig = value; }

            svn_client_ctx_t* ToSvnContext( const Pool& pool );

        private:
            NSvn::Common::AuthenticationBaton* authBaton;
            NSvn::Core::PromptCallback* promptCallback;
            NSvn::Core::NotifyCallback* notifyCallback;
            NSvn::Core::LogMessageCallback* logMessageCallback;
            NSvn::Core::ClientConfig* clientConfig;
        };

        

        // implementation


        inline svn_client_ctx_t* ClientContext::ToSvnContext( const Pool& pool )
        {
            svn_client_ctx_t* ctx = static_cast<svn_client_ctx_t*>(
                apr_palloc( pool, sizeof( svn_client_ctx_t ) ) );

            if ( this->NotifyCallback != 0 )
            {
                ctx->notify_func = notify_func;

                ctx->notify_baton = pool.Allocate( 
                    ManagedPointer<NSvn::Core::NotifyCallback*>( this->NotifyCallback ) );
            }

            return ctx; 


        }
    }
}

// necessary to get the managed compiler to generate metadata for the type
struct apr_pool_t
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


}