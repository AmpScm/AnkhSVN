using System;
using System.Collections.Generic;
using System.Text;
using ErrorReportExtractor;
using DotNetMock.Dynamic;
using NUnit.Framework;

namespace ErrorReportExtractorTest
{
    [TestFixture]
    public class MailContainerTest
    {
        [SetUp]
        public void SetUp()
        {
            this.progressCallbackMock = new DynamicMock<IProgressCallback>();
            this.progressCallbackMock.ExpectNoCall("Exception");


        }

        [TearDown]
        public void TearDown()
        {
        }

        private DynamicMock<IProgressCallback> progressCallbackMock;
    }
}
