#include "StdAfx.h"
#include "../NSvn.Core/StringHelper.h"
#include "stringhelpertest.h"
#include <cstring>
#using <mscorlib.dll>

namespace NSvn
{
namespace Core
{
namespace Tests
{
namespace MCpp
{
    void StringHelperTest::TestFromConstChar()
    {
        char constChar[] = "Hello world";
        StringHelper s( constChar );
      
        Assertion::AssertEquals( static_cast<String*>(s), S"Hello world" );
    }

    void StringHelperTest::TestFromSystemString()
    {
        String* sysString = S"Hello world";
        StringHelper s( sysString );

        Assertion::Assert( 
            std::strcmp( static_cast<const char*>(s), "Hello world" ) == 0 );
    }

    void StringHelperTest::TestCopying()
    {
        StringHelper s( "Hello world" );

        //make sure the const char* ptr is created before it is copied
        const char* ptr = static_cast<const char*>(s);

        StringHelper s2( s );

        Assertion::Assert( static_cast<String*>(s2) == static_cast<String*>(s) );

        //make sure they're not the same string
        const char* ptr1 = s;
        const char* ptr2 = s2;
        Assertion::Assert( ptr1 != ptr2 );

    }

    void StringHelperTest::TestAssignment()
    {
        StringHelper s( "Hello world" );

        StringHelper s2 = S"Moo world";

        s2 = s;
        Assertion::Assert( static_cast<String*>(s2) == static_cast<String*>(s) );

        //make sure they're not the same char ptr
        const char* ptr1 = s;
        const char* ptr2 = s2;
        Assertion::Assert( ptr1 != ptr2 );

    }
}
}
}
}

