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
        IFileIconMapper mapper;
        IAnkhServiceProvider sp;
        [SetUp]
        public void SetUp()
        {
            sp = new AnkhServiceProvider();

            mapper = new FileIconMapper(sp);
        }

        [TearDown]
        public void TearDown()
        {
            mapper = null;
        }

        [Test]
        public void TestNonExistingFileType()
        {
            Assert.That(mapper.GetFileType((string)null), Is.EqualTo(""));
            Assert.That(mapper.GetFileType(""), Is.EqualTo(""));
            Assert.That(mapper.GetFileType(".exe"), Is.EqualTo("Application"));
            Assert.That(mapper.GetFileType("exe"), Is.EqualTo("Application"));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestNullSvnItem()
        {
            mapper.GetFileType((SvnItem)null);
        }

        [Test]
        public void TestExistingFileType()
        {
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

        [Test]
        public void TestIconForExtension()
        {
            Assert.That(mapper.DirectoryIcon, Is.GreaterThan(0));
            Assert.That(mapper.FileIcon, Is.GreaterThan(0));

            Assert.That(mapper.GetIconForExtension(null), Is.EqualTo(mapper.FileIcon));
            Assert.That(mapper.GetIconForExtension(""), Is.EqualTo(mapper.FileIcon));
            Assert.That(mapper.GetIconForExtension("qweqweqeqwe"), Is.EqualTo(mapper.FileIcon));
            Assert.That(mapper.GetIconForExtension("exe"), Is.GreaterThan(0));
        }

        [Test]
        public void GetSpecialIcon()
        {
            foreach(Environment.SpecialFolder folder in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                Assert.That(mapper.GetSpecialFolderIcon(folder), Is.GreaterThan(0), "Failed with value: {0}", folder);
            }

            foreach (WindowsSpecialFolder folder in Enum.GetValues(typeof(WindowsSpecialFolder)))
            {
                if (folder == WindowsSpecialFolder.MyDocuments)
                    continue; // fails, find out why
                if (folder == WindowsSpecialFolder.ResourcesLocalized)
                    continue;
                if (folder == WindowsSpecialFolder.CommonOemLinks)
                    continue;
                    
                Assert.That(mapper.GetSpecialFolderIcon(folder), Is.GreaterThan(0), "Failed with value: {0}", folder);
            }

            foreach (SpecialIcon icon in Enum.GetValues(typeof(SpecialIcon)))
            {
                if (icon == SpecialIcon.Blank)
                    Assert.That(mapper.GetSpecialIcon(icon), Is.EqualTo(0));
                else
                    Assert.That(mapper.GetSpecialIcon(icon), Is.GreaterThan(0), "Failed with value: {0}", icon);
            }
        }

        [Test]
        public void TestStateIcons()
        {
            foreach (StateIcon icon in Enum.GetValues(typeof(StateIcon)))
            {
                Assert.That(mapper.GetStateIcon(icon), Is.GreaterThanOrEqualTo(0), "Failed with value: {0}", icon);
            }
        }
    }
}
