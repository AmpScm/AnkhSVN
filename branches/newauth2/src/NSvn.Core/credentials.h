// $Id$

#pragma once
#include "StdAfx.h"
#include <svn_auth.h>
#include "StringHelper.h"
#include <apr_strings.h>

namespace NSvn
{
    namespace Core
    {
        using namespace System;

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
    }
}