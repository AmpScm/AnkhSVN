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
        };
    }
}