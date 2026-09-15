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

using Ankh.UI.Annotate;
using NUnit.Framework;

namespace Ankh.Tests.Annotate
{
    [TestFixture]
    public class AnnotationLayoutCalculatorTests
    {
        [Test]
        public void FullyVisibleRegionKeepsEditorGeometry()
        {
            AnnotationRegionLayout result = AnnotationLayoutCalculator.Calculate(100, 140, 80, 100);

            Assert.IsTrue(result.IsVisible);
            Assert.AreEqual(20, result.Top, 0.001);
            Assert.AreEqual(40, result.Height, 0.001);
        }

        [Test]
        public void RegionIsClippedAtTopOfViewport()
        {
            AnnotationRegionLayout result = AnnotationLayoutCalculator.Calculate(60, 100, 80, 100);

            Assert.IsTrue(result.IsVisible);
            Assert.AreEqual(0, result.Top, 0.001);
            Assert.AreEqual(20, result.Height, 0.001);
        }

        [Test]
        public void RegionIsClippedAtBottomOfViewport()
        {
            AnnotationRegionLayout result = AnnotationLayoutCalculator.Calculate(150, 210, 80, 100);

            Assert.IsTrue(result.IsVisible);
            Assert.AreEqual(70, result.Top, 0.001);
            Assert.AreEqual(30, result.Height, 0.001);
        }

        [TestCase(20, 60, 80, 100)]
        [TestCase(180, 220, 80, 100)]
        public void RegionOutsideViewportIsHidden(double top, double bottom, double viewportTop, double viewportHeight)
        {
            AnnotationRegionLayout result = AnnotationLayoutCalculator.Calculate(top, bottom, viewportTop, viewportHeight);

            Assert.IsFalse(result.IsVisible);
            Assert.AreEqual(0, result.Height, 0.001);
        }

        [Test]
        public void LayoutTracksChangedLineHeightForZoomOrDpiChanges()
        {
            AnnotationRegionLayout normal = AnnotationLayoutCalculator.Calculate(100, 148, 100, 200);
            AnnotationRegionLayout scaled = AnnotationLayoutCalculator.Calculate(100, 196, 100, 200);

            Assert.IsTrue(normal.IsVisible);
            Assert.IsTrue(scaled.IsVisible);
            Assert.AreEqual(48, normal.Height, 0.001);
            Assert.AreEqual(96, scaled.Height, 0.001);
        }

        [Test]
        public void InvalidViewportDoesNotProduceVisibleGeometry()
        {
            AnnotationRegionLayout result = AnnotationLayoutCalculator.Calculate(100, 140, 80, 0);

            Assert.IsFalse(result.IsVisible);
            Assert.AreEqual(0, result.Top, 0.001);
            Assert.AreEqual(0, result.Height, 0.001);
        }
    }
}
