#pragma once
#include "stdafx.h"
using namespace NUnit::Framework;

namespace NSvn
{
    namespace Core
    {
        namespace Tests
        {
            namespace MCpp
            {
                [TestFixture]
                public __gc class ManagedPointerTest
                {
                public:
                    /// <summary>Test that the thing works</summary>
                    [Test]
                    void TestBasic();

                    [Test]
                    void TestAssignment();

                    [Test]
                    void TestCopying();

                };
            }
        }
    }
}
