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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Ankh.UI.VSSelectionControls;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class SmartTreeViewThemeLogicTests
    {
        [TestCase(true, true, false, true)]
        [TestCase(false, true, false, false)]
        [TestCase(true, false, false, false)]
        [TestCase(true, true, true, false)]
        public void DarkNativeTreeThemeRequiresActiveDarkSurface(
            bool inVsTheming,
            bool darkSurface,
            bool highContrast,
            bool expected)
        {
            Assert.That(
                SmartTreeViewThemeLogic.ShouldUseDarkNativeTheme(
                    inVsTheming,
                    darkSurface,
                    highContrast),
                Is.EqualTo(expected));
        }

        [Test, Apartment(ApartmentState.STA)]
        public void RestoreNativeColorsReappliesManagedTreePalette()
        {
            const int TV_FIRST = 0x1100;
            const int TVM_SETBKCOLOR = TV_FIRST + 29;
            const int TVM_SETTEXTCOLOR = TV_FIRST + 30;
            const int TVM_GETBKCOLOR = TV_FIRST + 31;
            const int TVM_GETTEXTCOLOR = TV_FIRST + 32;

            using (var tree = new SmartTreeView())
            {
                Color expectedBack = Color.FromArgb(31, 31, 31);
                Color expectedFore = Color.FromArgb(241, 241, 241);
                tree.BackColor = expectedBack;
                tree.ForeColor = expectedFore;

                IntPtr handle = tree.Handle;

                SendMessage(handle, TVM_SETBKCOLOR, IntPtr.Zero,
                    (IntPtr)ColorTranslator.ToWin32(Color.White));
                SendMessage(handle, TVM_SETTEXTCOLOR, IntPtr.Zero,
                    (IntPtr)ColorTranslator.ToWin32(Color.Black));

                SmartTreeView.RestoreNativeColors(tree);

                Color actualBack = ColorTranslator.FromWin32(
                    unchecked((int)SendMessage(handle, TVM_GETBKCOLOR, IntPtr.Zero, IntPtr.Zero).ToInt64()));
                Color actualFore = ColorTranslator.FromWin32(
                    unchecked((int)SendMessage(handle, TVM_GETTEXTCOLOR, IntPtr.Zero, IntPtr.Zero).ToInt64()));

                Assert.That(actualBack.ToArgb(), Is.EqualTo(expectedBack.ToArgb()));
                Assert.That(actualFore.ToArgb(), Is.EqualTo(expectedFore.ToArgb()));
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
