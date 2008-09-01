// $Id$
#pragma once
#using <mscorlib.dll>

#include "delegates.h"
#include "AuthenticationBaton.h"
#include "AdminAccessBaton.h"
#include "ClientConfig.h"
#include "ManagedPointer.h"
#include "Pool.h"
#include <svn_client.h>
#include <svn_auth.h>
#include <apr_pools.h>
#include <apr_tables.h>


namespace NSvn
{
    namespace Core
    {
        public __gc class Client;


        // .NET representation of the svn_client_ctx_t structure
        public __gc class ClientContext
        {
        private public:

            ClientContext( Client* client ) : client(client), context(0)
            {;}

            ClientContext( Client* client, AuthenticationBaton* baton, 
                ClientConfig* config ) : client(client), authBaton(baton), clientConfig(config),
                                         context(0)
            {;}
            
        public:
            // An authentication baton
            [System::Diagnostics::DebuggerStepThrough]
            __property NSvn::Core::AuthenticationBaton* get_AuthBaton()
            { return this->authBaton; }

            [System::Diagnostics::DebuggerStepThrough]
            __property void set_AuthBaton( NSvn::Core::AuthenticationBaton* value )
            { this->authBaton = value; }

            // The client configuration
            [System::Diagnostics::DebuggerStepThrough]
            __property ClientConfig* get_ClientConfig()
            { return this->clientConfig; }

            [System::Diagnostics::DebuggerStepThrough]
            __property void set_ClientConfig( NSvn::Core::ClientConfig* value )
            { this->clientConfig = value; }

        private public:
            svn_client_ctx_t* ToSvnContext();

        private:
            Client* client;

            svn_client_ctx_t* context;
            NSvn::Core::AuthenticationBaton* authBaton;
            NSvn::Core::ClientConfig* clientConfig;


        };
    }
}



