// $Id$

#pragma once
#include <svn_auth.h>
#include <svn_client.h>
#include <svn_pools.h>
#include <apr_tables.h>
#include "AuthenticationProvider.h"
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
                this->parameters = new Hashtable();
                this->pool = new GCPool();
            }

            AuthenticationBaton( ICollection* providerObjects ) : dirty( true ), authBaton(0)
            {
                this->providerObjects = new ArrayList( providerObjects );
                this->parameters = new Hashtable();
                this->pool = new GCPool();
            }            

            /// <summary>Add an authentication provider</summary>
            void Add( AuthenticationProvider* obj )
            {                
                this->providerObjects->Add( obj );
                this->dirty = true;
            }

            /// <summary>Add multiple authentication providers</summary>
            void Add( ICollection* providerObjects )
            {
                this->providerObjects->Add( providerObjects );
                this->dirty = true;
            }

            /// <summmary>Set a runtime parameter in the baton</summary>
            void SetParameter( String* name, String* value )
            {
                this->parameters->set_Item( name, value );
                this->dirty = true;
            }

            /// <summary>Get a runtime parameter from the baton</summary>
            String* GetParameter( String* name )
            {
                return static_cast<String*>( 
                    this->parameters->get_Item( name ) );
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
                        static_cast<AuthenticationProvider*>(
                            this->providerObjects->get_Item(i))->GetProvider();


                // create the baton
                svn_auth_baton_t* baton;
                svn_auth_open( &baton, providers, pool->ToAprPool() );
                this->authBaton = baton;

                // set the parameters
                IDictionaryEnumerator* iter = this->parameters->GetEnumerator();
                while( iter->MoveNext() )
                {
					const char* name = StringToUtf8(static_cast<String*>(iter->get_Key()), 
						this->pool->ToAprPool());
					const char* val = StringToUtf8(static_cast<String*>(iter->get_Value()), 
						this->pool->ToAprPool());					

                    svn_auth_set_parameter( this->authBaton, name, val );
                }

                
                this->dirty = false;
            }


            Hashtable* parameters;
            ArrayList* providerObjects;
            bool dirty;
            svn_auth_baton_t* authBaton;
            GCPool* pool;

        };
    }
}