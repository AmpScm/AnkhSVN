#include "StdAfx.h"
#include <svn_client.h>
#include "svnenums.h"

namespace NSvn
{
    namespace Core
    {
        /// <summary>Represents a commit candidate structure</summary>
        public __gc class CommitItem
        {
        public:
            CommitItem( svn_client_commit_item_t* item ) : 
                path( StringHelper( item->path ) ), 
                kind( static_cast<NodeKind>(item->kind) ),
                url( StringHelper( item->url ) ),
                revision( item->revision ),
                copyFromUrl( StringHelper( item->copyfrom_url ) )

                {;}
                
            ///<summary>The working copy path to this item</summary>
            __property String* get_Path()
            { return this->path; }

            /// <summary>The kind of node - file or dir</summary>
            __property NodeKind get_Kind()
            { return this->kind; }

            /// <summary>The repository URL to this item</summary>
            __property String* get_Url()
            { return this->url; }

            /// <summary>The revision number associated with this commit</summary>
            __property int get_Revision()
            { return this->revision; }

            /// <summary>The copy from URL</summary>
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