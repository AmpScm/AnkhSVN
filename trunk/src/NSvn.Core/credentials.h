// $Id$

#pragma once
#include "StdAfx.h"
#include <svn_auth.h>
#include "StringHelper.h"
#include <apr_strings.h>
#include "svnenums.h"

namespace NSvn
{
    namespace Core
    {
        using namespace System;

        /// TODO: make stuff public
        /// <summary>Represents a simple credential obtained by prompting the user
        /// for a username and/or password </summary>
        public __gc class SimpleCredential
        {
        public:
            SimpleCredential( String* username, String* password ) :
                username(username), password(password)
            {;}

            __property String* get_UserName()
            { return this->username; }

            __property void set_Username( String* username )
            { this->username = username; }

            __property String* get_Password()
            { return this->password; }

            __property void set_Password( String* password )
            { this->password = password; }

        public private:
            /// <summary>Convert to an svn_auth_cred_simple_t*</summary>
          svn_auth_cred_simple_t* GetCredential( apr_pool_t* pool )
          {
              svn_auth_cred_simple_t* cred = static_cast<svn_auth_cred_simple_t*>( 
                  apr_palloc( pool, sizeof(*cred) ) );
              cred->username = apr_pstrdup( pool, StringHelper( username ) );
              cred->password = apr_pstrdup( pool, StringHelper( password ) );
              return cred;
          }
        private:
            String* username;
            String* password;
        };

        /// <summary>Represents a credential stating which certificates to trust.
        /// </summary>
        public __gc class SslServerTrustCredential
        {
        public:
            /// <summary>Trust this certificate permanently?</summary>
            bool MaySave;

            /// <summary>A collection of flags of which kinds of failures should be accepted 
            /// permanently.</summary>
            SslFailures AcceptedFailures;
        public private:
            svn_auth_cred_ssl_server_trust_t* GetCredential( apr_pool_t* pool )
            {
                svn_auth_cred_ssl_server_trust_t* cred = 
                    static_cast<svn_auth_cred_ssl_server_trust_t*>( 
                        apr_pcalloc(pool, sizeof(*cred)) );
                cred->accepted_failures = static_cast<int>(this->AcceptedFailures);
                cred->may_save = this->MaySave;
                return cred;
            }
        };

        /// <summary>Represents a client certificate credential. </summary>
        public __gc class SslClientCertificateCredential
        {
        public:
            String* CertificateFile;
        public private:
            svn_auth_cred_ssl_client_cert_t* GetCredential( apr_pool_t* pool )
            {
                svn_auth_cred_ssl_client_cert_t* cred = 
                    static_cast<svn_auth_cred_ssl_client_cert_t*>(
                        apr_pcalloc(pool, sizeof(*cred) ) );
                cred->cert_file = StringHelper( this->CertificateFile ).CopyToPool( pool );

                return cred;
            }

        };

        /// <summary>Represents a password phrase for aclient certificate.</summary>
        public __gc class SslClientCertificatePasswordCredential
        {
        public:
            String* Password;
        public private:
            svn_auth_cred_ssl_client_cert_pw_t* GetCredential( apr_pool_t* pool )
            {
                svn_auth_cred_ssl_client_cert_pw_t* cred = 
                    static_cast<svn_auth_cred_ssl_client_cert_pw_t*>(
                        apr_pcalloc(pool, sizeof(*cred) ) );
                cred->password = StringHelper( this->Password ).CopyToPool( pool );

                return cred;
            }

        };
    }
}