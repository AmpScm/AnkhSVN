// $Id$
#pragma once
#include "stdafx.h"
#include "StringHelper.h"
#include <svn_wc.h>
#include "utils.h"

namespace NSvn
{
    namespace Core
    {
        /// Represents a working copy entry</summary>
        public __gc class Entry
        {
        private public:
            Entry( svn_wc_entry_t* entry ) : 
        name( StringHelper(entry->name) ),
            revision( entry->revision ),
            url( StringHelper(entry->url) ),
            repository( StringHelper(entry->repos) ),
            uuid( StringHelper(entry->uuid) ),
            kind( static_cast<NodeKind>(entry->kind) ),
            schedule( static_cast<NSvn::Core::Schedule>(entry->schedule) ),
            copied( entry->copied != 0 ),
            deleted( entry->deleted != 0 ),
            copyFromUrl( StringHelper(entry->copyfrom_url) ),
            copyFromRevision( entry->copyfrom_rev ),
            conflictOld( StringHelper(entry->conflict_old) ),
            conflictNew( StringHelper(entry->conflict_new) ),
            conflictWorking( StringHelper(entry->conflict_wrk) ),
            propertyRejectFile( StringHelper(entry->prejfile) ),
            textTime( AprTimeToDateTime(entry->text_time) ),
            propertyTime( AprTimeToDateTime(entry->prop_time) ),
            checkSum( StringHelper(entry->checksum) ),
            commitRevision( entry->cmt_rev ),
            commitDate( AprTimeToDateTime(entry->cmt_date) ),
            commitAuthor( StringHelper(entry->cmt_author) )

        {;}


        public:
            /// <summary>The entry's name</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_Name()
            { return this->name; }

            /// <summary>The item's base revision</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property svn_revnum_t get_Revision()
            { return this->revision; }

            /// <summary>The url to the item in the repository</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_Url()
            { return this->url; }

            /// <summary>The canonical repository url</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_Repository()
            { return this->repository; }

            /// <summary>The repository UUID</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_Uuid()
            { return this->uuid; }

            /// <summary>The type of node(file or directory)</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property NodeKind get_Kind()
            { return this->kind; }

            /// <summary>The scheduled state of the item</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property Schedule get_Schedule()
            { return this->schedule; }

            /// <summary>Whether the item is in a copied state</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property bool get_Copied()
            { return this->copied; }

            /// <summary>Deleted, but parent revision lags behind</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property bool get_Deleted()
            { return this->deleted; }

            /// <summary>The copy-from location</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_CopyFromUrl()
            { return this->copyFromUrl; }

            /// <summary>The copy-from revision</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property svn_revnum_t get_CopyFromRevision()
            { return this->copyFromRevision; }

            /// <summary>The old version of a conflicted file</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_ConflictOld()
            { return this->conflictOld; }

            /// <summary>The new version of a conflicted file</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_ConflictNew()
            { return this->conflictNew; }

            /// <summary>The working copy version of a conflicted file</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_ConflictWorking()
            { return this->conflictWorking; }

            /// <summary>The property reject file</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_PropertyRejectFile()
            { return this->propertyRejectFile; }

            /// <summary>The last up-to-date time for text contents</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property DateTime get_TextTime()
            { return this->textTime; }

            /// <summary>The last up-to-date time for property contents</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property DateTime get_PropertyTime()
            { return this->propertyTime; }

            /// <summary>Base64 encoded checksum for the text base file</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_CheckSum()
            { return this->checkSum; }

            /// <summary>The last revision this was changed</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property svn_revnum_t get_CommitRevision()
            { return this->commitRevision; }

            /// <summary>The last time this was changed</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property DateTime get_CommitDate()
            { return this->commitDate; }

            /// <summary>The author of the last change</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_CommitAuthor()
            { return this->commitAuthor; }

        private:
            String* name;
            svn_revnum_t revision;
            String* url;
            String* repository;
            String* uuid;
            NodeKind kind;
            NSvn::Core::Schedule schedule;
            bool copied;
            bool deleted;
            String* copyFromUrl;
            svn_revnum_t copyFromRevision;
            String* conflictOld;
            String* conflictNew;
            String* conflictWorking;
            String* propertyRejectFile;
            DateTime textTime;
            DateTime propertyTime;
            String* checkSum;
            svn_revnum_t commitRevision;
            DateTime commitDate;
            String* commitAuthor;
        };
    }
}