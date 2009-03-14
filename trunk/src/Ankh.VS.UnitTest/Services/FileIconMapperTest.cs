using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ankh.VS.SolutionExplorer;
using AnkhSvn_UnitTestProject.Helpers;
using Ankh.VS;
using NUnit.Framework.SyntaxHelpers;
using Ankh;
using System.IO;
using Ankh.Scc;
using Moq;

namespace AnkhSvn_UnitTestProject.Services
{
    [TestFixture]
    public class FileIconMapperTest
    {
        [Test]
        public void TestNonExistingFileType()
        {
            AnkhServiceProvider sp = new AnkhServiceProvider();
            
            IFileIconMapper mapper = new FileIconMapper(sp);

            Assert.That(mapper.GetFileType((string)null), Is.EqualTo(""));
            Assert.That(mapper.GetFileType(""), Is.EqualTo(""));
            Assert.That(mapper.GetFileType(".exe"), Is.EqualTo("Application"));
            Assert.That(mapper.GetFileType("exe"), Is.EqualTo("Application"));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestNullSvnItem()
        {
            AnkhServiceProvider sp = new AnkhServiceProvider();
            IFileIconMapper mapper = new FileIconMapper(sp);

            mapper.GetFileType((SvnItem)null);
        }

        [Test]
        public void TestExistingFileType()
        {
            AnkhServiceProvider sp = new AnkhServiceProvider();
            var statusCache = new Mock<IFileStatusCache>();


            IFileIconMapper mapper = new FileIconMapper(sp);
            string tempFile = Path.GetTempFileName();
            string exeTempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".exe");
            using (File.CreateText(exeTempFile))
            { }

            try
            {
                SvnItem item = new SvnItem(statusCache.Object, tempFile, NoSccStatus.Unknown, SharpSvn.SvnNodeKind.File);
                Assert.That(mapper.GetFileType(item), Is.EqualTo("TMP File"));

                item = new SvnItem(statusCache.Object, exeTempFile, NoSccStatus.Unknown, SharpSvn.SvnNodeKind.File);
                Assert.That(mapper.GetFileType(item), Is.EqualTo("Application"));

                item = new SvnItem(statusCache.Object, "C:\\", NoSccStatus.Unknown, SharpSvn.SvnNodeKind.Directory);
                Assert.That(mapper.GetFileType(item), Is.EqualTo("Local Disk"));
            }
            finally
            {
                File.Delete(tempFile);
                File.Delete(exeTempFile);
            }
        }
    }
}
