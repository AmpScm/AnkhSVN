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

using System.Windows.Forms;

using Ankh.Diff.DiffUtils.Controls;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Diff
{
    [TestFixture]
    public class DiffViewKeyLogicTests
    {
        [Test]
        public void GetAction_CopiesOnlyForCtrlCWithSelection()
        {
            AssertAction(
                Action(Keys.C, Keys.Control, true),
                DiffViewKeyOperation.CopySelection);

            AssertAction(
                Action(Keys.C, Keys.Control, false),
                DiffViewKeyOperation.None);

            AssertAction(
                Action(Keys.C, Keys.Shift, true),
                DiffViewKeyOperation.None);

            AssertAction(
                Action(Keys.C, Keys.Control | Keys.Shift, true),
                DiffViewKeyOperation.None);
        }

        [TestCase(Keys.Up, Keys.None, -1, 0, false)]
        [TestCase(Keys.Down, Keys.None, 1, 0, false)]
        [TestCase(Keys.Left, Keys.None, 0, -1, false)]
        [TestCase(Keys.Right, Keys.None, 0, 1, false)]
        [TestCase(Keys.Up, Keys.Control, -1, 0, true)]
        [TestCase(Keys.Down, Keys.Control, 1, 0, true)]
        public void GetAction_MapsMovementKeys(
            Keys key,
            Keys modifiers,
            int lineDelta,
            int columnDelta,
            bool scrollVertically)
        {
            DiffViewKeyAction action = Action(key, modifiers);

            AssertAction(action, DiffViewKeyOperation.OffsetPosition);
            Assert.That(action.LineDelta, Is.EqualTo(lineDelta));
            Assert.That(action.ColumnDelta, Is.EqualTo(columnDelta));
            Assert.That(action.ScrollVertically, Is.EqualTo(scrollVertically));
            Assert.That(action.PageFactor, Is.Zero);
        }

        [TestCase(Keys.Up, -1, 0)]
        [TestCase(Keys.Down, 1, 0)]
        [TestCase(Keys.Left, 0, -1)]
        [TestCase(Keys.Right, 0, 1)]
        public void GetAction_ShiftExtendsSelection(
            Keys key,
            int lineDelta,
            int columnDelta)
        {
            DiffViewKeyAction action = Action(key, Keys.Shift);

            AssertAction(action, DiffViewKeyOperation.ExtendSelection);
            Assert.That(action.LineDelta, Is.EqualTo(lineDelta));
            Assert.That(action.ColumnDelta, Is.EqualTo(columnDelta));
            Assert.That(action.ScrollVertically, Is.False);
        }

        [TestCase(Keys.PageUp, Keys.None, -1, true, "OffsetPosition")]
        [TestCase(Keys.PageDown, Keys.None, 1, true, "OffsetPosition")]
        [TestCase(Keys.PageUp, Keys.Shift, -1, false, "ExtendSelection")]
        [TestCase(Keys.PageDown, Keys.Shift, 1, false, "ExtendSelection")]
        public void GetAction_MapsPageMovement(
            Keys key,
            Keys modifiers,
            int pageFactor,
            bool scrollVertically,
            string operation)
        {
            DiffViewKeyAction action = Action(key, modifiers);

            Assert.That(action.Operation.ToString(), Is.EqualTo(operation));
            Assert.That(action.PageFactor, Is.EqualTo(pageFactor));
            Assert.That(action.ScrollVertically, Is.EqualTo(scrollVertically));
            Assert.That(action.LineDelta, Is.Zero);
        }

        [Test]
        public void GetAction_MapsHomeVariants()
        {
            DiffViewKeyAction ctrl = Action(Keys.Home, Keys.Control);
            AssertAction(ctrl, DiffViewKeyOperation.SetPosition);
            Assert.That(ctrl.TargetLine, Is.Zero);
            Assert.That(ctrl.TargetColumn, Is.Zero);

            DiffViewKeyAction shift = Action(Keys.Home, Keys.Shift);
            AssertAction(shift, DiffViewKeyOperation.ExtendSelection);
            Assert.That(shift.LineDelta, Is.Zero);
            Assert.That(shift.ColumnDelta, Is.EqualTo(-7));

            DiffViewKeyAction normal = Action(Keys.Home, Keys.None);
            AssertAction(normal, DiffViewKeyOperation.SetPosition);
            Assert.That(normal.TargetLine, Is.EqualTo(4));
            Assert.That(normal.TargetColumn, Is.Zero);
        }

        [Test]
        public void GetAction_MapsEndVariants()
        {
            DiffViewKeyAction ctrl = Action(Keys.End, Keys.Control);
            AssertAction(ctrl, DiffViewKeyOperation.SetPosition);
            Assert.That(ctrl.TargetLine, Is.EqualTo(12));
            Assert.That(ctrl.TargetColumn, Is.EqualTo(31));

            DiffViewKeyAction shift = Action(Keys.End, Keys.Shift);
            AssertAction(shift, DiffViewKeyOperation.ExtendSelection);
            Assert.That(shift.LineDelta, Is.Zero);
            Assert.That(shift.ColumnDelta, Is.EqualTo(13));

            DiffViewKeyAction normal = Action(Keys.End, Keys.None);
            AssertAction(normal, DiffViewKeyOperation.SetPosition);
            Assert.That(normal.TargetLine, Is.EqualTo(4));
            Assert.That(normal.TargetColumn, Is.EqualTo(20));
        }

        [TestCase(Keys.Up, Keys.Control | Keys.Shift)]
        [TestCase(Keys.Down, Keys.Control | Keys.Shift)]
        [TestCase(Keys.Left, Keys.Control)]
        [TestCase(Keys.Right, Keys.Control)]
        [TestCase(Keys.PageUp, Keys.Control)]
        [TestCase(Keys.PageDown, Keys.Control)]
        [TestCase(Keys.Home, Keys.Control | Keys.Shift)]
        [TestCase(Keys.End, Keys.Control | Keys.Shift)]
        [TestCase(Keys.F5, Keys.None)]
        public void GetAction_IgnoresUnsupportedModifierCombinations(Keys key, Keys modifiers)
        {
            AssertAction(Action(key, modifiers), DiffViewKeyOperation.None);
        }

        static DiffViewKeyAction Action(
            Keys key,
            Keys modifiers = Keys.None,
            bool hasSelection = false)
        {
            return DiffViewKeyLogic.GetAction(
                key,
                modifiers,
                hasSelection,
                4,
                7,
                12,
                20,
                31);
        }

        static void AssertAction(
            DiffViewKeyAction action,
            DiffViewKeyOperation operation)
        {
            Assert.That(action, Is.Not.Null);
            Assert.That(action.Operation, Is.EqualTo(operation));
        }
    }
}
