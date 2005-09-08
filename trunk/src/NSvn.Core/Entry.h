// $Id$
#pragma once
#include "stdafx.h"
#include <svn_wc.h>
#include "utils.h"


#define OBJEQUALS( obj1, obj2 ) ((((obj1) == 0 && (obj2) == 0)) || \
                                      (((obj1) !=0) && ((obj2) != 0) && ((obj1)->Equals(obj2))))

namespace NSvn
{
    namespace Core
    {
        /// Represents a working copy entry</summary>
        public __gc class Entry
        {
        private public:
            Entry( svn_wc_entry_t* entry, apr_pool_t* pool ) : 
        name( Utf8ToString(entry->name, pool) ),
            revision( entry->revision ),
            url( Utf8ToString(entry->url, pool) ),
            repository( Utf8ToString(entry->repos, pool) ),
            uuid( Utf8ToString(entry->uuid, pool) ),
            kind( static_cast<NodeKind>(entry->kind) ),
            schedule( static_cast<NSvn::Core::Schedule>(entry->schedule) ),
            copied( entry->copied != 0 ),
            deleted( entry->deleted != 0 ),
            copyFromUrl( Utf8ToString(entry->copyfrom_url, pool) ),
            copyFromRevision( entry->copyfrom_rev ),
            conflictOld( Utf8ToString(entry->conflict_old, pool) ),
            conflictNew( Utf8ToString(entry->conflict_new, pool) ),
            conflictWorking( Utf8ToString(entry->conflict_wrk, pool) ),
            propertyRejectFile( Utf8ToString(entry->prejfile, pool) ),
            textTime( AprTimeToDateTime(entry->text_time) ),
            propertyTime( AprTimeToDateTime(entry->prop_time) ),
            checkSum( Utf8ToString(entry->checksum, pool) ),
            commitRevision( entry->cmt_rev ),
            commitDate( AprTimeToDateTime(entry->cmt_date) ),
            commitAuthor( Utf8ToString(entry->cmt_author, pool) ),
            lockToken( Utf8ToString(entry->lock_token, pool) ),
            lockOwner( Utf8ToString(entry->lock_owner, pool) ),
            lockComment( Utf8ToString(entry->lock_comment, pool) ),
            lockCreationDate( AprTimeToDateTime(entry->lock_creation_date) )

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

            /// <summary>The lock token.</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_LockToken()
            { return this->lockToken; }

            /// <summary>The owner of the lock.</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_LockOwner()
            { return this->lockOwner; }

            /// <summary>The comment associated with the lock</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_LockComment()
            { return this->lockComment; }

            /// <summary>The creation time of the lock</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property DateTime get_LockCreationDate()
            { return this->lockCreationDate; }

            /// <summary>Test whether two Entry instances are equal.</summary>
            virtual bool Equals( Object* obj )
            {
                if ( !obj )
                    return false;

                if ( !this->GetType()->IsInstanceOfType( obj ) )
                    return false;
                Entry* other = static_cast<Entry*>(obj);

                return OBJEQUALS(this->name, other->name) &&
                    this->revision == other->revision &&
                    OBJEQUALS(this->url, other->url) &&
                    OBJEQUALS(this->repository, other->repository) &&
                    OBJEQUALS(this->uuid, other->uuid) &&
                    this->kind == other->kind &&
                    this->schedule == other->schedule &&
                    this->copied == other->copied &&
                    this->deleted == other->deleted &&
                    OBJEQUALS(this->copyFromUrl, other->copyFromUrl) &&
                    this->copyFromRevision == other->copyFromRevision &&
                    OBJEQUALS(this->conflictNew, other->conflictNew) &&
                    OBJEQUALS(this->conflictOld, other->conflictOld ) &&
                    OBJEQUALS(this->conflictWorking, other->conflictWorking) &&
                    OBJEQUALS(this->propertyRejectFile, other->propertyRejectFile) &&
                    this->textTime.Equals( __box(other->textTime) ) &&
                    this->propertyTime.Equals( __box(other->propertyTime) ) &&
                    OBJEQUALS(this->checkSum, other->checkSum) &&
                    this->commitRevision == other->commitRevision &&
                    this->commitDate.Equals( __box(other->commitDate) ) &&
                    OBJEQUALS(this->commitAuthor, other->commitAuthor ) &&
                    OBJEQUALS(this->lockToken, other->lockToken) &&
                    OBJEQUALS(this->lockComment, other->lockComment) &&
                    OBJEQUALS(this->lockOwner, other->lockOwner) &&
                    this->lockCreationDate.Equals( __box(other->lockCreationDate) );
            }

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
            String* lockToken;
            String* lockOwner;
            String* lockComment;
            DateTime lockCreationDate; 
        };
    }
}