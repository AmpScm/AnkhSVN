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
                public __gc class StreamTest
                {
                public:
                    [Test]
                    void TestWrite();

                    [Test]
                    void TestRead();

                    [Test]
                    void TestReadFromNullStream();

                };
            }
        }
    }
}