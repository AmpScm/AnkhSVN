#pragma once
#include "stdafx.h"
#include "svnenums.h"
#include "utils.h"

namespace NSvn
{
    namespace Core
    {
        /// <summary>Represents a directory entry</summary>
        public __gc class DirectoryEntry
        {
        public:
            DirectoryEntry( const char* path, svn_dirent* dirent ) : 
              path( StringHelper( path ) ),
              nodeKind( static_cast<NSvn::Core::NodeKind>(dirent->kind) ),
              size( dirent->size ),
              hasProps( dirent->has_props != 0 ),
              createdRev( dirent->created_rev ),
              lastAuthor( StringHelper(dirent->last_author) )
            {
                this->time = AprTimeToDateTime( dirent->time );
            }

            ///<summary>The path to this entry</summary>
            __property String* get_Path()
            { return this->path; }

            ///<summary>The kind of node represented by this entry</summary>
            __property NodeKind get_NodeKind()
            { return this->nodeKind; }

            ///<summary>The size of the file, 0 for a directory</summary>
            __property __int64 get_Size()
            { return this->size; }

            ///<summary>Whether the node has properties</summary>
            __property bool get_HasProperties()
            { return this->hasProps; }

            ///<summary>The last revision in which this node changed</summary>
            __property svn_revnum_t get_CreatedRevision()
            { return this->createdRev; }

            ///<summary>The time of the last modification</summary>
            __property DateTime get_Time()
            { return this->time; }

            ///<summary>The author responsible for the last change</summary>
            __property String* get_LastAuthor()
            { return this->lastAuthor; }

        private:  
            String* path;
            NSvn::Core::NodeKind nodeKind;
            __int64 size;
            bool hasProps;
            svn_revnum_t createdRev;
            DateTime time;
            String* lastAuthor;
        };
    }
}