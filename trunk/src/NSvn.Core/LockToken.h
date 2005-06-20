#pragma once
#include "stdafx.h"
#include <svn_types.h>

namespace NSvn
{
    namespace Core
    {
        using namespace System;
        public __gc class LockToken
        {
        public:
            LockToken( svn_lock_t* lock ) :
                path( StringHelper(lock->path) ),
                token( StringHelper(lock->token) ),
                owner( StringHelper(lock->owner) ),
                comment( StringHelper(lock->comment) ),
                isDavComment( lock->is_dav_comment != 0 ),
                creationDate( AprTimeToDateTime(lock->creation_date) ),
                expirationDate( AprTimeToDateTime(lock->expiration_date) )
                {
                    // empty 
                }


            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_Path()
            { return this->path; }

            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_Token()
            { return this->token; }

            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_Owner()
            { return this->owner; }

            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_Comment()
            { return this->comment; }

            [System::Diagnostics::DebuggerStepThrough]
            __property bool get_IsDavComment()
            { return this->isDavComment; }

            [System::Diagnostics::DebuggerStepThrough]
            __property DateTime get_CreationDate()
            { return this->creationDate; }

            [System::Diagnostics::DebuggerStepThrough]
            __property DateTime get_ExpirationDate()
            { return this->expirationDate; }

        private:
            String* path;
            String* token;
            String* owner;
            String* comment;
            bool isDavComment;
            DateTime creationDate;
            DateTime expirationDate;
        };
    }
}