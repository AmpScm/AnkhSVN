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

namespace Ankh.UI.VSSelectionControls
{
    internal static class SmartListViewThemeLogic
    {
        internal static bool ShouldOwnerDrawHeader(
            bool themeCancelled,
            bool hasPalette,
            bool highContrast)
        {
            return !themeCancelled && hasPalette && !highContrast;
        }

        internal static bool ShouldUseDarkNativeTheme(
            bool inVsTheming,
            bool darkSurface,
            bool highContrast)
        {
            // Native ListView inactive-selection colors otherwise come from
            // the light Explorer theme and can render dark text on a dark
            // Visual Studio surface after focus moves away from the list.
            return inVsTheming && darkSurface && !highContrast;
        }
    }
}
