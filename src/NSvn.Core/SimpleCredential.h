#include "stdafx.h"
#include <svn_auth.h>
#include <apr_strings.h>
#include "StringHelper.h"

namespace NSvn
{
    namespace Core
    {
        using namespace NSvn::Common;
        using namespace System;

        public __gc class SimpleCredential : public ICredential
        {
        public:
            SimpleCredential( String* username, String* password ) : 
              username( username ), password( password )
              {;}

            __property String* get_Kind()
            {
                return StringHelper( SVN_AUTH_CRED_USERNAME ); 
            }

            /// <summary>Convert to an svn_auth_cred_simple_t*</summary>
            void* GetCredential( void* p )
            {
                apr_pool_t* pool = static_cast<apr_pool_t*>(p);
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
                

    }
}
