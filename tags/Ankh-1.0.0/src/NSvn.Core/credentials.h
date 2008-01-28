// $Id$

#pragma once
#include "StdAfx.h"
#include <svn_auth.h>
#include <apr_strings.h>
#include "svnenums.h"
#include "utils.h"

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
            SimpleCredential( String* username, String* password, bool maySave ) :
                username(username), password(password), maySave(maySave)
            {;}

            __property String* get_Username()
            { return this->username; }

            __property void set_Username( String* username )
            { this->username = username; }

            __property String* get_Password()
            { return this->password; }

            __property void set_Password( String* password )
            { this->password = password; }

            __property bool get_MaySave()
            { return this->maySave; }

            __property void set_MaySave( bool maySave )
            { this->maySave = maySave; }

        public private:
            /// <summary>Convert to an svn_auth_cred_simple_t*</summary>
          svn_auth_cred_simple_t* GetCredential( apr_pool_t* pool )
          {
              svn_auth_cred_simple_t* cred = static_cast<svn_auth_cred_simple_t*>( 
                  apr_palloc( pool, sizeof(*cred) ) );
              cred->username = StringToUtf8( username, pool );
              cred->password = StringToUtf8( password, pool );
              cred->may_save = this->maySave;
              return cred;
          }
        private:
            String* username;
            String* password;
            bool maySave;
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
            bool MaySave;
        public private:
            svn_auth_cred_ssl_client_cert_t* GetCredential( apr_pool_t* pool )
            {
                svn_auth_cred_ssl_client_cert_t* cred = 
                    static_cast<svn_auth_cred_ssl_client_cert_t*>(
                        apr_pcalloc(pool, sizeof(*cred) ) );
				cred->cert_file = StringToUtf8( this->CertificateFile, pool );
                cred->may_save = this->MaySave;

                return cred;
            }

        };

        /// <summary>Represents a password phrase for a client certificate.</summary>
        public __gc class SslClientCertificatePasswordCredential
        {
        public:
            String* Password;
            bool MaySave;
        public private:
            svn_auth_cred_ssl_client_cert_pw_t* GetCredential( apr_pool_t* pool )
            {
                svn_auth_cred_ssl_client_cert_pw_t* cred = 
                    static_cast<svn_auth_cred_ssl_client_cert_pw_t*>(
                        apr_pcalloc(pool, sizeof(*cred) ) );
				cred->password = StringToUtf8( this->Password, pool );
                cred->may_save = this->MaySave;

                return cred;
            }

        };
    }
}