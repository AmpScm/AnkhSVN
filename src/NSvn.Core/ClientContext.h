// $Id$
#pragma once
#using <mscorlib.dll>

#include "delegates.h"
#include "AdminAccessBaton.h"
#include "ClientConfig.h"
#include "ManagedPointer.h"
#include "Pool.h"
#include <svn_client.h>
#include <svn_auth.h>
#include <apr_pools.h>
#include <apr_tables.h>

#using <NSvn.Common.dll>




namespace
{
    void notify_func( void *baton, const char *path, 
        svn_wc_notify_action_t action, svn_node_kind_t kind, 
        const char *mime_type, svn_wc_notify_state_t content_state, 
        svn_wc_notify_state_t prop_state, svn_revnum_t revision );
    svn_error_t * first_credentials  (void **credentials,
                                      void **iter_baton,
                                      void *provider_baton,
                                      apr_hash_t *parameters,
                                      apr_pool_t *pool);
    svn_error_t * next_credentials (void **credentials,
                                     void *iter_baton,
                                     apr_hash_t *parameters,
                                     apr_pool_t *pool);
    svn_error_t * save_credentials (svn_boolean_t *saved,
                                     void *credentials,
                                     void *provider_baton,
                                     apr_hash_t *parameters,
                                     apr_pool_t *pool);


}

namespace NSvn
{
    namespace Core
    {
        using namespace NSvn::Common;

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
            /// <summary> Creates an svn_auth_baton_t* from a 
            /// AuthenticationBaton object</summary>
            svn_auth_baton_t* CreateAuthBaton( const Pool& pool, AuthenticationBaton* baton );

            /// <summary> Creates an svn_auth_provider_object_t* from an IAuthenticationProvider
            /// </summary>
            svn_auth_provider_object_t* CreateProvider( 
                const Pool& pool, IAuthenticationProvider* authProvider );

            NSvn::Common::AuthenticationBaton* authBaton;
            NSvn::Core::PromptCallback* promptCallback;
            NSvn::Core::NotifyCallback* notifyCallback;
            NSvn::Core::LogMessageCallback* logMessageCallback;
            NSvn::Core::ClientConfig* clientConfig;

            
        };
    }
}


        
