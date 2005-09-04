// $Id$
#pragma once
#include "stdafx.h"
using namespace NUnit::Framework;

namespace NSvn{
    namespace Core{
        namespace Tests{
            namespace MCpp{
                [TestFixture]
                public __gc class MiscTests
                {
                public:
                    [Test]
                    void TestRevisionFromNumber();

                    [Test]
                    void TestRevisionFromDate();

                    [Test]
                    void TestParameterDictionary();

					[Test]
					void TestStringToUtf8();

					[Test]
					void TestUtf8ToString();

                };
            }
        }
    }
}
