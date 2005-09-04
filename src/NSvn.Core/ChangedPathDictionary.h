// $Id$
#pragma once
#include "stdafx.h"
#include "ChangedPath.h"
#include <svn_path.h>
#include <svn_pools.h>
#include <apr_hash.h>

namespace NSvn
{
    namespace Core
    {
    public __gc class ChangedPathDictionary : public System::Collections::DictionaryBase
    {
    public:

        /* __property virtual void set_Item( String* path, Status* status )
        { this->Dictionary->Item[ path ] = status; }*/

        //TODO: straighten out this fucking mess
        virtual ChangedPath* Get( String* path )
        { return static_cast<ChangedPath*>(this->Dictionary->Item[ path ]); }

        [System::Diagnostics::DebuggerStepThrough]
        virtual void Add( String* path, ChangedPath* status )
        { this->Dictionary->Add( path, status ); }

        [System::Diagnostics::DebuggerStepThrough]
        virtual bool Contains( ChangedPath* status )
        { return this->Dictionary->Contains( status ); }

        [System::Diagnostics::DebuggerStepThrough]
        virtual void Remove( String* path )
        { this->Dictionary->Remove( path ); }

        [System::Diagnostics::DebuggerStepThrough]
        __property virtual System::Collections::ICollection* get_Keys()
        { return this->Dictionary->Keys; }

        [System::Diagnostics::DebuggerStepThrough]
        __property virtual System::Collections::ICollection* get_Values()
        { return this->Dictionary->Values; }

    public private:
        /// <summary>Creates a StatusDictionary from an apr_hash_t</summary>
        static ChangedPathDictionary* FromChangedPathsHash( apr_hash_t* changedPaths, 
            apr_pool_t* pool )
        {
            ChangedPathDictionary* dict = new ChangedPathDictionary();

            apr_hash_index_t* idx = apr_hash_first( pool, changedPaths );
            while( idx != 0 )
            {
                const char* path;
                apr_ssize_t keyLength;
                svn_log_changed_path_t* changedPath;

                apr_hash_this( idx, reinterpret_cast<const void**>(&path), &keyLength,
                    reinterpret_cast<void**>(&changedPath) );
                String* managedPath = Utf8ToString( 
                    svn_path_local_style(path, pool), pool );
                dict->Add( managedPath, new ChangedPath(changedPath, pool) );

                idx = apr_hash_next( idx );
            }

            return dict;
        }
    };
    }
}