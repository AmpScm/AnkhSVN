// $Id$
#include "stdafx.h"
#include "PoolTest.h"
#include "Pool.h"

bool destructorCalled = false;

class Class
{
public:
    int a;
    ~Class()
    {
        destructorCalled = true;
    }
};

void NSvn::Core::Tests::MCpp::PoolTest::TestAllocateObject()
{
    Pool* p = new Pool();
    Class* ptr = p->AllocateObject( Class() );

    Assertion::Assert( ptr != 0 );

    //deleting the pool should cause cleanup functions to be run
    delete p;

    Assertion::Assert( "Destructor not called", destructorCalled );
}

void NSvn::Core::Tests::MCpp::PoolTest::TestAlloc()
{
    Pool p;
    Assertion::Assert( p.Alloc( 100 ) != 0 );
}

void NSvn::Core::Tests::MCpp::PoolTest::TestPCalloc()
{
    Pool p;
    char* ptr = static_cast<char*>(p.PCalloc( 100 ) );

    for ( int i = 0; i < 100; i++ )
        Assertion::Assert( ptr[i] == 0 );
}


