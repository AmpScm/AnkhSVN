// $Id$
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
        private public:
            DirectoryEntry( const char* path, svn_dirent_t* dirent, apr_pool_t* pool ) : 
              path( Utf8ToString( path , pool )),
                  nodeKind( static_cast<NSvn::Core::NodeKind>(dirent->kind) ),
                  size( dirent->size ),
                  hasProps( dirent->has_props != 0 ),
                  createdRev( dirent->created_rev ),
                  lastAuthor( Utf8ToString(dirent->last_author, pool) )
              {
                  this->time = AprTimeToDateTime( dirent->time );
              }
        public:
              ///<summary>The path to this entry</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property String* get_Path()
              { return this->path; }

              ///<summary>The kind of node represented by this entry</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property NodeKind get_NodeKind()
              { return this->nodeKind; }

              ///<summary>The size of the file, 0 for a directory</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property __int64 get_Size()
              { return this->size; }

              ///<summary>Whether the node has properties</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property bool get_HasProperties()
              { return this->hasProps; }

              ///<summary>The last revision in which this node changed</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property svn_revnum_t get_CreatedRevision()
              { return this->createdRev; }

              ///<summary>The time of the last modification</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property DateTime get_Time()
              { return this->time; }

              ///<summary>The author responsible for the last change</summary>
              [System::Diagnostics::DebuggerStepThrough]
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