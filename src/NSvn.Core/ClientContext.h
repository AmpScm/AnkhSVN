#using <mscorlib.dll>

#include "delegates.h"
#include "AuthenticationBaton.h"
#include "AdminAccessBaton.h"
#include "ClientConfig.h"
#include <svn_client.h>

namespace NSvn
{
    namespace Core
    {
        // .NET representation of the svn_client_ctx_t structure
        public __gc class ClientContext
        {
            ClientContext( NotifyCallback* callback ) : notifyCallback( callback )
            {;}

            ClientContext( NotifyCallback* callback, AuthenticationBaton* authBaton ) :
                notifyCallback( callback ), authBaton( authBaton )
                {;}

            ClientContext( NotifyCallback* callback, AuthenticationBaton* authBaton, 
                ClientConfig* config ) :
                notifyCallback( callback ), authBaton( authBaton ), clientConfig( config ) 
                {;}

            // An authentication baton
            __property AuthenticationBaton* get_AuthBaton()
            { return this->authBaton; }
            __property void set_AuthBaton( AuthenticationBaton* value )
            { this->authBaton = value; }

            // A callback delegate for prompts
            __property PromptCallback* get_PromptCallback()
            { return this->promptCallback; }
            __property void set_PromptCallback( NSvn::Core::PromptCallback* value )
            { this->promptCallback = value; }

            // Callback delegate for notifications
            __property NotifyCallback* get_NotifyCallback()
            { return this->notifyCallback; }
            __property void set_NotifyCallback( NSvn::Core::NotifyCallback* value )
            { this->notifyCallback = value; }

            // Callback delegate for log messages
            __property LogMessageCallback* get_LogMessageCallback()
            { return this->logMessageCallback; }
            __property void set_LogMessageCallback( NSvn::Core::LogMessageCallback* value )
            { this->logMessageCallback = value; }

            // The client configuration
            __property ClientConfig* get_ClientConfig()
            { return this->clientConfig; }
            __property void set_ClientConfig( NSvn::Core::ClientConfig* value )
            { this->clientConfig = value; }

            svn_client_ctx_t* ToSvnContext( Pool& pool );

        private:
            NSvn::Core::AuthenticationBaton* authBaton;
            NSvn::Core::PromptCallback* promptCallback;
            NSvn::Core::NotifyCallback* notifyCallback;
            NSvn::Core::LogMessageCallback* logMessageCallback;
            NSvn::Core::ClientConfig* clientConfig;
        };
    }
}