#include "StdAfx.h"
#include "../NSvn.Core/ManagedPointer.h"
#include "managedpointertest.h"
#using <mscorlib.dll>

using namespace System;

namespace
{
    void voidFunc( void* ptr )
    {
        String* string = *(static_cast<NSvn::Core::ManagedPointer<String*>* >( ptr ) );
        Assertion::AssertEquals( string, S"Moo world" );
    }
}

void NSvn::Core::Tests::MCpp::ManagedPointerTest::TestBasic()
{
    String* str = S"Moo world";
    ManagedPointer<String*> ptr( str );

    //check that the implicit conversion works
    Assertion::AssertEquals( ptr, S"Moo world" );

    //implicitly convert to void*
    voidFunc( ptr );
}