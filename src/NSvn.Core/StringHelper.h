#include "Stdafx.h"
#include <vcclr.h>
using namespace System;
using namespace System::Runtime::InteropServices;

namespace NSvn
{
    namespace Core
    {
        // Helper class to effect string conversions
        class StringHelper
        {
        public:
            // constructor that takes a System::String
            StringHelper( String __gc* string )
            {
                this->string = string;
                this->charPtr = 0;
            }

            // constructor that takes a const char*
            StringHelper( const char* charPtr )
            {
                this->string = this->ConvertToSystemString( charPtr );
                this->charPtr = 0;
            }

            // implicit conversion to a System::String
            operator String*() const
            {
                return this->string;
            }

            // implicit conversion to a const char*
            operator const char* () const
            {
                // lazy creation of the char* - we might not need it
                if ( this->charPtr == 0 )
                    this->charPtr = this->ConvertToCharPtr( this->string );

                return this->charPtr;
            }

            // dtor - get rid of the char ptr
            ~StringHelper()
            {
                if ( this->charPtr != 0 )
                    this->FreeCharPtr( this->charPtr );
            }

        private:
            // convert a char ptr to a System::String
            String __gc* ConvertToSystemString( const char* ptr )
            {
                return Marshal::PtrToStringAnsi( static_cast<IntPtr>(const_cast<char*>(ptr) ) );
            }

            // convert a System::String to a char*
            char* ConvertToCharPtr( String __gc* string ) const
            {
                return static_cast<char*>( Marshal::StringToHGlobalAnsi( string ).ToPointer() );
            }

            // free the memory allocated by the StringToHGlobalAnsi call
            void FreeCharPtr( char* ptr )
            {
                Marshal::FreeHGlobal( static_cast<IntPtr>( static_cast<void*>(ptr) ) );
            }

            gcroot<String*> string;
            mutable char* charPtr;
        };
    }
}