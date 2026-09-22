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

using System.Drawing;

using Ankh.WpfPackage.Services;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class WinFormsNativeThemeLogicTests
    {
        [TestCase(30, 30, 30, false, true)]
        [TestCase(245, 245, 245, false, false)]
        [TestCase(30, 30, 30, true, false)]
        [TestCase(128, 128, 128, false, false)]
        public void DarkNativeThemeUsesSurfaceLuminanceAndHonorsHighContrast(
            int red,
            int green,
            int blue,
            bool highContrast,
            bool expected)
        {
            Assert.That(
                WinFormsNativeThemeLogic.ShouldUseDarkTheme(
                    Color.FromArgb(red, green, blue),
                    highContrast),
                Is.EqualTo(expected));
        }

        [Test]
        public void NativeThemeClassNamesMatchSupportedWindowsDarkSubThemes()
        {
            Assert.That(WinFormsNativeThemeLogic.DarkExplorerTheme, Is.EqualTo("DarkMode_Explorer"));
            Assert.That(WinFormsNativeThemeLogic.DarkComboTheme, Is.EqualTo("DarkMode_CFD"));
            Assert.That(WinFormsNativeThemeLogic.DarkItemsViewTheme, Is.EqualTo("DarkMode_ItemsView"));
        }
    }
}
