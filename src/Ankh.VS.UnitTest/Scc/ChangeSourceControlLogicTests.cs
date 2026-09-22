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
using System.Collections.Generic;
using System.IO;

using Ankh.Scc.SccUI;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Scc
{
    [TestFixture]
    public class ChangeSourceControlLogicTests
    {
        [Test]
        public void BuildSelection_RejectsNullInputs()
        {
            Assert.Throws<ArgumentNullException>(
                () => ChangeSourceControlLogic.BuildSelection(null));

            Assert.Throws<ArgumentException>(
                () => ChangeSourceControlLogic.BuildSelection(
                    new ChangeSourceControlProjectBinding[] { null }));
        }

        [Test]
        public void NormalizeRelativePath_HandlesRootChildAndPrefixCollision()
        {
            string root = Root("repo");
            string child = Path.Combine(root, "src", "project");
            string collision = root + "-other";

            Assert.That(
                ChangeSourceControlLogic.NormalizeRelativePath(root, root),
                Is.EqualTo("."));
            Assert.That(
                ChangeSourceControlLogic.NormalizeRelativePath(child, root),
                Is.EqualTo(Path.Combine("src", "project")));
            Assert.That(
                ChangeSourceControlLogic.NormalizeRelativePath(collision, root),
                Is.EqualTo(collision));
        }

        [Test]
        public void BuildSelection_UsesSingleProjectValues()
        {
            Uri uri = new Uri("https://example.invalid/svn/project/");
            string root = Root("repo");

            ChangeSourceControlSelectionState state =
                ChangeSourceControlLogic.BuildSelection(
                    Projects(
                        Project(
                            root,
                            root,
                            "solution/project.csproj",
                            uri)));

            Assert.That(state.ProjectBase, Is.EqualTo(root));
            Assert.That(state.RelativePath, Is.EqualTo("."));
            Assert.That(
                state.ProjectLocation,
                Is.EqualTo("solution/project.csproj"));
            Assert.That(state.ProjectUri, Is.EqualTo(uri));
            Assert.That(state.ProjectBaseText, Is.EqualTo(root));
            Assert.That(state.RelativePathText, Is.EqualTo("."));
            Assert.That(
                state.ProjectLocationText,
                Is.EqualTo("solution/project.csproj"));
            Assert.That(
                state.ProjectUrlText,
                Is.EqualTo(uri.ToString()));
        }

        [Test]
        public void BuildSelection_PreservesCommonValuesIgnoringStringCase()
        {
            Uri uri = new Uri("https://example.invalid/svn/project/");
            string root = Root("Repo");

            ChangeSourceControlSelectionState state =
                ChangeSourceControlLogic.BuildSelection(
                    Projects(
                        Project(
                            root,
                            Path.Combine(root, "Src"),
                            "Solution/Project.csproj",
                            uri),
                        Project(
                            root.ToLowerInvariant(),
                            Path.Combine(root.ToLowerInvariant(), "src"),
                            "solution/project.csproj",
                            uri)));

            Assert.That(state.ProjectBase, Is.EqualTo(root));
            Assert.That(state.RelativePath, Is.EqualTo("Src"));
            Assert.That(
                state.ProjectLocation,
                Is.EqualTo("Solution/Project.csproj"));
            Assert.That(state.ProjectUri, Is.EqualTo(uri));
        }

        [Test]
        public void BuildSelection_ClearsDifferentCommonValues()
        {
            string root = Root("repo");
            Uri firstUri = new Uri("https://example.invalid/svn/one/");
            Uri secondUri = new Uri("https://example.invalid/svn/two/");

            ChangeSourceControlSelectionState state =
                ChangeSourceControlLogic.BuildSelection(
                    Projects(
                        Project(
                            root,
                            Path.Combine(root, "one"),
                            "one.csproj",
                            firstUri),
                        Project(
                            Root("other"),
                            Path.Combine(Root("other"), "two"),
                            "two.csproj",
                            secondUri)));

            Assert.That(state.ProjectBase, Is.Null);
            Assert.That(state.RelativePath, Is.Null);
            Assert.That(state.ProjectLocation, Is.Null);
            Assert.That(state.ProjectUri, Is.Null);
            Assert.That(state.ProjectBaseText, Is.Empty);
            Assert.That(state.RelativePathText, Is.EqualTo("."));
            Assert.That(state.ProjectLocationText, Is.Empty);
            Assert.That(state.ProjectUrlText, Is.Empty);
        }

        [Test]
        public void BuildSelection_InvalidProjectStopsAccumulation()
        {
            Uri uri = new Uri("https://example.invalid/svn/project/");
            string root = Root("repo");

            ChangeSourceControlSelectionState state =
                ChangeSourceControlLogic.BuildSelection(
                    Projects(
                        Project(
                            root,
                            Path.Combine(root, "one"),
                            "one.csproj",
                            uri),
                        new ChangeSourceControlProjectBinding(
                            false,
                            null,
                            null,
                            null,
                            null),
                        Project(
                            root,
                            Path.Combine(root, "two"),
                            "two.csproj",
                            uri)));

            Assert.That(state.ProjectBase, Is.Null);
            Assert.That(state.RelativePath, Is.Null);
            Assert.That(state.ProjectLocation, Is.Null);
            Assert.That(
                state.ProjectUri,
                Is.EqualTo(uri),
                "Preserve the existing method's URI behavior when an invalid later selection stops processing.");
        }

        [Test]
        public void BuildSelection_EmptySelectionUsesBlankDisplayValues()
        {
            ChangeSourceControlSelectionState state =
                ChangeSourceControlLogic.BuildSelection(
                    new List<ChangeSourceControlProjectBinding>());

            Assert.That(state.ProjectBaseText, Is.Empty);
            Assert.That(state.RelativePathText, Is.EqualTo("."));
            Assert.That(state.ProjectLocationText, Is.Empty);
            Assert.That(state.ProjectUrlText, Is.Empty);
        }

        [Test]
        public void BuildSelection_DifferentRelativeOrLocationValuesCollapseIndependently()
        {
            string root = Root("repo");
            Uri uri = new Uri("https://example.invalid/svn/project/");

            ChangeSourceControlSelectionState state =
                ChangeSourceControlLogic.BuildSelection(
                    Projects(
                        Project(
                            root,
                            Path.Combine(root, "one"),
                            "one.csproj",
                            uri),
                        Project(
                            root,
                            Path.Combine(root, "two"),
                            "two.csproj",
                            uri)));

            Assert.That(state.ProjectBase, Is.EqualTo(root));
            Assert.That(state.RelativePath, Is.Null);
            Assert.That(state.ProjectLocation, Is.Null);
            Assert.That(state.ProjectUri, Is.EqualTo(uri));
            Assert.That(state.RelativePathText, Is.EqualTo("."));
        }

        static ChangeSourceControlProjectBinding Project(
            string projectBase,
            string projectDirectory,
            string projectLocation,
            Uri projectUri)
        {
            return new ChangeSourceControlProjectBinding(
                true,
                projectBase,
                projectDirectory,
                projectLocation,
                projectUri);
        }

        static IList<ChangeSourceControlProjectBinding> Projects(
            params ChangeSourceControlProjectBinding[] projects)
        {
            return new List<ChangeSourceControlProjectBinding>(projects);
        }

        static string Root(string name)
        {
            return Path.Combine(
                Path.GetPathRoot(Environment.CurrentDirectory),
                name);
        }
    }
}
