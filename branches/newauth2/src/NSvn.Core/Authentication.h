// $Id$
#pragma once

#include "StdAfx.h"
#include "AuthenticationProviderObject.h"
#include "delegates.h"

namespace NSvn
{
    namespace Core
    {
        public __gc class Authentication
        {
        public:
            /// <summary>Create and return an authentication provider that gets information by 
            /// prompting the user</summary>
            static AuthenticationProviderObject* GetSimplePromptProvider( 
                SimplePromptDelegate* promptDelegate,
                int retryLimit );

            /// <summary>Create and return an authentication provider that gets username 
            /// information from the user's ~/.subversion configuration directory </summary>
            static AuthenticationProviderObject* GetUsernameProvider();

            /// <summary>Create and return an authentication provider that gets username and
            /// password information from the user's ~/.subversion configuration directory </summary>
            static AuthenticationProviderObject* GetSimpleProvider();

            /// <summary>Create and return an authentication provider that queries the user
            /// whether to trust a specific SSL server certificate</summary>
            static AuthenticationProviderObject* GetSslServerTrustPromptProvider( 
                SslServerTrustPromptDelegate* trustDelegate );

        private:


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