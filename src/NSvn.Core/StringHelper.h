// $Id$
#pragma once
#include <vcclr.h>
#include <svn_utf.h>
#include "SvnClientException.h"
#include "Pool.h"
#include <apr_strings.h>

namespace NSvn
{
    namespace Core
    {
        using namespace System;
        using namespace System::Runtime::InteropServices;

        // Helper class to effect string conversions
        class StringHelper
        {
        public:
            // <summary>constructor that takes a System::String</summary>
            StringHelper( String __gc* string )
            {
                this->string = string;
                this->charPtr = 0;
            }

            /// <summary>constructor that takes a const char*</summary>
            StringHelper( const char* charPtr )
            {
                this->string = this->ConvertToSystemString( charPtr );
                this->charPtr = 0;
            }

            /// <summary>implicit conversion to a System::String</summary>
            operator String*() const
            {
                return this->string;
            }

            /// <summary>Copy constructor</summary>
            StringHelper( const StringHelper& other )
            {
                //System::Strings can be shared
                this->string = other.string;

                //char ptrs can not
                this->charPtr = 0;
            }

            /// <summary>Assignment operator</summary>
            StringHelper& operator=( const StringHelper& other )
            {
                if ( this == &other )
                    return *this;

                this->string = other.string;
                if( this->charPtr != 0 )
                    this->FreeCharPtr( this->charPtr );

                this->charPtr = 0;

                return *this;
            }

            /// <summary>Copies the string to the pool, encoded as UTF8</summary>
            char* CopyToPoolUtf8( apr_pool_t* pool )
            {
                const char* utf8String;
                HandleError( svn_utf_cstring_to_utf8( &utf8String, this->CopyToPool( pool ), 
                    0, pool ) );

                return const_cast<char*>(utf8String);
            }
            
            char* CopyToPool( apr_pool_t* pool )
            {
               //TODO: unicode issues

                char* hglobal = this->ConvertToCharPtr( this->string );

                //make room in the pool and copy our string there
                char* ptr = apr_pstrdup( pool, hglobal );

                // free the hglobal
                this->FreeCharPtr( hglobal );

                return ptr;
            }
                
                

            /// <summary>implicit conversion to a const char*</summary>
            operator const char* () const
            {
                // lazy creation of the char* - we might not need it
                if ( this->charPtr == 0 )
                    this->charPtr = this->ConvertToCharPtr( this->string );

                return this->charPtr;
            }

            // <summary>dtor - get rid of the char ptr</summary>
            ~StringHelper()
            {
                if ( this->charPtr != 0 )
                    this->FreeCharPtr( this->charPtr );
            }

        private:
            /// <summary>convert a char ptr to a System::String</summary>
            String __gc* ConvertToSystemString( const char* ptr )
            {
                return Marshal::PtrToStringAnsi( static_cast<IntPtr>(const_cast<char*>(ptr) ) );
            }

            /// <summary>convert a System::String to a char*</summary>
            char* ConvertToCharPtr( String __gc* string ) const
            {
                return static_cast<char*>( Marshal::StringToHGlobalAnsi( string ).ToPointer() );
            }

            /// <summary>free the memory allocated by the StringToHGlobalAnsi call</summary>
            void FreeCharPtr( char* ptr )
            {
                Marshal::FreeHGlobal( static_cast<IntPtr>( static_cast<void*>(ptr) ) );
            }

            gcroot<String*> string;
            mutable char* charPtr;
        };
    }
}
