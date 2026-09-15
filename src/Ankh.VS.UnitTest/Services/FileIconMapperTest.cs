// Copyright 2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.IO;
using Ankh;
using Ankh.Scc;
using Ankh.VS;
using Ankh.VS.SolutionExplorer;
using AnkhSvn_UnitTestProject.Helpers;
using Moq;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Services
{
    [TestFixture]
    public class FileIconMapperTest
    {
        IFileIconMapper mapper;

        [SetUp]
        public void SetUp()
        {
            IAnkhServiceProvider serviceProvider = new AnkhServiceProvider();
            mapper = new FileIconMapper(serviceProvider);
        }

        [TearDown]
        public void TearDown()
        {
            mapper = null;
        }

        [TestCase(null)]
        [TestCase("")]
        public void EmptyExtensionHasNoFileType(string extension)
        {
            Assert.That(mapper.GetFileType(extension), Is.EqualTo(""));
        }

        [Test]
        public void ExtensionWithAndWithoutDotResolveToSameType()
        {
            string withoutDot = mapper.GetFileType("exe");
            string withDot = mapper.GetFileType(".exe");

            Assert.That(withoutDot, Is.Not.Null.And.Not.Empty);
            Assert.That(withDot, Is.EqualTo(withoutDot));
        }

        [Test]
        public void NullSvnItemThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => mapper.GetFileType((SvnItem)null));
        }

        [Test]
        public void ExistingFileUsesAResolvedShellType()
        {
            var statusCache = new Mock<ISvnStatusCache>();
            string file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".txt");
            File.WriteAllText(file, "test");

            try
            {
                var item = new SvnItem(statusCache.Object, file, NoSccStatus.Unknown, SharpSvn.SvnNodeKind.File);
                string fileType = mapper.GetFileType(item);

                Assert.That(fileType, Is.Not.Null.And.Not.Empty);
            }
            finally
            {
                File.Delete(file);
            }
        }

        [Test]
        public void GenericFileAndDirectoryIconsCanBeResolved()
        {
            Assert.That(mapper.DirectoryIcon, Is.GreaterThanOrEqualTo(0));
            Assert.That(mapper.FileIcon, Is.GreaterThanOrEqualTo(0));
            Assert.That(mapper.DirectoryIcon, Is.Not.EqualTo(mapper.FileIcon));
        }

        [TestCase(null)]
        [TestCase("")]
        public void EmptyExtensionUsesGenericFileIcon(string extension)
        {
            Assert.That(mapper.GetIconForExtension(extension), Is.EqualTo(mapper.FileIcon));
        }

        [Test]
        public void KnownExtensionCanResolveAnIcon()
        {
            Assert.That(mapper.GetIconForExtension("exe"), Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void ProjectIconReferenceEqualityUsesBothListAndIndex()
        {
            var sameOne = new ProjectIconReference(new IntPtr(3), 4);
            var sameTwo = new ProjectIconReference(new IntPtr(3), 4);
            var differentIndex = new ProjectIconReference(new IntPtr(3), 5);
            var differentList = new ProjectIconReference(new IntPtr(4), 4);

            Assert.That(sameOne, Is.EqualTo(sameTwo));
            Assert.That(sameOne, Is.Not.EqualTo(differentIndex));
            Assert.That(sameOne, Is.Not.EqualTo(differentList));
        }

        [Test]
        public void ProjectIconReferenceEqualityDistinguishesHandlesFromImageListReferences()
        {
            var handleOne = new ProjectIconReference(new IntPtr(3));
            var handleTwo = new ProjectIconReference(new IntPtr(3));
            var otherHandle = new ProjectIconReference(new IntPtr(4));
            var imageListReference = new ProjectIconReference(new IntPtr(3), 3);

            Assert.That(handleOne, Is.EqualTo(handleTwo));
            Assert.That(handleOne, Is.Not.EqualTo(otherHandle));
            Assert.That(handleOne, Is.Not.EqualTo(imageListReference));
        }

        [Test]
        public void EmbeddedSpecialIconsAreAvailable()
        {
            foreach (SpecialIcon icon in Enum.GetValues(typeof(SpecialIcon)))
                Assert.That(mapper.GetSpecialIcon(icon), Is.GreaterThanOrEqualTo(0), "Failed with value: {0}", icon);
        }

        [Test]
        public void StateIconsMapToEmbeddedSpecialIcons()
        {
            Assert.That(mapper.GetStateIcon(StateIcon.Blank), Is.EqualTo(mapper.GetSpecialIcon(SpecialIcon.Blank)));
            Assert.That(mapper.GetStateIcon(StateIcon.Incoming), Is.EqualTo(mapper.GetSpecialIcon(SpecialIcon.Incoming)));
            Assert.That(mapper.GetStateIcon(StateIcon.Outgoing), Is.EqualTo(mapper.GetSpecialIcon(SpecialIcon.Outgoing)));
            Assert.That(mapper.GetStateIcon(StateIcon.Collision), Is.EqualTo(mapper.GetSpecialIcon(SpecialIcon.Collision)));
        }

        [Test]
        public void ExistingFileIconCanBeResolved()
        {
            string file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".txt");
            File.WriteAllText(file, "test");

            try
            {
                Assert.That(mapper.GetIcon(file), Is.GreaterThanOrEqualTo(0));
            }
            finally
            {
                File.Delete(file);
            }
        }

        [Test]
        public void GetIconRejectsNullPath()
        {
            Assert.Throws<ArgumentNullException>(() => mapper.GetIcon(null));
        }
    }
}
