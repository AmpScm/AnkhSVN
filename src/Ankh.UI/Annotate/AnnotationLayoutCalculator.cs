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

namespace Ankh.UI.Annotate
{
    public struct AnnotationRegionLayout
    {
        public AnnotationRegionLayout(bool isVisible, double top, double height)
        {
            IsVisible = isVisible;
            Top = top;
            Height = height;
        }

        public bool IsVisible { get; }
        public double Top { get; }
        public double Height { get; }
    }

    public static class AnnotationLayoutCalculator
    {
        public static AnnotationRegionLayout Calculate(
            double regionTop,
            double regionBottom,
            double viewportTop,
            double viewportHeight)
        {
            if (double.IsNaN(regionTop) || double.IsNaN(regionBottom) ||
                double.IsNaN(viewportTop) || double.IsNaN(viewportHeight) ||
                double.IsInfinity(regionTop) || double.IsInfinity(regionBottom) ||
                double.IsInfinity(viewportTop) || double.IsInfinity(viewportHeight) ||
                viewportHeight <= 0 || regionBottom <= regionTop)
            {
                return new AnnotationRegionLayout(false, 0, 0);
            }

            double top = regionTop - viewportTop;
            double bottom = regionBottom - viewportTop;

            if (bottom <= 0 || top >= viewportHeight)
                return new AnnotationRegionLayout(false, 0, 0);

            double clippedTop = Math.Max(0, top);
            double clippedBottom = Math.Min(viewportHeight, bottom);
            double height = clippedBottom - clippedTop;

            if (height <= 0)
                return new AnnotationRegionLayout(false, 0, 0);

            return new AnnotationRegionLayout(true, clippedTop, height);
        }
    }
}
