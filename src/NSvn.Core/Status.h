#pragma once
#include "stdafx.h"
#include "svnenums.h"
#include "Entry.h"
#include <svn_wc.h>

namespace NSvn
{
    namespace Core
    {
        /// <summary>Represents an item's status</summary>
        public __gc class Status
        {
        private public:
            Status( svn_wc_status_t* status ) :
                        textStatus( static_cast<StatusKind>(status->text_status) ),
                        propertyStatus( static_cast<StatusKind>(status->prop_status) ),
                        locked( status->locked != 0 ),
                        copied( status->copied != 0 ),
                        repositoryTextStatus( 
                            static_cast<StatusKind>(status->repos_text_status) ),
                        repositoryPropertyStatus(
                            static_cast<StatusKind>(status->repos_prop_status) )

                        {
                            if ( status->entry != 0)
                                this->entry = new NSvn::Core::Entry(status->entry);
                            else
                                this->entry = 0;
                        }
            Status() :
                            textStatus( StatusKind::Unversioned ),
                            propertyStatus( StatusKind::Unversioned ),
                            locked( false ),
                            copied( false ),
                            repositoryTextStatus( StatusKind::Unversioned ),
                            repositoryPropertyStatus( StatusKind::Unversioned ),
                            entry( 0 )
                        {;}





        public:
            /// <summary></summary>
            __property Entry* get_Entry()
            { return this->entry; }

            /// <summary>The status of the file itself</summary>
            __property StatusKind get_TextStatus()
            { return this->textStatus; }

            /// <summary>The status of the file's properties</summary>
            __property StatusKind get_PropertyStatus()
            { return this->propertyStatus; }

            /// <summary>The directory is locked(usually happens when 
            /// an update is interrupted)</summary>
            __property bool get_Locked()
            { return this->locked; }

            /// <summary>The item has been copied</summary>
            __property bool get_Copied()
            { return this->copied; }

            /// <summary>The item has been switched</summary>
            __property bool get_Switched()
            { return this->switched; }

            /// <summary>The entry's text status in the repository</summary>
            __property StatusKind get_RepositoryTextStatus()
            { return this->repositoryTextStatus; }

            /// <summary>The entry's property status in the repository
            __property StatusKind get_RepositoryPropertyStatus()
            { return this->repositoryPropertyStatus; }

            /// <summary>Represents the status of an unversioned item</summary>
            __property static Status* get_Unversioned()
            { return Status::unversioned; }

        private:
            NSvn::Core::Entry* entry;
            StatusKind textStatus;
            StatusKind propertyStatus;
            bool locked;
            bool copied;
            bool switched;
            StatusKind repositoryTextStatus;
            StatusKind repositoryPropertyStatus;

            static Status* unversioned;
        };
    }
}