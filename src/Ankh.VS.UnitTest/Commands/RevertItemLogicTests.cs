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

using Ankh.Commands;
using NUnit.Framework;
using SharpSvn;

namespace AnkhSvn_UnitTestProject.Commands
{
    [TestFixture]
    public class RevertItemLogicTests
    {
        [TestCase(true, false, false, false, true)]
        [TestCase(false, true, true, false, true)]
        [TestCase(false, false, false, true, true)]
        [TestCase(false, false, true, false, false)]
        [TestCase(false, true, false, false, false)]
        public void ShouldIncludePrimary_CoversModifiedDirtyAndConflictRules(
            bool modified,
            bool versioned,
            bool dirty,
            bool conflicted,
            bool expected)
        {
            Assert.That(
                RevertItemLogic.ShouldIncludePrimary(
                    modified,
                    versioned,
                    dirty,
                    conflicted),
                Is.EqualTo(expected));
        }

        [TestCase(true, false, false, true)]
        [TestCase(false, true, true, true)]
        [TestCase(false, false, true, false)]
        [TestCase(false, true, false, false)]
        public void ShouldIncludeDescendant_RequiresARealWorkingCopyChange(
            bool modified,
            bool versioned,
            bool dirty,
            bool expected)
        {
            Assert.That(
                RevertItemLogic.ShouldIncludeDescendant(
                    modified,
                    versioned,
                    dirty),
                Is.EqualTo(expected));
        }

        [Test]
        public void CompareForRevert_PrioritizesAddsAndReplacements()
        {
            Assert.That(
                RevertItemLogic.CompareForRevert(
                    true, @"C:\wc\child",
                    false, @"C:\wc\parent"),
                Is.LessThan(0));

            Assert.That(
                RevertItemLogic.CompareForRevert(
                    false, @"C:\wc\parent",
                    true, @"C:\wc\child"),
                Is.GreaterThan(0));
        }

        [Test]
        public void CompareForRevert_UsesReversePathOrderForAddsAndNormalOrderOtherwise()
        {
            Assert.That(
                RevertItemLogic.CompareForRevert(
                    true, @"C:\wc\parent",
                    true, @"C:\wc\parent\child"),
                Is.GreaterThan(0),
                "Added/replaced children sort before their parents.");

            Assert.That(
                RevertItemLogic.CompareForRevert(
                    false, @"C:\wc\parent",
                    false, @"C:\wc\parent\child"),
                Is.LessThan(0));

            Assert.That(
                RevertItemLogic.CompareForRevert(
                    false, @"C:\WC\A",
                    false, @"c:\wc\a"),
                Is.Zero,
                "Revert ordering remains case-insensitive.");
        }

        [TestCase(SvnStatus.None)]
        [TestCase(SvnStatus.Normal)]
        [TestCase(SvnStatus.Ignored)]
        [TestCase(SvnStatus.External)]
        [TestCase(SvnStatus.NotVersioned)]
        public void HasBlockingChildChange_AllowsBenignNodeStates(SvnStatus nodeStatus)
        {
            Assert.That(
                RevertItemLogic.HasBlockingChildChange(
                    false,
                    SvnStatus.Normal,
                    nodeStatus),
                Is.False);
        }

        [TestCase(SvnStatus.Modified)]
        [TestCase(SvnStatus.Added)]
        [TestCase(SvnStatus.Deleted)]
        [TestCase(SvnStatus.Replaced)]
        public void HasBlockingChildChange_BlocksChangedNodeStates(SvnStatus nodeStatus)
        {
            Assert.That(
                RevertItemLogic.HasBlockingChildChange(
                    false,
                    SvnStatus.Normal,
                    nodeStatus),
                Is.True);
        }

        [Test]
        public void HasBlockingChildChange_BlocksConflictsAndPropertyChanges()
        {
            Assert.That(
                RevertItemLogic.HasBlockingChildChange(
                    true,
                    SvnStatus.Normal,
                    SvnStatus.Normal),
                Is.True);

            Assert.That(
                RevertItemLogic.HasBlockingChildChange(
                    false,
                    SvnStatus.Modified,
                    SvnStatus.Normal),
                Is.True);

            Assert.That(
                RevertItemLogic.HasBlockingChildChange(
                    false,
                    SvnStatus.None,
                    SvnStatus.Normal),
                Is.False);
        }
    }
}
