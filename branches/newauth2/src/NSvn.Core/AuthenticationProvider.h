// $Id$

#pragma once

#include "StdAfx.h"
#include "GCPool.h"
#include "svnenums.h"
#include "delegates.h"
#include "credentials.h"
#include <svn_auth.h>

namespace NSvn
{
    namespace Core
    {
        /// <summary>
        /// An authentication provider object.
        /// </summary>
        public __gc class AuthenticationProvider
        {
        public:
            /// <summary>Create and return an authentication provider that gets information by 
            /// prompting the user</summary>
            static AuthenticationProvider* GetSimplePromptProvider( 
                SimplePromptDelegate* promptDelegate,
                int retryLimit );

            /// <summary>Create and return an authentication provider that gets username 
            /// information from the user's ~/.subversion configuration directory </summary>
            static AuthenticationProvider* GetUsernameProvider();

            /// <summary>Create and return an authentication provider that gets username and
            /// password information from the user's ~/.subversion configuration directory </summary>
            static AuthenticationProvider* GetSimpleProvider();

            /// <summary>Create and return an authentication provider that queries the user
            /// whether to trust a specific SSL server certificate</summary>
            static AuthenticationProvider* GetSslServerTrustPromptProvider( 
                SslServerTrustPromptDelegate* trustDelegate );

            /// <summary>Create and return an authentication provider that queries
            /// the user's config area for certificate trust stuff.</summary>
            static AuthenticationProvider* GetSslServerTrustFileProvider();

            /// <summary>Create and return an authentication provider that queries 
            /// the user's config area for client certificates.</summary>
            static AuthenticationProvider* GetSslClientCertFileProvider();

            /// <summary>Create and return an authentication provider that queries 
            /// the user's config area for a password for a client certificate.</summary>
            static AuthenticationProvider* GetSslClientCertPasswordFileProvider();

            /// <summary>Create and return an authentication provider that prompts
            /// the user for a client certificate.</summary>
            static AuthenticationProvider* GetSslClientCertPromptProvider( 
                SslClientCertPromptDelegate* promptDelegate );

            /// <summary>Create and return an authentication provider that prompts
            /// the user for a passphrase for a client certificate.</summary>
            static AuthenticationProvider* GetSslClientCertPasswordPromptProvider( 
                SslClientCertPasswordPromptDelegate* promptDelegate );

        private public:
            /// <summary>Note that provider *must* be allocated on pool</summary>
            AuthenticationProvider( svn_auth_provider_object_t* provider, GCPool* pool ) :
                pool(pool), provider(provider)
                {;}

            svn_auth_provider_object_t* GetProvider()
            { return this->provider; }

        private:
            // the managed pool this provider is allocated on
            GCPool* pool;
            svn_auth_provider_object_t* provider;
        };

         /// <summary>Contains information about a specific certificate.</summary>
        public __gc class SslServerCertificateInfo
        {
        private public:
            SslServerCertificateInfo( const svn_auth_ssl_server_cert_info_t* info ) : 
                HostName(StringHelper(info->hostname)), 
                FingerPrint(StringHelper(info->fingerprint)),
                ValidFrom(StringHelper(info->valid_from)), 
                ValidUntil(StringHelper(info->valid_until)),
                Issuer(StringHelper(info->issuer_dname)), 
                AsciiCertificate(StringHelper(info->ascii_cert))
            {
            }
        public:
            String* const HostName;
            String* const FingerPrint;
            String* const ValidFrom;
            String* const ValidUntil;
            String* const Issuer;
            String* const AsciiCertificate;
        };

       
    }
}