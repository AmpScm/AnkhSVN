// $Id$

#include "stdafx.h"

#include "ClientContext.h"
#include "UsernameCredential.h"
#include "CommitItem.h"

#include <svn_subst.h>
//#include <svn_client.h>
//#include <apr_pools.h>
#include "Notification.h"
//#include "ManagedPointer.h"

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
            ctx->auth_baton = this->CreateAuthBaton( pool, this->AuthBaton );

            // log message callback?
            if ( this->LogMessageCallback != 0 )
            {
                ctx->log_msg_func = log_msg_func;
                ctx->log_msg_baton = pool.AllocateObject( 
                    ManagedPointer<NSvn::Core::LogMessageCallback*>( 
                    this->LogMessageCallback ) );
            }

            
            // client configuration
            if ( this->ClientConfig != 0 )
            {
                throw new Exception( "This isnt implemented yet" );
            }
            else
            {  
                // TODO: We just put an empty table here for now
                ctx->config = apr_hash_make( pool );
            }

            return ctx; 


        }

        svn_auth_baton_t* ClientContext::CreateAuthBaton( const Pool& pool, 
            AuthenticationBaton* baton )
        {
            apr_array_header_t* providers = 0;

            // any providers provided?
            if ( baton == 0 )
            {
                // Create one anyway
                baton = new AuthenticationBaton();
            }

            // create an array to put our providers in, leaving room for the LoggedInUser provider
            providers = apr_array_make( pool, baton->Providers->Count + 1, 
                sizeof( svn_auth_provider_object_t* ) );

            // always put this one in, since it seems to be needed
            // TODO: is there a better way?
            svn_auth_provider_object_t* providerObject =
                this->CreateProvider( pool, new SimpleProvider( 
                UsernameCredential::LoggedInUser ) );
            *((svn_auth_provider_object_t **)apr_array_push (providers)) = 
                    providerObject;


            // put our providers in the array
            for( int i = 0; i < baton->Providers->Count; i++ )
            {
                svn_auth_provider_object_t* providerObject = 
                    this->CreateProvider( pool, baton->Providers->Item[i] );

                *((svn_auth_provider_object_t **)apr_array_push (providers)) = 
                    providerObject;
            }
         


            // create the actual baton
            svn_auth_baton_t* ab;
            svn_auth_open( &ab, providers, pool );

            return ab;
        }

        svn_auth_provider_object_t* ClientContext::CreateProvider( 
            const Pool& pool, IAuthenticationProvider* authProvider )
        {            

            svn_auth_provider_t* provider = static_cast<svn_auth_provider_t*>(
                pool.PCalloc( sizeof(*provider) ) );
            provider->cred_kind = StringHelper( authProvider->Kind ).CopyToPool( pool );

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

            return providerObject;

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

    void* GetCredentials( ICredential* credential, apr_pool_t* pool  )
    {
        // did we get a valid credential?
        if ( credential != 0 )
        {            
            return credential->GetCredential( pool ).ToPointer();
        }
        else 
            return 0;
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

        *credentials = GetCredentials( provider->FirstCredentials(), pool );
       
        // next_creds doesnt have a provider_baton param, so we store it in
        // the iter baton. We don't need it for anything else, since 
        // the IAuth... object can maintain it's own context
        *iter_baton = provider_baton;


        // everything is aok
        return SVN_NO_ERROR;
    }

    svn_error_t * next_credentials (void **credentials,
                                     void *iter_baton,
                                     apr_hash_t *parameters,
                                     apr_pool_t *pool)
    {
        IAuthenticationProvider* provider = 
            *(static_cast<ManagedPointer<IAuthenticationProvider*>*>(iter_baton) );

        *credentials = GetCredentials( provider->NextCredentials(), pool );

        // nothing can ever go wrong here
        return SVN_NO_ERROR;
    }


    svn_error_t * save_credentials (svn_boolean_t *saved,
                                     void *credentials,
                                     void *provider_baton,
                                     apr_hash_t *parameters,
                                     apr_pool_t *pool)
    {
        // TODO: implement this
        *saved = false;
        return SVN_NO_ERROR;
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
