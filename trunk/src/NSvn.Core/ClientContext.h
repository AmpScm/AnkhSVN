#pragma once
#using <mscorlib.dll>
#using <NSvn.Common.dll>

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

        

        // implementation


        inline svn_client_ctx_t* ClientContext::ToSvnContext( const Pool& pool )
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

            //// is there an auth baton? (should be)
            //if ( this->AuthBaton != 0 )
            //{
            //    ctx->auth_baton = this->CreateAuthBaton( pool, this->AuthBaton );
            //}

            return ctx; 


        }

        inline svn_auth_baton_t* ClientContext::CreateAuthBaton( const Pool& pool, 
            AuthenticationBaton* baton )
        {
            // create an array to put our providers in
            apr_array_header_t* providers = apr_array_make( pool, baton->Providers->Count, 
                sizeof( svn_auth_provider_object_t* ) );

            // put our providers in the array
            for( int i = 0; i < baton->Providers->Count; i++ )
            {
                IAuthenticationProvider* pr = baton->Providers->Item[i];
                svn_auth_provider_object_t* providerObject = 0;/*
                    this->CreateProvider( pool, baton->Providers->Item(i) );*/

                *((svn_auth_provider_object_t **)apr_array_push (providers)) = 
                    providerObject;
            }

            return 0;
        }

        inline svn_auth_provider_object_t* ClientContext::CreateProvider( 
            const Pool& pool, IAuthenticationProvider* authProvider )
        {            

            svn_auth_provider_t* provider = static_cast<svn_auth_provider_t*>(
                pool.PCalloc( sizeof(*provider) ) );
            provider->cred_kind = SVN_AUTH_CRED_SIMPLE;

            // these simple callback functions merely delegate to our IAuth object
            provider->first_credentials = first_credentials;
            provider->next_credentials = next_credentials;
            provider->save_credentials = save_credentials;

            // create a simple provider - we handle the complexity on our end
            svn_auth_provider_object_t* providerObject = static_cast<svn_auth_provider_object_t*>(
                pool.PCalloc( sizeof(*provider) ) );
            providerObject->vtable = provider;

            //store a pointer to our .NET IAuth... object in the baton
            providerObject->provider_baton = pool.AllocateObject( 
                ManagedPointer<IAuthenticationProvider*>( authProvider ) );

            return 0;

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


    svn_error_t * first_credentials  (void **credentials,
                                      void **iter_baton,
                                      void *provider_baton,
                                      apr_hash_t *parameters,
                                      apr_pool_t *pool)
    {
        // delegate to the IAuth.. object
        IAuthenticationProvider* provider = 
            *( static_cast<ManagedPointer<IAuthenticationProvider*>* >(provider_baton) );

        Credential* credential = provider->FirstCredentials();
        /*if ( credential != Credential.InvalidCredentials )
        {
            svn_auth_cred_simple_t *creds = apr_pcalloc (pool, sizeof(*creds));
            creds->*/
        return 0;


    }

    svn_error_t * next_credentials (void **credentials,
                                     void *iter_baton,
                                     apr_hash_t *parameters,
                                     apr_pool_t *pool)
    {
        return 0;
    }


    svn_error_t * save_credentials (svn_boolean_t *saved,
                                     void *credentials,
                                     void *provider_baton,
                                     apr_hash_t *parameters,
                                     apr_pool_t *pool)
    {
        return 0;
    }


}