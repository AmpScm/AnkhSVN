#include "stdafx.h"
#include <apr_pools.h>
#include <apr_strings.h>
#include <svn_auth.h>
#include "StringHelper.h"

namespace NSvn
{
    namespace Core
    {
        using namespace System;

        ///<summary>Represents a credential that has just a username</summary>
        public __gc class UsernameCredential : public NSvn::Common::ICredential
        {
        public:
            UsernameCredential( String* username ) : username( username )
            {;}

            /// <summary>The type of credential represented by this class </summary>
            __property String* get_Kind()
            { return SVN_AUTH_CRED_USERNAME; }

            ///<summary>Convert to an svn_auth_cred_username_t*</summary>
            IntPtr GetCredential( IntPtr pool )
            {
                apr_pool_t* aprPool = static_cast<apr_pool_t*>( pool.ToPointer() );

                svn_auth_cred_username_t* cred = static_cast<svn_auth_cred_username_t*>(
                    apr_palloc( aprPool, sizeof(*cred) ) );
                cred->username = apr_pstrdup( aprPool, StringHelper( this->username ) );

                return cred;
            }

            /// <summary>Represents a credential that holds the username of the currently
            /// logged in user</summary>
            static UsernameCredential* const LoggedInUser = new UsernameCredential( Environment::UserName );
            
        private:
            String* username;
        };

    }
}