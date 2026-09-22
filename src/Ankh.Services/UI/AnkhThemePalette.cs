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

namespace Ankh.UI
{
    /// <summary>
    /// Semantic colors used by Ankh WinForms surfaces.
    /// </summary>
    public sealed class AnkhThemePalette
    {
        public AnkhThemePalette(
            Color surfaceBackground,
            Color surfaceForeground,
            Color inputBackground,
            Color inputForeground,
            Color disabledText,
            Color border,
            Color selectionBackground,
            Color selectionForeground,
            Color hoverBackground,
            Color pressedBackground,
            Color focusBorder)
        {
            SurfaceBackground = surfaceBackground;
            SurfaceForeground = surfaceForeground;
            InputBackground = inputBackground;
            InputForeground = inputForeground;
            DisabledText = disabledText;
            Border = border;
            SelectionBackground = selectionBackground;
            SelectionForeground = selectionForeground;
            HoverBackground = hoverBackground;
            PressedBackground = pressedBackground;
            FocusBorder = focusBorder;
        }

        public Color SurfaceBackground { get; private set; }
        public Color SurfaceForeground { get; private set; }
        public Color InputBackground { get; private set; }
        public Color InputForeground { get; private set; }
        public Color DisabledText { get; private set; }
        public Color Border { get; private set; }
        public Color SelectionBackground { get; private set; }
        public Color SelectionForeground { get; private set; }
        public Color HoverBackground { get; private set; }
        public Color PressedBackground { get; private set; }
        public Color FocusBorder { get; private set; }

        public Color SecondaryText
        {
            get { return Blend(SurfaceForeground, SurfaceBackground, 0.70); }
        }

        public static double ContrastRatio(Color foreground, Color background)
        {
            double first = RelativeLuminance(foreground);
            double second = RelativeLuminance(background);
            return (Math.Max(first, second) + 0.05) / (Math.Min(first, second) + 0.05);
        }

        static double RelativeLuminance(Color color)
        {
            return 0.2126 * LinearChannel(color.R)
                + 0.7152 * LinearChannel(color.G)
                + 0.0722 * LinearChannel(color.B);
        }

        static double LinearChannel(byte channel)
        {
            double value = channel / 255.0;
            return value <= 0.04045 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
        }

        public bool IsDarkSurface
        {
            get { return IsDark(SurfaceBackground); }
        }

        public static bool IsDark(Color background)
        {
            double luminance =
                (0.2126 * background.R) +
                (0.7152 * background.G) +
                (0.0722 * background.B);

            return luminance < 128.0;
        }

        /// <summary>
        /// Blends two colors. A weight of 1 returns <paramref name="foreground"/>;
        /// a weight of 0 returns <paramref name="background"/>.
        /// </summary>
        public static Color Blend(Color foreground, Color background, double foregroundWeight)
        {
            if (double.IsNaN(foregroundWeight) || foregroundWeight < 0.0 || foregroundWeight > 1.0)
                throw new ArgumentOutOfRangeException("foregroundWeight");

            double backgroundWeight = 1.0 - foregroundWeight;

            return Color.FromArgb(
                BlendChannel(foreground.A, background.A, foregroundWeight, backgroundWeight),
                BlendChannel(foreground.R, background.R, foregroundWeight, backgroundWeight),
                BlendChannel(foreground.G, background.G, foregroundWeight, backgroundWeight),
                BlendChannel(foreground.B, background.B, foregroundWeight, backgroundWeight));
        }

        static int BlendChannel(
            int foreground,
            int background,
            double foregroundWeight,
            double backgroundWeight)
        {
            return (int)Math.Round(
                (foreground * foregroundWeight) +
                (background * backgroundWeight));
        }
    }
}
