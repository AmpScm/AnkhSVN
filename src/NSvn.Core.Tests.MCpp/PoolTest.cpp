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
