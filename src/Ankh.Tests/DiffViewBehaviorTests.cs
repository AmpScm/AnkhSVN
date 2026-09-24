using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Ankh.Diff;
using Ankh.Diff.DiffUtils;
using Ankh.Diff.DiffUtils.Controls;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DiffViewBehaviorTests
    {
        static readonly string[] Original =
        {
            "alpha",
            "bravo",
            "charlie",
            "delta",
            "foxtrot"
        };

        static readonly string[] Changed =
        {
            "alpha",
            "BRAVO",
            "charlie",
            "echo",
            "delta",
            "foxtrot!"
        };

        static EditScript Script()
        {
            return new TextDiff(HashType.Unique, false, false).Execute(Original, Changed);
        }

        static DiffView CreateView()
        {
            var view = new DiffView
            {
                Width = 640,
                Height = 320
            };
            view.CreateControl();
            return view;
        }

        [Test]
        public void DiffViewLinesRepresentBothSidesAndTrackDiffRanges()
        {
            EditScript script = Script();
            var left = new DiffViewLines(Original, script, true);
            var right = new DiffViewLines(Changed, script, false);

            Assert.Multiple(() =>
            {
                Assert.That(left.Count, Is.GreaterThanOrEqualTo(Original.Length));
                Assert.That(right.Count, Is.GreaterThanOrEqualTo(Changed.Length));
                Assert.That(left.DiffStartLines.Length, Is.EqualTo(script.Count));
                Assert.That(left.DiffEndLines.Length, Is.EqualTo(script.Count));
                Assert.That(right.DiffStartLines.Length, Is.EqualTo(script.Count));
                Assert.That(right.DiffEndLines.Length, Is.EqualTo(script.Count));
                Assert.That(left.MaxLineNumber, Is.GreaterThanOrEqualTo(Original.Length - 1));
                Assert.That(right.MaxLineNumber, Is.GreaterThanOrEqualTo(Changed.Length - 1));
                Assert.That(left.LongestStringLength, Is.GreaterThan(0));
                Assert.That(right.LongestStringLength, Is.GreaterThan(0));
            });

            var pair = new DiffViewLines(
                new DiffViewLine("one\ttwo", 7, EditType.Change, true),
                new DiffViewLine("", -1, EditType.Insert, true));

            int before = pair.LongestStringLength;
            pair.RecheckLongestStringLength();

            Assert.Multiple(() =>
            {
                Assert.That(pair.Count, Is.EqualTo(2));
                Assert.That(pair[0].Edited, Is.True);
                Assert.That(pair[0].Number, Is.EqualTo(7));
                Assert.That(pair[0].EditType, Is.EqualTo(EditType.Change));
                Assert.That(pair.DiffStartLines, Is.Empty);
                Assert.That(pair.DiffEndLines, Is.Empty);
                Assert.That(pair.LongestStringLength, Is.EqualTo(before));
                Assert.That(pair.MaxLineNumber, Is.EqualTo(7));
            });
        }

        [Test]
        public void DiffNavigationMovesToFirstNextPreviousAndLastChanges()
        {
            using (DiffView view = CreateView())
            {
                view.SetData(Original, Script(), true);

                Assert.That(view.CanGoToFirstDiff, Is.True);
                Assert.That(view.GoToFirstDiff(), Is.True);
                int first = view.Position.Line;

                Assert.That(view.CanGoToNextDiff, Is.True);
                Assert.That(view.GoToNextDiff(), Is.True);
                int second = view.Position.Line;
                Assert.That(second, Is.GreaterThan(first));

                while (view.CanGoToNextDiff)
                    Assert.That(view.GoToNextDiff(), Is.True);

                int last = view.Position.Line;
                Assert.That(view.CanGoToLastDiff, Is.False);
                Assert.That(view.GoToLastDiff(), Is.False);

                Assert.That(view.CanGoToPreviousDiff, Is.True);
                Assert.That(view.GoToPreviousDiff(), Is.True);
                Assert.That(view.Position.Line, Is.LessThan(last));

                view.Position = new DiffViewPosition(0, 0);
                Assert.That(view.GoToLastDiff(), Is.True);
                Assert.That(view.Position.Line, Is.EqualTo(view.Lines.DiffStartLines[view.Lines.DiffStartLines.Length - 1]));
            }
        }

        [Test]
        public void GoToLineAndPositionClampingCoverValidAndInvalidLocations()
        {
            using (DiffView view = CreateView())
            {
                view.SetData(Original, Script(), true);

                Assert.Multiple(() =>
                {
                    Assert.That(view.GoToLine(-1), Is.False);
                    Assert.That(view.GoToLine(Original.Length), Is.False);
                    Assert.That(view.GoToLine(2), Is.True);
                    Assert.That(view.Position.Line, Is.GreaterThanOrEqualTo(2));
                });

                view.Position = new DiffViewPosition(999, 999);
                Assert.Multiple(() =>
                {
                    Assert.That(view.Position.Line, Is.EqualTo(view.LineCount - 1));
                    Assert.That(view.Position.Column, Is.EqualTo(view.Lines[view.LineCount - 1].Text.Length));
                });

                view.Position = new DiffViewPosition(-10, -10);
                Assert.That(view.Position, Is.EqualTo(new DiffViewPosition(0, 0)));
            }
        }

        [Test]
        public void FindNextAndPreviousSupportCaseInsensitiveAndWrappingSearch()
        {
            using (DiffView view = CreateView())
            {
                view.SetData(Original, Script(), true);

                var forward = new FindData
                {
                    Text = "BRAVO",
                    MatchCase = false,
                    SearchUp = false
                };

                Assert.That(view.FindNext(forward), Is.True);
                Assert.That(view.SelectedText, Is.EqualTo("BRAVO").IgnoreCase);

                view.Position = new DiffViewPosition(view.LineCount - 1, view.Lines[view.LineCount - 1].Text.Length);
                var wrappedForward = new FindData
                {
                    Text = "alpha",
                    MatchCase = true
                };
                Assert.That(view.FindNext(wrappedForward), Is.True);
                Assert.That(view.Position.Line, Is.Zero);

                view.Position = new DiffViewPosition(0, 0);
                var wrappedBackward = new FindData
                {
                    Text = "foxtrot",
                    MatchCase = false,
                    SearchUp = true
                };
                Assert.That(view.FindPrevious(wrappedBackward), Is.True);
                Assert.That(view.Position.Line, Is.GreaterThan(0));
            }
        }

        [Test]
        public void ForwardAndReverseSelectionsProduceTheSameSelectedText()
        {
            using (DiffView view = CreateView())
            {
                view.SetData(Original, Script(), true);

                view.Position = new DiffViewPosition(0, 1);
                SetSelectionEnd(view, 1, 3);
                string forward = view.SelectedText;

                // Move away first so assigning the reverse anchor clears the existing selection.
                view.Position = new DiffViewPosition(2, 0);
                view.Position = new DiffViewPosition(1, 3);
                SetSelectionEnd(view, 0, 1);
                string reverse = view.SelectedText;

                Assert.Multiple(() =>
                {
                    Assert.That(forward, Is.EqualTo("lpha\r\nbra"));
                    Assert.That(reverse, Is.EqualTo(forward));
                    Assert.That(view.HasSelection, Is.True);
                });

                view.Position = new DiffViewPosition(2, 0);
                Assert.That(view.HasSelection, Is.False);
                Assert.That(view.SelectedText, Is.Empty);
            }
        }

        [Test]
        public void SingleLineSelectionAndEventsTrackStateChanges()
        {
            using (DiffView view = CreateView())
            {
                int linesChanged = 0;
                int positionsChanged = 0;
                int selectionsChanged = 0;

                view.LinesChanged += delegate { linesChanged++; };
                view.PositionChanged += delegate { positionsChanged++; };
                view.SelectionChanged += delegate { selectionsChanged++; };

                view.SetData(Original, Script(), true);
                view.Position = new DiffViewPosition(1, 1);
                SetSelectionEnd(view, 1, 4);

                MethodInfo selectedMethod = typeof(DiffView).GetMethod(
                    "GetSingleLineSelectedText",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                object[] args = { null };
                bool selected = (bool)selectedMethod.Invoke(view, args);

                Assert.Multiple(() =>
                {
                    Assert.That(selected, Is.True);
                    Assert.That((string)args[0], Is.EqualTo("rav"));
                    Assert.That(linesChanged, Is.GreaterThan(0));
                    Assert.That(positionsChanged, Is.GreaterThan(0));
                    Assert.That(selectionsChanged, Is.GreaterThan(0));
                });
            }
        }

        [Test]
        public void DisplayPropertiesExerciseRepeatedAndChangedValues()
        {
            using (DiffView view = CreateView())
            {
                Assert.That(view.ShowWhitespace, Is.False);
                view.ShowWhitespace = true;
                view.ShowWhitespace = true;
                Assert.That(view.ShowWhitespace, Is.True);

                BorderStyle original = view.BorderStyle;
                view.BorderStyle = original;
                view.BorderStyle = BorderStyle.FixedSingle;
                Assert.That(view.BorderStyle, Is.EqualTo(BorderStyle.FixedSingle));

                Assert.That(view.LineCount, Is.Zero);
                Assert.That(view.CanGoToFirstDiff, Is.False);
                Assert.That(view.CanGoToNextDiff, Is.False);
                Assert.That(view.CanGoToPreviousDiff, Is.False);
                Assert.That(view.CanGoToLastDiff, Is.False);
            }
        }

        [Test]
        public void DiffViewPositionComparisonOperatorsCoverLineAndColumnOrdering()
        {
            var a = new DiffViewPosition(1, 2);
            var b = new DiffViewPosition(1, 3);
            var c = new DiffViewPosition(2, 0);

            Assert.Multiple(() =>
            {
                Assert.That(a < b, Is.True);
                Assert.That(b > a, Is.True);
                Assert.That(a <= b, Is.True);
                Assert.That(b >= a, Is.True);
                Assert.That(a != b, Is.True);
                Assert.That(a == new DiffViewPosition(1, 2), Is.True);
                Assert.That(b < c, Is.True);
                Assert.That(a.CompareTo(a), Is.Zero);
                Assert.That(a.Equals(new DiffViewPosition(1, 2)), Is.True);
            });
        }

        static void SetSelectionEnd(DiffView view, int line, int column)
        {
            MethodInfo method = typeof(DiffView).GetMethod(
                "SetSelectionEnd",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(view, new object[] { line, column });
        }
    }
}
