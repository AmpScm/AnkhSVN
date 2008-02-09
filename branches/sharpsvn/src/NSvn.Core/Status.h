// $Id$
#pragma once
#include "stdafx.h"
#include "svnenums.h"
#include "Entry.h"
#include "LockToken.h"
#include <svn_wc.h>



namespace NSvn
{
    namespace Core
    {
        /// <summary>Represents an item's status</summary>
        public __gc class Status
        {
        private public:
            Status( svn_wc_status2_t* status, apr_pool_t* pool ) :
            textStatus( static_cast<StatusKind>(status->text_status) ),
            propertyStatus( static_cast<StatusKind>(status->prop_status) ),
            locked( status->locked != 0 ),
            copied( status->copied != 0 ),
            switched( status->switched != 0 ),
            repositoryTextStatus( 
            static_cast<StatusKind>(status->repos_text_status) ),
            repositoryPropertyStatus(
            static_cast<StatusKind>(status->repos_prop_status) )

        {
            if ( status->entry != 0)
                this->entry = new NSvn::Core::Entry( status->entry, pool );
            else
                this->entry = 0;

            if ( status->repos_lock != 0 )
                this->reposLock = new NSvn::Core::LockToken( status->repos_lock, pool );
            else
                this->reposLock = 0;
        }
        Status()                         
        {;}

        static Status()
        {
            unversioned->textStatus = StatusKind::Unversioned;
            unversioned->propertyStatus = StatusKind::Unversioned;
            unversioned->locked = false;
            unversioned->copied = false;
            unversioned->repositoryTextStatus = StatusKind::Unversioned;
            unversioned->repositoryPropertyStatus = StatusKind::Unversioned;
            unversioned->entry = 0;

            none->textStatus = StatusKind::None;
            none->propertyStatus = StatusKind::None;
            none->locked = false;
            none->copied = false;
            none->repositoryTextStatus = StatusKind::None;
            none->repositoryPropertyStatus = StatusKind::None;
            none->entry = 0;                
        }





        public:
            /// <summary></summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property Entry* get_Entry()
            { return this->entry; }

            /// <summary>The status of the file itself</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property StatusKind get_TextStatus()
            { return this->textStatus; }

            /// <summary>The status of the file's properties</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property StatusKind get_PropertyStatus()
            { return this->propertyStatus; }

            /// <summary>The directory is locked(usually happens when 
            /// an update is interrupted)</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property bool get_Locked()
            { return this->locked; }

            /// <summary>The item has been copied</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property bool get_Copied()
            { return this->copied; }

            /// <summary>The item has been switched</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property bool get_Switched()
            { return this->switched; }

            /// <summary>The entry's text status in the repository</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property StatusKind get_RepositoryTextStatus()
            { return this->repositoryTextStatus; }

            /// <summary>The entry's property status in the repository
            [System::Diagnostics::DebuggerStepThrough]
            __property StatusKind get_RepositoryPropertyStatus()
            { return this->repositoryPropertyStatus; }

            [System::Diagnostics::DebuggerStepThrough]
            __property LockToken* get_ReposLock()
            { return this->reposLock; }

            /// <summary>Represents the status of an unversioned item</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property static Status* get_Unversioned()
            { return Status::unversioned; }

            /// <summary>Represents the status of an item with no status at all.</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property static Status* get_None()
            { return Status::none; }

            

            /// <summary>Test whether two Status instances are equal.</summary>
            virtual bool Equals( Object* obj )
            {
                if ( !obj )
                    return false;
                if ( !this->GetType()->IsInstanceOfType( obj ) )
                    return false;
                Status* other = static_cast<Status*>(obj);             

                return this->textStatus == other->textStatus &&
                    this->propertyStatus == other->propertyStatus &&
                    this->repositoryTextStatus == other->repositoryTextStatus &&
                    this->repositoryPropertyStatus == other->repositoryPropertyStatus &&
                    this->locked == other->locked &&
                    this->copied == other->copied &&
                    this->switched == other->switched &&
                    OBJEQUALS(this->entry, other->entry);
            }

        private:
            NSvn::Core::Entry* entry;
            StatusKind textStatus;
            StatusKind propertyStatus;
            bool locked;
            bool copied;
            bool switched;
            StatusKind repositoryTextStatus;
            StatusKind repositoryPropertyStatus;
            NSvn::Core::LockToken* reposLock;

            static Status* unversioned = new Status();
            static Status* none = new Status();
        };
    }
}