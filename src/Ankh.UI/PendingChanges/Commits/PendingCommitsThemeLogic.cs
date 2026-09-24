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

        internal static Color ResolveItemForeColor(
            Color statusColor,
            Color listForeColor,
            Color listBackColor)
        {
            // An empty ListViewItem.ForeColor lets the native control choose a
            // system/default color that may not match the active Visual Studio
            // theme. Always fall back to the explicit current VS palette
            // foreground, whatever color that theme defines.
            return AnkhThemePalette.ResolveReadableForeground(
                statusColor,
                listForeColor,
                listBackColor);
        }
    }
}
