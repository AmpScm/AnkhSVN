#pragma once
#include "stdafx.h"


namespace NSvn
{
    namespace Core
    {
        namespace Tests
        {
            namespace MCpp
            {

                using namespace NUnit::Framework;
                [TestFixture]
                public __gc class PoolTest
                {
                public:
                    /// <summary>Test that the Allocate method works and that
                    /// destructors on allocated objects are run when the pool is destroyed
                    /// </summary>
                    [Test]
                    void TestAllocateObject();

                    [Test]
                    void TestAlloc();

                    [Test]
                    void TestPCalloc();
                };
            }
        }
    }
}