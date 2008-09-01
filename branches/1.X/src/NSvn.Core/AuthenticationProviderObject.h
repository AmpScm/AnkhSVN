// $Id$

#pragma once

#include "StdAfx.h"
#include "GCPool.h"
#include <svn_auth.h>

namespace NSvn
{
    namespace Core
    {
        /// <summary>
        /// An authentication provider object.
        /// </summary>
        public __gc class AuthenticationProviderObject
        {
        private public:
            /// <summary>Note that provider *must* be allocated on pool</summary>
            AuthenticationProviderObject( svn_auth_provider_object_t* provider, GCPool* pool ) :
                pool(pool), provider(provider)
                {;}

            svn_auth_provider_object_t* GetProvider()
            { return this->provider; }

        private:
            // the managed pool this provider is allocated on
            GCPool* pool;
            svn_auth_provider_object_t* provider;
        };
    }
}