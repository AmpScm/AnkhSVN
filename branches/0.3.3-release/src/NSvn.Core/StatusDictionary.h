// $Id$
#pragma once
#include "stdafx.h"
#include <svn_path.h>

namespace NSvn
{
    namespace Core
    {
    public __gc class StatusDictionary : public System::Collections::DictionaryBase
    {
    public:

        /* __property virtual void set_Item( String* path, Status* status )
        { this->Dictionary->Item[ path ] = status; }*/

        //TODO: straighten out this fucking mess
        [System::Diagnostics::DebuggerStepThrough]
        virtual Status* Get( String* path )
        { return static_cast<Status*>(this->Dictionary->Item[ path ]); }

        virtual Status* GetFirst( )
        {
            System::Collections::IEnumerator* en = this->Keys->GetEnumerator();
            en->MoveNext();
            return this->Get( static_cast<String*>(en->Current) );
        }



        [System::Diagnostics::DebuggerStepThrough]
        virtual void Add( String* path, Status* status )
        { this->Dictionary->Add( path, status ); }

        [System::Diagnostics::DebuggerStepThrough]
        virtual bool Contains( Status* status )
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
        static StatusDictionary* FromStatusHash( apr_hash_t* statushash, Pool& pool )
        {
            StatusDictionary* dict = new StatusDictionary();
            apr_hash_index_t* idx = apr_hash_first( pool, statushash );
            while( idx != 0 )
            {
                const char* path;
                apr_ssize_t keyLength;
                svn_wc_status_t* status;

                apr_hash_this( idx, reinterpret_cast<const void**>(&path), &keyLength,
                    reinterpret_cast<void**>(&status) );
                String* managedPath = StringHelper( 
                    svn_path_local_style(path, pool) );
                dict->Add( managedPath, new Status(status) );

                idx = apr_hash_next( idx );
            }

            return dict;
        }
    };
    }
}