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

	Assert::IsTrue( ptr != 0 );

    //deleting the pool should cause cleanup functions to be run
    delete p;

	Assert::IsTrue( destructorCalled, "Destructor not called" );
}

void NSvn::Core::Tests::MCpp::PoolTest::TestAlloc()
{
    Pool p;
	Assert::IsTrue( p.Alloc( 100 ) != 0 );
}

void NSvn::Core::Tests::MCpp::PoolTest::TestPCalloc()
{
    Pool p;
    char* ptr = static_cast<char*>(p.PCalloc( 100 ) );

    for ( int i = 0; i < 100; i++ )
		Assert::IsTrue( ptr[i] == 0 );
}


