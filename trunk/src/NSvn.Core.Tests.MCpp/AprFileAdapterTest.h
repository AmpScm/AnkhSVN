#pragma once
#include "stdafx.h"
#using <nunit.framework.dll>

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
                public __gc class AprFileAdapterTest
                {
                public:
                    [Test]
                    void TestBasic();
                };
            }
        }
    }
}
