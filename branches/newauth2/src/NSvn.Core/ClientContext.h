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


        // .NET representation of the svn_client_ctx_t structure
        public __gc class ClientContext
        {
        public:
            ClientContext() : notifyCallback( 0 ), authBaton( 0 ), clientConfig( 0 ),
                logMessageCallback( 0 ), promptCallback( 0 )
            {;}


            ClientContext( NSvn::Core::NotifyCallback* callback ) : notifyCallback( callback ), 
                authBaton( 0 ), clientConfig( 0 ),
                logMessageCallback( 0 ), promptCallback( 0 )
            {;}

            ClientContext( NSvn::Core::NotifyCallback* callback, 
                NSvn::Core::AuthenticationBaton* authBaton ) :
            notifyCallback( callback ), authBaton( authBaton ), clientConfig( 0 ),
                logMessageCallback( 0 ), promptCallback( 0 )
            {;}

            ClientContext( NotifyCallback* callback, AuthenticationBaton* authBaton, 
                ClientConfig* config ) :
            notifyCallback( callback ), authBaton( authBaton ), clientConfig( config ),
                logMessageCallback( 0 ), promptCallback( 0 )
            {;}

            // An authentication baton
            [System::Diagnostics::DebuggerStepThrough]
            __property NSvn::Core::AuthenticationBaton* get_AuthBaton()
            { return this->authBaton; }

            [System::Diagnostics::DebuggerStepThrough]
            __property void set_AuthBaton( NSvn::Core::AuthenticationBaton* value )
            { this->authBaton = value; }

            // A callback delegate for prompts
            [System::Diagnostics::DebuggerStepThrough]
            __property PromptCallback* get_PromptCallback()
            { return this->promptCallback; }

            [System::Diagnostics::DebuggerStepThrough]
            __property void set_PromptCallback( NSvn::Core::PromptCallback* value )
            { this->promptCallback = value; }

            // Callback delegate for notifications
            [System::Diagnostics::DebuggerStepThrough]
            __property NotifyCallback* get_NotifyCallback()
            { return this->notifyCallback; }

            [System::Diagnostics::DebuggerStepThrough]
            __property void set_NotifyCallback( NSvn::Core::NotifyCallback* value )
            { this->notifyCallback = value; }

            // Callback delegate for log messages
            [System::Diagnostics::DebuggerStepThrough]
            __property LogMessageCallback* get_LogMessageCallback()
            { return this->logMessageCallback; }
            
            [System::Diagnostics::DebuggerStepThrough]
            __property void set_LogMessageCallback( NSvn::Core::LogMessageCallback* value )
            { this->logMessageCallback = value; }

            // The client configuration
            [System::Diagnostics::DebuggerStepThrough]
            __property ClientConfig* get_ClientConfig()
            { return this->clientConfig; }

            [System::Diagnostics::DebuggerStepThrough]
            __property void set_ClientConfig( NSvn::Core::ClientConfig* value )
            { this->clientConfig = value; }

        private public:
            svn_client_ctx_t* ToSvnContext( const Pool& pool );

        private:

            NSvn::Core::AuthenticationBaton* authBaton;
            NSvn::Core::PromptCallback* promptCallback;
            NSvn::Core::NotifyCallback* notifyCallback;
            NSvn::Core::LogMessageCallback* logMessageCallback;
            NSvn::Core::ClientConfig* clientConfig;


        };
    }
}



