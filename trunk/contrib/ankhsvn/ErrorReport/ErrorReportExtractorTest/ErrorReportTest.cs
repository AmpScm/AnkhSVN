using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Outlook;
using ErrorReportExtractor;

namespace ErrorReportExtractorTest
{
    [TestFixture]
    public class ErrorReportTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void Version()
        {
            ErrorReport report = GetErrorReport(ErrorReportResources.Report1);
            Assert.AreEqual(0, report.MajorVersion);
            Assert.AreEqual(6, report.MinorVersion);
            Assert.AreEqual(0, report.PatchVersion);
            Assert.AreEqual(2380, report.Revision);
        }

        [Test]
        public void NoVersion()
        {
            ErrorReport report = GetErrorReport(ErrorReportResources.EmptyReport);
            Assert.IsNull(report.MajorVersion);
            Assert.IsNull(report.MinorVersion);
            Assert.IsNull(report.PatchVersion);
            Assert.IsNull(report.Revision);
        }

        [Test]
        public void ExceptionType()
        {
            ErrorReport report = GetErrorReport(ErrorReportResources.Report1);
            Assert.AreEqual("NSvn.Core.SvnClientException", report.ExceptionType);
        }

        [Test]
        public void NoExceptionType()
        {
            ErrorReport report = GetErrorReport(ErrorReportResources.EmptyReport);
            Assert.IsNull(report.ExceptionType);
        }

        [Test]
        public void DteVersion()
        {
            ErrorReport report = GetErrorReport(ErrorReportResources.Report1);
            Assert.AreEqual("8.0", report.DteVersion);
        }

        [Test]
        public void OldDteVersion()
        {
            ErrorReport report = GetErrorReport(ErrorReportResources.OldDteVersionReport);
            Assert.AreEqual("8.0", report.DteVersion);
        }

        [Test]
        public void NoDteVersion()
        {
            ErrorReport report = GetErrorReport(ErrorReportResources.EmptyReport);
            Assert.IsNull(report.DteVersion);
        }

        [Test]
        public void StackTrace()
        {
            ErrorReport report = GetErrorReport(ErrorReportResources.OldDteVersionReport);
            Assert.AreEqual(7, report.StackTrace.Count);
        }

        private ErrorReport GetErrorReport(string report)
        {
            return new ErrorReport("", "", report, "", DateTime.Now);
        }

    }
}
