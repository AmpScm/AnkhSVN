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

using Ankh.UI;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class AnkhThemePaletteTests
    {
        [TestCase(30, 30, 30, true)]
        [TestCase(245, 245, 245, false)]
        [TestCase(127, 127, 127, true)]
        [TestCase(128, 128, 128, false)]
        [TestCase(0, 120, 215, true)]
        public void DarknessComesFromSurfaceLuminance(
            int red,
            int green,
            int blue,
            bool expected)
        {
            Assert.That(
                AnkhThemePalette.IsDark(Color.FromArgb(red, green, blue)),
                Is.EqualTo(expected));
        }

        [Test]
        public void BlendProducesNeutralColorFromActualThemePair()
        {
            Color result = AnkhThemePalette.Blend(
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(0, 0, 0),
                0.5);

            Assert.That(result, Is.EqualTo(Color.FromArgb(128, 128, 128)));
        }

        [TestCase(-0.01)]
        [TestCase(1.01)]
        [TestCase(double.NaN)]
        public void BlendRejectsInvalidWeights(double weight)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                delegate
                {
                    AnkhThemePalette.Blend(Color.White, Color.Black, weight);
                });
        }

        [Test]
        public void ContrastMeasuresActualColorPair()
        {
            Assert.That(AnkhThemePalette.ContrastRatio(Color.Black, Color.White), Is.EqualTo(21).Within(0.001));
            Assert.That(AnkhThemePalette.ContrastRatio(Color.Gray, Color.Gray), Is.EqualTo(1));
            Assert.That(AnkhThemePalette.ContrastRatio(Color.DarkBlue, Color.FromArgb(30, 30, 30)), Is.LessThan(4.5));
            Assert.That(AnkhThemePalette.ContrastRatio(Color.DarkBlue, Color.FromArgb(245, 245, 245)), Is.GreaterThan(4.5));
        }

        [Test]
        public void ReadableForegroundKeepsThemeSafeAccent()
        {
            Color background = Color.FromArgb(30, 30, 30);
            Color accent = Color.FromArgb(190, 190, 190);
            Color fallback = Color.White;

            Assert.That(
                AnkhThemePalette.ResolveReadableForeground(
                    accent,
                    fallback,
                    background),
                Is.EqualTo(accent));
        }

        [Test]
        public void ReadableForegroundFallsBackFromUnsafeOrEmptyColor()
        {
            Color background = Color.FromArgb(30, 30, 30);
            Color fallback = Color.FromArgb(241, 241, 241);

            Assert.That(
                AnkhThemePalette.ResolveReadableForeground(
                    Color.Black,
                    fallback,
                    background),
                Is.EqualTo(fallback));

            Assert.That(
                AnkhThemePalette.ResolveReadableForeground(
                    Color.Empty,
                    fallback,
                    background),
                Is.EqualTo(fallback));
        }
    }
}
