// $Id$
#include "StdAfx.h"
#include <svn_client.h>
#include <svn_path.h>
#include <svn_pools.h>
#include "svnenums.h"

namespace NSvn
{
    namespace Core
    {
        /// <summary>Represents a commit candidate structure</summary>
        public __gc class CommitItem
        {
        public:
            CommitItem( svn_client_commit_item_t* item, apr_pool_t* pool ) :                 
              kind( static_cast<NodeKind>(item->kind) ),
                  url( Utf8ToString( item->url, pool ) ),
                  revision( item->revision ),
                  copyFromUrl( Utf8ToString( item->copyfrom_url, pool ) )

              {
                  // convert to a native path
                  if ( item->path )
                    this->path = Utf8ToString( svn_path_local_style(item->path, pool), pool );
                  else 
                      this->path = 0;
              }

              ///<summary>The working copy path to this item</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property String* get_Path()
              { return this->path; }

              /// <summary>The kind of node - file or dir</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property NodeKind get_Kind()
              { return this->kind; }

              /// <summary>The repository URL to this item</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property String* get_Url()
              { return this->url; }

              /// <summary>The revision number associated with this commit</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property int get_Revision()
              { return this->revision; }

              /// <summary>The copy from URL</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property String* get_CopyFromUrl()
              { return this->copyFromUrl; }

        private:
            String* path;
            NodeKind kind;
            String* url;
            int revision;
            unsigned char stateFlags;
            String* copyFromUrl;
            //TODO: wcprop_changes
        };
    }
}