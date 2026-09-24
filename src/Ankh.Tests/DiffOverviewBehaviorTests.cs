using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Ankh.Diff.DiffUtils;
using Ankh.Diff.DiffUtils.Controls;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DiffOverviewBehaviorTests
    {
        static readonly string[] Left = { "one", "two", "three", "four" };
        static readonly string[] Right = { "one", "TWO", "three", "five", "four" };

        static DiffView CreateDiffView()
        {
            var view = new DiffView
            {
                Width = 500,
                Height = 200
            };
            view.CreateControl();
            EditScript script = new TextDiff(HashType.Unique, false, false).Execute(Left, Right);
            view.SetData(Left, script, true);
            return view;
        }

        [Test]
        public void DefaultsAndAttachDetachReflectDiffViewState()
        {
            using (var overview = new TestDiffOverview())
            {
                overview.Width = 80;
                overview.Height = 200;

                Assert.Multiple(() =>
                {
                    Assert.That(overview.DiffView, Is.Null);
                    Assert.That(overview.LineCount, Is.Zero);
                    Assert.That(overview.ViewLineCount, Is.Zero);
                    Assert.That(overview.ViewTopLine, Is.Zero);
                    Assert.That(overview.UseTranslucentView, Is.True);
                });

                using (DiffView view = CreateDiffView())
                {
                    overview.DiffView = view;

                    Assert.Multiple(() =>
                    {
                        Assert.That(overview.DiffView, Is.SameAs(view));
                        Assert.That(overview.LineCount, Is.EqualTo(view.LineCount));
                        Assert.That(overview.ViewLineCount, Is.EqualTo(view.VisibleLineCount));
                        Assert.That(overview.ViewTopLine, Is.EqualTo(view.FirstVisibleLine));
                    });

                    overview.DiffView = view;
                    overview.DiffView = null;

                    Assert.That(overview.LineCount, Is.Zero);
                    Assert.That(overview.ViewLineCount, Is.Zero);
                    Assert.That(overview.ViewTopLine, Is.Zero);
                }
            }
        }

        [Test]
        public void DisplayPropertiesHandleRepeatedAndChangedValues()
        {
            using (var overview = new TestDiffOverview())
            {
                overview.Width = 80;
                overview.Height = 200;

                overview.UseTranslucentView = true;
                overview.UseTranslucentView = false;
                overview.UseTranslucentView = false;
                Assert.That(overview.UseTranslucentView, Is.False);

                BorderStyle initial = overview.BorderStyle;
                overview.BorderStyle = initial;
                overview.BorderStyle = BorderStyle.FixedSingle;
                Assert.That(overview.BorderStyle, Is.EqualTo(BorderStyle.FixedSingle));
            }
        }

        [Test]
        public void MouseDragMapsTopMiddleAndBottomToBoundedLines()
        {
            using (var overview = new TestDiffOverview())
            using (DiffView view = CreateDiffView())
            {
                overview.Width = 100;
                overview.Height = 100;
                overview.CreateControl();
                overview.DiffView = view;

                int lastLine = -1;
                int eventCount = 0;
                overview.LineClick += delegate(object sender, DiffLineClickEventArgs e)
                {
                    lastLine = e.Line;
                    eventCount++;
                };

                overview.MouseDownForTest(MouseButtons.Left, 10, -10);
                Assert.That(lastLine, Is.Zero);

                overview.MouseMoveForTest(MouseButtons.Left, 10, 50);
                Assert.That(lastLine, Is.InRange(0, overview.LineCount - 1));

                overview.MouseMoveForTest(MouseButtons.Left, 10, 200);
                Assert.That(lastLine, Is.EqualTo(overview.LineCount - 1));

                overview.MouseUpForTest(MouseButtons.Left, 10, 200);
                int afterUp = eventCount;
                overview.MouseMoveForTest(MouseButtons.Left, 10, 25);
                Assert.That(eventCount, Is.EqualTo(afterUp));

                overview.MouseDownForTest(MouseButtons.Right, 10, 50);
                Assert.That(eventCount, Is.EqualTo(afterUp));
            }
        }

        [Test]
        public void MouseOperationsWithoutViewAreNoOps()
        {
            using (var overview = new TestDiffOverview())
            {
                overview.Width = 100;
                overview.Height = 100;

                Assert.DoesNotThrow(() =>
                {
                    overview.MouseDownForTest(MouseButtons.Left, 10, 10);
                    overview.MouseMoveForTest(MouseButtons.Left, 10, 20);
                    overview.MouseUpForTest(MouseButtons.Left, 10, 20);
                });
            }
        }

        [Test]
        public void PaintingCoversEmptyImageTranslucentAndOutlineModes()
        {
            using (var overview = new TestDiffOverview())
            {
                overview.Width = 100;
                overview.Height = 120;
                overview.CreateControl();

                using (var bitmap = new Bitmap(100, 120))
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    Assert.DoesNotThrow(() => overview.PaintForTest(graphics));
                }

                using (DiffView view = CreateDiffView())
                {
                    overview.DiffView = view;

                    using (var bitmap = new Bitmap(100, 120))
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        overview.UseTranslucentView = true;
                        Assert.DoesNotThrow(() => overview.PaintForTest(graphics));

                        overview.UseTranslucentView = false;
                        Assert.DoesNotThrow(() => overview.PaintForTest(graphics));
                    }
                }
            }
        }

        [Test]
        public void DiffViewEventsKeepOverviewSynchronized()
        {
            using (var overview = new TestDiffOverview())
            using (DiffView view = CreateDiffView())
            {
                overview.Width = 100;
                overview.Height = 180;
                overview.DiffView = view;

                int lineCount = overview.LineCount;
                view.Height = 80;
                Assert.That(overview.ViewLineCount, Is.EqualTo(view.VisibleLineCount));

                view.SetData(
                    new[] { "a", "b" },
                    new TextDiff(HashType.Unique, false, false).Execute(
                        new[] { "a", "b" },
                        new[] { "a", "B" }),
                    true);

                Assert.That(overview.LineCount, Is.EqualTo(view.LineCount));
                Assert.That(overview.LineCount, Is.Not.EqualTo(lineCount));

                int before = overview.ViewTopLine;
                view.FirstVisibleLine = Math.Min(1, Math.Max(0, view.LineCount - 1));
                Assert.That(overview.ViewTopLine, Is.EqualTo(view.FirstVisibleLine));
            }
        }

        sealed class TestDiffOverview : DiffOverview
        {
            public void MouseDownForTest(MouseButtons button, int x, int y)
            {
                OnMouseDown(new MouseEventArgs(button, 1, x, y, 0));
            }

            public void MouseMoveForTest(MouseButtons button, int x, int y)
            {
                OnMouseMove(new MouseEventArgs(button, 0, x, y, 0));
            }

            public void MouseUpForTest(MouseButtons button, int x, int y)
            {
                OnMouseUp(new MouseEventArgs(button, 1, x, y, 0));
            }

            public void PaintForTest(Graphics graphics)
            {
                OnPaint(new PaintEventArgs(graphics, ClientRectangle));
            }
        }
    }
}
