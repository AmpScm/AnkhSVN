// $Id: StringHelperTest.h 128 2003-02-16 15:37:11Z Arild $
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
                    void TestSimpleCredential();

                    [Test]
                    void TestUsernameCredential();

                    [Test]
                    void TestRevisionFromNumber();

                    [Test]
                    void TestRevisionFromDate();
                };
            }
        }
    }
}
