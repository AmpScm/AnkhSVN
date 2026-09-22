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

using Ankh.UI.VSSelectionControls;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class SmartListViewThemeLogicTests
    {
        [TestCase(false, true, false, true)]
        [TestCase(false, false, false, false)]
        [TestCase(true, true, false, false)]
        [TestCase(false, true, true, false)]
        public void OwnerDrawHeaderWhenPaletteAvailable(
            bool themeCancelled,
            bool hasPalette,
            bool highContrast,
            bool expected)
        {
            Assert.That(
                SmartListViewThemeLogic.ShouldOwnerDrawHeader(
                    themeCancelled,
                    hasPalette,
                    highContrast),
                Is.EqualTo(expected));
        }

        [TestCase(true, true, false, true)]
        [TestCase(true, false, false, false)]
        [TestCase(false, true, false, false)]
        [TestCase(true, true, true, false)]
        public void DarkNativeThemeOnlyForDarkVsSurfaces(
            bool inVsTheming,
            bool darkSurface,
            bool highContrast,
            bool expected)
        {
            Assert.That(
                SmartListViewThemeLogic.ShouldUseDarkNativeTheme(
                    inVsTheming,
                    darkSurface,
                    highContrast),
                Is.EqualTo(expected));
        }
    }
}
