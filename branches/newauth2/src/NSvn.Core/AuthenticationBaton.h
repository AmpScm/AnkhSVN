// $Id$

#pragma once
#include <svn_auth.h>
#include <svn_client.h>
#include <svn_pools.h>
#include <apr_tables.h>
#include "AuthenticationProviderObject.h"
#include "GCPool.h"


namespace NSvn
{
    namespace Core
    {
        using namespace System::Collections;
        using namespace System;

        public __gc class AuthenticationBaton
        {
        public:
            /// <summary>Constructor</summary>
            AuthenticationBaton() : dirty( true ), authBaton(0)
            {
                this->providerObjects = new ArrayList();
                this->pool = new GCPool();
            }

            AuthenticationBaton( ICollection* providerObjects ) : dirty( true ), authBaton(0)
            {
                this->providerObjects = new ArrayList( providerObjects );
                this->pool = new GCPool();
            }            

            /// <summary>Add an authentication provider</summary>
            void Add( AuthenticationProviderObject* obj )
            {
                this->providerObjects->Add( obj );
            }

            /// <summary>Add multiple authentication providers</summary>
            void Add( ICollection* providerObjects )
            {
                this->providerObjects->Add( providerObjects );
            }

            /// <summmary>Set a runtime parameter in the baton</summary>
            void SetParameter( String* name, String* value )
            {
                this->CreateAuthBaton();
                svn_auth_set_parameter( this->authBaton, 
                    StringHelper(name).CopyToPool(this->pool->ToAprPool()), 
                    StringHelper(value).CopyToPool(this->pool->ToAprPool()) );
            }

            /// <summary>Get a runtime parameter from the baton</summary>
            String* GetParameter( String* name )
            {
                this->CreateAuthBaton();
                const void* param = svn_auth_get_parameter( this->authBaton,
                    StringHelper(name) );

                return StringHelper( static_cast<const char*>(param) );
            }

            // TODO: docstrings here
            static String* const ParamDefaultUsername = SVN_AUTH_PARAM_DEFAULT_USERNAME;
            static String* const ParamDefaultPassword = SVN_AUTH_PARAM_DEFAULT_PASSWORD;
            static String* const ParamNonInteractive = SVN_AUTH_PARAM_NON_INTERACTIVE;
            static String* const ParamNoAuthCache = SVN_AUTH_PARAM_NO_AUTH_CACHE;
            static String* const ParamSslServerFailures = SVN_AUTH_PARAM_SSL_SERVER_FAILURES;
            static String* const ParamSslServerCertInfo = SVN_AUTH_PARAM_SSL_SERVER_CERT_INFO;
            static String* const ParamConfig = SVN_AUTH_PARAM_CONFIG;
            static String* const ParamServerGroup = SVN_AUTH_PARAM_SERVER_GROUP;
            static String* const ParamConfigDir = SVN_AUTH_PARAM_CONFIG_DIR;
            
        private public:
            svn_auth_baton_t* GetAuthBaton()
            {
                this->CreateAuthBaton();

                // and return it
                return this->authBaton;
            }


        private:
            // actually creates the auth baton
            void CreateAuthBaton()
            {
                // don't bother if the one we have is up to date
                if ( !this->dirty )
                    return;  

                svn_pool_clear( this->pool->ToAprPool() );

                // put all the providers in an apr array
                apr_array_header_t* providers = apr_array_make( 
                    this->pool->ToAprPool(), this->providerObjects->Count, 
                    sizeof(svn_auth_provider_object_t*) );

                for( int i = 0; i < this->providerObjects->Count; i++ )
                    APR_ARRAY_PUSH( providers, svn_auth_provider_object_t* ) = 
                        static_cast<AuthenticationProviderObject*>(
                            this->providerObjects->get_Item(i))->GetProvider();


                // create the baton
                svn_auth_baton_t* baton;
                svn_auth_open( &baton, providers, pool->ToAprPool() );

                this->authBaton = baton;
                this->dirty = false;
            }


            ArrayList* providerObjects;
            bool dirty;
            svn_auth_baton_t* authBaton;
            GCPool* pool;

        };
    }
}