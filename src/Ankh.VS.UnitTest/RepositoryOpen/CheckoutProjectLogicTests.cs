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
using Ankh.UI.RepositoryOpen;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.RepositoryOpen
{
    [TestFixture]
    public class CheckoutProjectLogicTests
    {
        [Test]
        public void BuildLoadPlan_WalksFromProjectDirectoryToRepositoryRoot()
        {
            CheckoutProjectLoadPlan plan = CheckoutProjectLogic.BuildLoadPlan(
                new Uri("https://example.invalid/svn/"),
                new Uri("https://example.invalid/svn/trunk/project/project.csproj"),
                null);

            Assert.That(
                plan.RepositoryRootUri,
                Is.EqualTo(new Uri("https://example.invalid/svn/")));

            CollectionAssert.AreEqual(
                new[]
                {
                    new Uri("https://example.invalid/svn/trunk/project/"),
                    new Uri("https://example.invalid/svn/trunk/"),
                    new Uri("https://example.invalid/svn/")
                },
                plan.Candidates);
        }

        [Test]
        public void BuildLoadPlan_HonorsExplicitProjectTop()
        {
            Uri projectTop = new Uri("https://example.invalid/svn/branches/feature/");

            CheckoutProjectLoadPlan plan = CheckoutProjectLogic.BuildLoadPlan(
                new Uri("https://example.invalid/svn/"),
                new Uri("https://example.invalid/svn/branches/feature/src/project.csproj"),
                projectTop);

            Assert.That(plan.Candidates[0], Is.EqualTo(projectTop));
        }

        [Test]
        public void BuildLoadPlan_AdjustsRootWhenProjectIsOutsideSuppliedRepository()
        {
            CheckoutProjectLoadPlan plan = CheckoutProjectLogic.BuildLoadPlan(
                new Uri("https://example.invalid/repo-a/"),
                new Uri("https://other.invalid/repo-b/trunk/project/project.csproj"),
                null);

            Assert.That(
                plan.RepositoryRootUri,
                Is.EqualTo(new Uri("https://other.invalid/")));
            Assert.That(
                plan.Candidates[plan.Candidates.Count - 1],
                Is.EqualTo(new Uri("https://other.invalid/")));
        }

        [Test]
        public void BuildLoadPlan_RejectsMissingRequiredUris()
        {
            Assert.Throws<ArgumentNullException>(
                () => CheckoutProjectLogic.BuildLoadPlan(
                    null,
                    new Uri("https://example.invalid/svn/project.csproj"),
                    null));

            Assert.Throws<ArgumentNullException>(
                () => CheckoutProjectLogic.BuildLoadPlan(
                    new Uri("https://example.invalid/svn/"),
                    null,
                    null));
        }

        [Test]
        public void SelectDefaultIndex_PrefersGuessedWorkingRoot()
        {
            IList<Uri> candidates = Candidates(
                "https://example.invalid/svn/trunk/project/",
                "https://example.invalid/svn/trunk/",
                "https://example.invalid/svn/");

            Assert.That(
                CheckoutProjectLogic.SelectDefaultIndex(
                    candidates,
                    new Uri("https://example.invalid/svn/")),
                Is.EqualTo(2));
        }

        [Test]
        public void SelectDefaultIndex_PrefersTrunkWithoutLayoutGuess()
        {
            IList<Uri> candidates = Candidates(
                "https://example.invalid/svn/trunk/project/",
                "https://example.invalid/svn/trunk/",
                "https://example.invalid/svn/");

            Assert.That(
                CheckoutProjectLogic.SelectDefaultIndex(candidates, null),
                Is.EqualTo(1));
        }

        [TestCase("/branches/")]
        [TestCase("/tags/")]
        [TestCase("/releases/")]
        public void SelectDefaultIndex_SelectsChildOfContainer(string container)
        {
            IList<Uri> candidates = Candidates(
                "https://example.invalid/svn" + container + "feature/src/",
                "https://example.invalid/svn" + container + "feature/",
                "https://example.invalid/svn" + container,
                "https://example.invalid/svn/");

            Assert.That(
                CheckoutProjectLogic.SelectDefaultIndex(candidates, null),
                Is.EqualTo(1));
        }

        [TestCase("/src/")]
        [TestCase("/source/")]
        [TestCase("/sourcecode/")]
        public void SelectDefaultIndex_SelectsParentOfSourceContainer(string source)
        {
            IList<Uri> candidates = Candidates(
                "https://example.invalid/svn" + source + "project/",
                "https://example.invalid/svn" + source,
                "https://example.invalid/svn/");

            Assert.That(
                CheckoutProjectLogic.SelectDefaultIndex(candidates, null),
                Is.EqualTo(2));
        }

        [Test]
        public void SelectDefaultIndex_FallsBackToFirstCandidateOrNone()
        {
            Assert.That(
                CheckoutProjectLogic.SelectDefaultIndex(
                    Candidates(
                        "https://example.invalid/svn/custom/project/",
                        "https://example.invalid/svn/custom/"),
                    null),
                Is.Zero);

            Assert.That(
                CheckoutProjectLogic.SelectDefaultIndex(
                    new List<Uri>(),
                    null),
                Is.EqualTo(-1));
        }

        [Test]
        public void SelectDefaultIndex_RejectsNullCandidateList()
        {
            Assert.Throws<ArgumentNullException>(
                () => CheckoutProjectLogic.SelectDefaultIndex(null, null));
        }

        static IList<Uri> Candidates(params string[] values)
        {
            List<Uri> result = new List<Uri>();
            foreach (string value in values)
                result.Add(new Uri(value));

            return result;
        }
    }
}
