#pragma once
#using <mscorlib.dll>
#using <NUnit.Framework.dll>
using namespace NUnit::Framework;

namespace NSvn{
    namespace Core{
        namespace Tests{
            namespace MCpp{
                [TestFixture]
                public __gc class StringHelperTest
                {
                public:
                    [Test]
                    //Test that the implicit conversion to String* works
                    void TestFromConstChar(); 

                    //Test that the implicit conversion to char* works
                    [Test]
                    void TestFromSystemString();

                    //Test that the string can be copied
                    [Test]
                    void TestCopying();

                    ///<summary>Test that the string can be copied</summary>
                    [Test]
                    void TestAssignment();

                    [Test]
                    void TestCopyToPool();
                };
            }
        }
    }
}