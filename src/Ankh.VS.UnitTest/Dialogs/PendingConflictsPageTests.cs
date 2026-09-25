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

using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Ankh;
using Ankh.Scc;
using Ankh.UI.PendingChanges;
using Ankh.UI.PendingChanges.Conflicts;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class PendingConflictsPageTests
    {
        [TestCase(PendingChangeKind.Conflicted, true)]
        [TestCase(PendingChangeKind.PropertyConflicted, true)]
        [TestCase(PendingChangeKind.TreeConflict, true)]
        [TestCase(PendingChangeKind.Modified, false)]
        [TestCase(PendingChangeKind.WrongCasing, false)]
        public void OnlyActualVersionPropertyAndTreeConflictsAppear(
            PendingChangeKind kind,
            bool expected)
        {
            Assert.That(PendingConflictLogic.IsConflict(kind), Is.EqualTo(expected));
        }

        [TestCase(0, 0, "No conflicts.")]
        [TestCase(1, 0, "1 conflict: 1 version conflict, 0 tree conflicts.")]
        [TestCase(0, 1, "1 conflict: 0 version conflicts, 1 tree conflict.")]
        [TestCase(2, 3, "5 conflicts: 2 version conflicts, 3 tree conflicts.")]
        public void ConflictHeaderReportsRealCounts(
            int version,
            int tree,
            string expected)
        {
            Assert.That(PendingConflictLogic.FormatHeader(version, tree), Is.EqualTo(expected));
        }

        [TestCase(PendingChangeKind.TreeConflict, false, false, "Tree", "Tree conflict")]
        [TestCase(PendingChangeKind.Conflicted, true, false, "Text", "Text conflict")]
        [TestCase(PendingChangeKind.Conflicted, false, true, "Property", "Property conflict")]
        [TestCase(PendingChangeKind.Conflicted, true, true, "Text + Property", "Text and property conflict")]
        [TestCase(PendingChangeKind.Conflicted, false, false, "Version", "Version conflict")]
        public void ConflictTypeAndDescriptionAreSpecific(
            PendingChangeKind kind,
            bool textConflict,
            bool propertyConflict,
            string expectedType,
            string expectedDescription)
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    PendingConflictLogic.GetConflictType(kind, textConflict, propertyConflict),
                    Is.EqualTo(expectedType));
                Assert.That(
                    PendingConflictLogic.GetConflictDescription(kind, textConflict, propertyConflict),
                    Is.EqualTo(expectedDescription));
            });
        }

        [Test]
        public void TreeConflictOnlyOffersWorkingCopyResolution()
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    PendingConflictLogic.CanExecuteResolution(
                        AnkhCommand.ItemResolveWorking, true, true, false, false),
                    Is.True);
                Assert.That(
                    PendingConflictLogic.CanExecuteResolution(
                        AnkhCommand.ItemResolveMerge, true, true, false, false),
                    Is.False);
                Assert.That(
                    PendingConflictLogic.CanExecuteResolution(
                        AnkhCommand.ItemResolveMineFull, true, true, false, false),
                    Is.False);
                Assert.That(
                    PendingConflictLogic.CanExecuteResolution(
                        AnkhCommand.ItemConflictEdit, true, true, false, false),
                    Is.False);
            });
        }

        [Test]
        public void TextConflictOffersEditorAndTextStrategies()
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    PendingConflictLogic.CanExecuteResolution(
                        AnkhCommand.ItemConflictEdit, true, false, true, true),
                    Is.True);
                Assert.That(
                    PendingConflictLogic.CanExecuteResolution(
                        AnkhCommand.ItemResolveMineConflict, true, false, true, true),
                    Is.True);
                Assert.That(
                    PendingConflictLogic.CanExecuteResolution(
                        AnkhCommand.ItemResolveTheirsConflict, true, false, true, true),
                    Is.True);
            });
        }

        [Test, Apartment(ApartmentState.STA)]
        public void PageStartsCollapsedAndContainsNoDesignerPlaceholderControls()
        {
            using (var page = new PendingConflictsPage())
            {
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var splitter = (SplitContainer)typeof(PendingConflictsPage)
                    .GetField("conflictEditSplitter", flags).GetValue(page);
                var top = (Label)typeof(PendingConflictsPage)
                    .GetField("resolveTopLabel", flags).GetValue(page);
                var bottom = (Label)typeof(PendingConflictsPage)
                    .GetField("resolveBottomLabel", flags).GetValue(page);

                Button[] buttons = Enumerable.Range(0, 8)
                    .Select(i => (Button)typeof(PendingConflictsPage)
                        .GetField("resolveButton" + i, flags).GetValue(page))
                    .ToArray();

                Assert.Multiple(() =>
                {
                    Assert.That(splitter.Panel2Collapsed, Is.True);
                    Assert.That(top.Text, Is.EqualTo("Conflict"));
                    Assert.That(bottom.Text, Does.Not.Contain("label3"));
                    Assert.That(bottom.Text, Does.Not.Contain("resolveBottomLabel"));
                    Assert.That(buttons.All(b => !string.IsNullOrWhiteSpace(b.Text)), Is.True);
                    Assert.That(buttons.All(b => b.Tag is AnkhCommand), Is.True);
                });
            }
        }
    }
}
