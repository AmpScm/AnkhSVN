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

        };
    }
}