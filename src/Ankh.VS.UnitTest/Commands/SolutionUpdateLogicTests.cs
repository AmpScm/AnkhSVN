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
using Ankh;
using Ankh.Commands;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Commands
{
    [TestFixture]
    public class SolutionUpdateLogicTests
    {
        [TestCase(AnkhCommand.SolutionUpdateLatest, "Solution")]
        [TestCase(AnkhCommand.SolutionUpdateSpecific, "Solution")]
        [TestCase(AnkhCommand.PendingChangesUpdateLatest, "Solution")]
        [TestCase(AnkhCommand.FolderUpdateLatest, "Folder")]
        [TestCase(AnkhCommand.FolderUpdateSpecific, "Folder")]
        [TestCase(AnkhCommand.ProjectUpdateLatest, "Project")]
        [TestCase(AnkhCommand.ProjectUpdateSpecific, "Project")]
        public void GetScope_ClassifiesUpdateCommands(
            AnkhCommand command,
            string expected)
        {
            Assert.That(SolutionUpdateLogic.GetScope(command).ToString(), Is.EqualTo(expected));
        }

        [TestCase(AnkhCommand.SolutionUpdateLatest, true)]
        [TestCase(AnkhCommand.ProjectUpdateLatest, true)]
        [TestCase(AnkhCommand.PendingChangesUpdateLatest, true)]
        [TestCase(AnkhCommand.FolderUpdateLatest, true)]
        [TestCase(AnkhCommand.SolutionUpdateSpecific, false)]
        [TestCase(AnkhCommand.ProjectUpdateSpecific, false)]
        [TestCase(AnkhCommand.FolderUpdateSpecific, false)]
        public void IsHeadCommand_ClassifiesLatestVsSpecific(
            AnkhCommand command,
            bool expected)
        {
            Assert.That(SolutionUpdateLogic.IsHeadCommand(command), Is.EqualTo(expected));
        }

        [Test]
        public void UsesImplicitHeadRevision_AlsoHonorsDontPrompt()
        {
            Assert.That(
                SolutionUpdateLogic.UsesImplicitHeadRevision(
                    AnkhCommand.ProjectUpdateSpecific,
                    true),
                Is.True);

            Assert.That(
                SolutionUpdateLogic.UsesImplicitHeadRevision(
                    AnkhCommand.ProjectUpdateSpecific,
                    false),
                Is.False);

            Assert.That(
                SolutionUpdateLogic.UsesImplicitHeadRevision(
                    AnkhCommand.ProjectUpdateLatest,
                    false),
                Is.True);
        }

        [Test]
        public void GetCommonAncestorUri_FindsSharedRepositoryDirectory()
        {
            Uri left = new Uri("https://example.invalid/svn/trunk/project-a/src");
            Uri right = new Uri("https://example.invalid/svn/trunk/project-b/tests");

            Uri common = SolutionUpdateLogic.GetCommonAncestorUri(left, right);

            Assert.That(
                common.AbsoluteUri,
                Is.EqualTo("https://example.invalid/svn/trunk/"));
        }

        [Test]
        public void GetCommonAncestorUri_PreservesDeeperSharedDirectory()
        {
            Uri left = new Uri("https://example.invalid/svn/trunk/shared/a");
            Uri right = new Uri("https://example.invalid/svn/trunk/shared/b");

            Uri common = SolutionUpdateLogic.GetCommonAncestorUri(left, right);

            Assert.That(
                common.AbsoluteUri,
                Is.EqualTo("https://example.invalid/svn/trunk/shared/"));
        }

        [Test]
        public void GetCommonAncestorUri_RejectsNullInputs()
        {
            Assert.Throws<ArgumentNullException>(
                () => SolutionUpdateLogic.GetCommonAncestorUri(
                    null,
                    new Uri("https://example.invalid/svn/")));

            Assert.Throws<ArgumentNullException>(
                () => SolutionUpdateLogic.GetCommonAncestorUri(
                    new Uri("https://example.invalid/svn/"),
                    null));
        }

        [Test]
        public void ShouldIncludeUpdateRoot_RejectsDuplicatesAndUnversionedItems()
        {
            Uri repo = new Uri("https://example.invalid/svn/");

            Assert.That(
                SolutionUpdateLogic.ShouldIncludeUpdateRoot(
                    true, true, true, repo, repo),
                Is.False);

            Assert.That(
                SolutionUpdateLogic.ShouldIncludeUpdateRoot(
                    false, false, true, repo, repo),
                Is.False);
        }

        [Test]
        public void ShouldIncludeUpdateRoot_SpecificRevisionRejectsAnotherRepository()
        {
            Uri selected = new Uri("https://example.invalid/svn-a/");
            Uri other = new Uri("https://example.invalid/svn-b/");

            Assert.That(
                SolutionUpdateLogic.ShouldIncludeUpdateRoot(
                    false, true, false, selected, other),
                Is.False);

            Assert.That(
                SolutionUpdateLogic.ShouldIncludeUpdateRoot(
                    false, true, false, selected, selected),
                Is.True);
        }

        [Test]
        public void ShouldIncludeUpdateRoot_HeadUpdateAllowsMultipleRepositories()
        {
            Assert.That(
                SolutionUpdateLogic.ShouldIncludeUpdateRoot(
                    false,
                    true,
                    true,
                    new Uri("https://example.invalid/svn-a/"),
                    new Uri("https://example.invalid/svn-b/")),
                Is.True);
        }

        [Test]
        public void ShouldIncludeUpdateRoot_AllowsUnknownRepositoryWhenOriginalCodeDid()
        {
            Assert.That(
                SolutionUpdateLogic.ShouldIncludeUpdateRoot(
                    false,
                    true,
                    false,
                    new Uri("https://example.invalid/svn-a/"),
                    null),
                Is.True);
        }

        [Test]
        public void UpdatePlan_GroupsAcceptedRootsByWorkingCopy()
        {
            SolutionUpdatePlan plan = new SolutionUpdatePlan();
            Uri repo = new Uri("https://example.invalid/svn/");

            Assert.That(
                plan.TryAddRoot(
                    @"C:\wc\one",
                    true,
                    true,
                    repo,
                    repo,
                    @"C:\wc"),
                Is.True);
            Assert.That(
                plan.TryAddRoot(
                    @"C:\wc\two",
                    true,
                    true,
                    repo,
                    repo,
                    @"C:\wc"),
                Is.True);

            Assert.That(plan.GroupCount, Is.EqualTo(1));

            UpdateGroup group = null;
            foreach (UpdateGroup candidate in plan.Groups)
                group = candidate;

            Assert.That(group, Is.Not.Null);
            Assert.That(group.WorkingCopyRoot, Is.EqualTo(@"C:\wc"));
            CollectionAssert.AreEqual(
                new[] { @"C:\wc\one", @"C:\wc\two" },
                group.Nodes);
        }

        [Test]
        public void UpdatePlan_RejectsDuplicateUnversionedAndWrongRepositoryRoots()
        {
            SolutionUpdatePlan plan = new SolutionUpdatePlan();
            Uri repo = new Uri("https://example.invalid/svn/");
            Uri other = new Uri("https://other.invalid/svn/");

            Assert.That(
                plan.TryAddRoot(
                    @"C:\wc\one",
                    true,
                    false,
                    repo,
                    repo,
                    @"C:\wc"),
                Is.True);

            Assert.That(
                plan.TryAddRoot(
                    @"C:\wc\one",
                    true,
                    false,
                    repo,
                    repo,
                    @"C:\wc"),
                Is.False);

            Assert.That(
                plan.TryAddRoot(
                    @"C:\wc\unversioned",
                    false,
                    false,
                    repo,
                    repo,
                    @"C:\wc"),
                Is.False);

            Assert.That(
                plan.TryAddRoot(
                    @"D:\other\item",
                    true,
                    false,
                    repo,
                    other,
                    @"D:\other"),
                Is.False);

            Assert.That(plan.GroupCount, Is.EqualTo(1));
        }

        [Test]
        public void UpdatePlan_HeadUpdateAcceptsMultipleRepositories()
        {
            SolutionUpdatePlan plan = new SolutionUpdatePlan();

            Assert.That(
                plan.TryAddRoot(
                    @"C:\a\one",
                    true,
                    true,
                    new Uri("https://example.invalid/a/"),
                    new Uri("https://example.invalid/a/"),
                    @"C:\a"),
                Is.True);

            Assert.That(
                plan.TryAddRoot(
                    @"D:\b\two",
                    true,
                    true,
                    new Uri("https://example.invalid/a/"),
                    new Uri("https://example.invalid/b/"),
                    @"D:\b"),
                Is.True);

            Assert.That(plan.GroupCount, Is.EqualTo(2));
        }

        [Test]
        public void UpdatePlan_RejectsMissingWorkingCopyRootForAcceptedItem()
        {
            SolutionUpdatePlan plan = new SolutionUpdatePlan();

            Assert.Throws<ArgumentNullException>(
                () => plan.TryAddRoot(
                    @"C:\wc\one",
                    true,
                    true,
                    null,
                    null,
                    null));
        }
    }
}
