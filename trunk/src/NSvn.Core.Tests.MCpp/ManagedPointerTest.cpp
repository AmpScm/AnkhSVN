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

void NSvn::Core::Tests::MCpp::ManagedPointerTest::TestAssignment()
{
    String* str = S"Moo world";
    ManagedPointer<String*> ptr1( str );
    ManagedPointer<String*> ptr2( S"Bleh" );

    ptr2 = ptr1;
    Assertion::AssertEquals( ptr1, ptr2 );
    Assertion::AssertEquals( ptr2, S"Moo world" );
}

void NSvn::Core::Tests::MCpp::ManagedPointerTest::TestCopying()
{
    String* str = S"Moo world";
    ManagedPointer<String*> ptr1( str );
    ManagedPointer<String*> ptr2( ptr1 );

    Assertion::AssertEquals( ptr1, ptr2 );
}