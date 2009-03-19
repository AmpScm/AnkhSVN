using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ErrorReportExtractor;

namespace ErrorReportExtractorTest
{
    [TestFixture]
    public class StackTraceTest
    {
        [SetUp]
        public void SetUp()
        {
            this.st = new StackTrace(ErrorReportResources.OldDteVersionReport);

        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void Count()
        {
            Assert.AreEqual(7, st.Count);
        }

        [Test]
        public void Line1()
        {
            IStackTraceItem item = st[0];
            Assert.AreEqual("System.IO.Path.NormalizePathFast", item.MethodName);
            Assert.AreEqual("String path, Boolean fullCheck", item.Parameters);
            Assert.IsNull(item.Filename);
            Assert.IsNull(item.LineNumber);
            Assert.AreEqual(0, item.SequenceNumber);

        }

        [Test]
        public void Line2()
        {
            IStackTraceItem item = st[1];
            Assert.AreEqual("System.IO.Path.GetDirectoryName", item.MethodName);
            Assert.AreEqual("String path", item.Parameters);
            Assert.IsNull(item.Filename);
            Assert.IsNull(item.LineNumber);
            Assert.AreEqual(1, item.SequenceNumber);

        }

        [Test]
        public void Line3()
        {
            IStackTraceItem item = st[2];
            Assert.AreEqual("Ankh.Commands.AddItemCommand.AddFilter.Filter", item.MethodName);
            Assert.AreEqual("SvnItem item", item.Parameters);
            Assert.AreEqual(@"N:\tmp\build\src\Ankh\Commands\AddItemCommand.cs", item.Filename);
            Assert.AreEqual(113, item.LineNumber);
            Assert.AreEqual(2, item.SequenceNumber);

        }

        [Test]
        public void Line4()
        {
            IStackTraceItem item = st[3];
            Assert.AreEqual("Ankh.Solution.ProjectNode.GetResources", item.MethodName);
            Assert.AreEqual("IList list, Boolean getChildItems, ResourceFilterCallback filter", item.Parameters);
            Assert.AreEqual(@"N:\tmp\build\src\Ankh\Solution\ProjectNode.cs", item.Filename);
            Assert.AreEqual(28, item.LineNumber);
            Assert.AreEqual(3, item.SequenceNumber);

        }

        [Test]
        public void Line7()
        {
            IStackTraceItem item = st[6];
            Assert.AreEqual("Ankh.Connect.QueryStatus", item.MethodName);
            Assert.AreEqual("String commandName, vsCommandStatusTextWanted neededText, vsCommandStatus& status, Object& commandText", item.Parameters);
            Assert.AreEqual(@"N:\tmp\build\src\Ankh\Connect.cs", item.Filename);
            Assert.AreEqual(228, item.LineNumber);
            Assert.AreEqual(6, item.SequenceNumber);

        }

        private StackTrace st;


    }
}
