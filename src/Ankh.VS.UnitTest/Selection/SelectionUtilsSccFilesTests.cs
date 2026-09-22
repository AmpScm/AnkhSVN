// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;

using Ankh;
using Ankh.Services;
using Ankh.VS.Selection;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Selection
{
    [TestFixture]
    public class SelectionUtilsSccFilesTests
    {
        const uint ItemId = 42;

        [Test]
        public void GetSccFiles_VcProjectEmptySccResultFallsBackToProjectDocument()
        {
            string root = Path.Combine(
                Path.GetTempPath(),
                "ankh-issue-17");
            string projectFile = Path.Combine(root, "Sample.vcxproj");
            string sourceFile = Path.Combine(root, "Tasks", "Sample.cpp");

            Mock<IVsHierarchy> hierarchy;
            Mock<IVsProject> project;
            CreateProjectHierarchy(
                projectFile,
                sourceFile,
                false,
                out hierarchy,
                out project);

            Mock<IVsSccProject2> sccProject =
                CreateEmptySccProject();

            string[] files;
            int[] flags;

            bool result = SelectionUtils.GetSccFiles(
                hierarchy.Object,
                sccProject.Object,
                ItemId,
                out files,
                out flags,
                false,
                null);

            Assert.That(result, Is.True);
            Assert.That(files, Is.EqualTo(new[] { sourceFile }));

            project.Verify(
                p => p.GetMkDocument(ItemId, out sourceFile),
                Times.Once);
        }

        [Test]
        public void GetSccFiles_VcProjectNonMemberDoesNotFallback()
        {
            string root = Path.Combine(
                Path.GetTempPath(),
                "ankh-issue-17");
            string projectFile = Path.Combine(root, "Sample.vcxproj");
            string sourceFile = Path.Combine(root, "Generated.cpp");

            Mock<IVsHierarchy> hierarchy;
            Mock<IVsProject> project;
            CreateProjectHierarchy(
                projectFile,
                sourceFile,
                true,
                out hierarchy,
                out project);

            Mock<IVsSccProject2> sccProject =
                CreateEmptySccProject();

            string[] files;
            int[] flags;

            bool result = SelectionUtils.GetSccFiles(
                hierarchy.Object,
                sccProject.Object,
                ItemId,
                out files,
                out flags,
                false,
                null);

            Assert.That(result, Is.True);
            Assert.That(files, Is.Empty);

            project.Verify(
                p => p.GetMkDocument(ItemId, out sourceFile),
                Times.Never);
        }

        [Test]
        public void GetSccFiles_NonVcProjectPreservesEmptySccResult()
        {
            string root = Path.Combine(
                Path.GetTempPath(),
                "ankh-issue-17");
            string projectFile = Path.Combine(root, "Sample.csproj");
            string sourceFile = Path.Combine(root, "Sample.cs");

            Mock<IVsHierarchy> hierarchy;
            Mock<IVsProject> project;
            CreateProjectHierarchy(
                projectFile,
                sourceFile,
                false,
                out hierarchy,
                out project);

            Mock<IVsSccProject2> sccProject =
                CreateEmptySccProject();

            string[] files;
            int[] flags;

            bool result = SelectionUtils.GetSccFiles(
                hierarchy.Object,
                sccProject.Object,
                ItemId,
                out files,
                out flags,
                false,
                null);

            Assert.That(result, Is.True);
            Assert.That(files, Is.Empty);

            project.Verify(
                p => p.GetMkDocument(ItemId, out sourceFile),
                Times.Never);
        }

        [Test]
        public void ShouldTryVcProjectDocumentFallback_RejectsProjectRoot()
        {
            string projectFile = Path.Combine(
                Path.GetTempPath(),
                "Sample.vcxproj");

            Mock<IVsHierarchy> hierarchy = new Mock<IVsHierarchy>();
            Mock<IVsProject> project = hierarchy.As<IVsProject>();

            project
                .Setup(
                    p => p.GetMkDocument(
                        VSItemId.Root,
                        out projectFile))
                .Returns(VSErr.S_OK);

            Assert.That(
                SelectionUtils.ShouldTryVcProjectDocumentFallback(
                    hierarchy.Object,
                    VSItemId.Root),
                Is.False);
        }

        static Mock<IVsSccProject2> CreateEmptySccProject()
        {
            Mock<IVsSccProject2> sccProject =
                new Mock<IVsSccProject2>();

            sccProject
                .Setup(
                    p => p.GetSccFiles(
                        ItemId,
                        It.IsAny<CALPOLESTR[]>(),
                        It.IsAny<CADWORD[]>()))
                .Callback<uint, CALPOLESTR[], CADWORD[]>(
                    delegate(
                        uint id,
                        CALPOLESTR[] paths,
                        CADWORD[] flags)
                    {
                        paths[0] = new CALPOLESTR();
                        flags[0] = new CADWORD();
                    })
                .Returns(VSErr.S_OK);

            return sccProject;
        }

        static void CreateProjectHierarchy(
            string projectFile,
            string itemFile,
            bool isNonMember,
            out Mock<IVsHierarchy> hierarchy,
            out Mock<IVsProject> project)
        {
            hierarchy = new Mock<IVsHierarchy>();
            project = hierarchy.As<IVsProject>();

            string rootDocument = projectFile;
            project
                .Setup(
                    p => p.GetMkDocument(
                        VSItemId.Root,
                        out rootDocument))
                .Returns(VSErr.S_OK);

            string itemDocument = itemFile;
            project
                .Setup(
                    p => p.GetMkDocument(
                        ItemId,
                        out itemDocument))
                .Returns(VSErr.S_OK);

            object nonMember = isNonMember;
            hierarchy
                .Setup(
                    h => h.GetProperty(
                        ItemId,
                        (int)__VSHPROPID.VSHPROPID_IsNonMemberItem,
                        out nonMember))
                .Returns(VSErr.S_OK);
        }
    }
}
