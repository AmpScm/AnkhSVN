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

namespace Ankh.UI.PendingChanges.Commits
{
    internal static class PendingCommitsThemeLogic
    {
        internal static bool ShouldCancelVsHeaderTheming(
            bool darkSurface,
            bool highContrast)
        {
            // The select-all header needs custom handling on ordinary light
            // surfaces. High contrast is left entirely to Visual Studio/Windows.
            return !darkSurface && !highContrast;
        }

        internal static bool ShouldHideInactiveSelection(
            bool darkSurface,
            bool highContrast)
        {
            // The native themed ListView can ignore custom foreground colors
            // for an unfocused selected row and fall back to black text.
            // On dark surfaces, hide that inactive native selection instead;
            // when focus returns, the normal selected-row highlight reappears.
            return darkSurface && !highContrast;
        }

        internal static Color ResolveItemForeColor(
            Color statusColor,
            Color listForeColor,
            Color listBackColor)
        {
            // An empty ListViewItem.ForeColor is interpreted by the native
            // control as a system/default color. That is normally hidden by
            // the active-selection highlight text, but becomes visible when
            // the list loses focus and can produce black text on a dark
            // inactive-selection background. Always fall back to the explicit
            // Visual Studio palette foreground instead.
            if (!statusColor.IsEmpty
                && AnkhThemePalette.ContrastRatio(statusColor, listBackColor) >= 4.5)
            {
                return statusColor;
            }

            return listForeColor;
        }
    }
}
