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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.UI;
using Ankh.VS;

namespace Ankh.WpfPackage.Services
{
    partial class ThemingService
    {
        AnkhThemePalette _activeThemePalette;

        public AnkhThemePalette ThemePalette
        {
            get { return _activeThemePalette ?? CreateThemePalette(); }
        }

        AnkhThemePalette CreateThemePalette()
        {
            Color surfaceBackground = GetVsColor(
                __VSSYSCOLOREX.VSCOLOR_TOOLWINDOW_BACKGROUND,
                SystemColors.Control);

            Color surfaceForeground = GetVsColor(
                __VSSYSCOLOREX.VSCOLOR_TOOLWINDOW_TEXT,
                SystemColors.ControlText);

            Color inputBackground = GetVsColor(
                (__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_COMBOBOX_BACKGROUND,
                surfaceBackground);

            Color inputForeground = GetVsColor(
                (__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_WINDOWTEXT,
                surfaceForeground);

            // CommonControls exposes the matching combo text token, which the
            // legacy GetVSSysColorEx enumeration does not provide.
            IVsUIShell5 shell = GetService<IVsUIShell5>(typeof(SVsUIShell));
            if (shell != null)
            {
                try
                {
                    var key = CommonControlsColors.ComboBoxTextColorKey;
                    var category = key.Category;
                    uint rgb = shell.GetThemedColor(ref category, key.Name, 0);
                    Color comboText = ColorTranslator.FromWin32(unchecked((int)rgb));
                    key = CommonControlsColors.ComboBoxBackgroundColorKey;
                    category = key.Category;
                    rgb = shell.GetThemedColor(ref category, key.Name, 0);
                    inputBackground = ColorTranslator.FromWin32(unchecked((int)rgb));
                    inputForeground = comboText;
                }
                catch (COMException)
                {
                    // Older shells may not expose the CommonControls token.
                }
            }

            Color disabledText = GetVsColor(
                (__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_GRAYTEXT,
                AnkhThemePalette.Blend(surfaceForeground, surfaceBackground, 0.55));

            Color border = GetVsColor(
                (__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_COMBOBOX_BORDER,
                AnkhThemePalette.Blend(surfaceForeground, surfaceBackground, 0.25));

            Color selectionBackground = GetVsColor(
                (__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_HIGHLIGHT,
                AnkhThemePalette.Blend(surfaceForeground, surfaceBackground, 0.22));

            Color selectionForeground = GetVsColor(
                (__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_HIGHLIGHTTEXT,
                surfaceForeground);

            Color hoverBackground = GetVsColor(
                (__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_COMBOBOX_MOUSEOVER_BACKGROUND_BEGIN,
                AnkhThemePalette.Blend(surfaceForeground, surfaceBackground, 0.08));

            Color pressedBackground = GetVsColor(
                (__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_COMBOBOX_MOUSEDOWN_BACKGROUND,
                AnkhThemePalette.Blend(surfaceForeground, surfaceBackground, 0.16));

            Color focusBorder = GetVsColor(
                (__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_COMBOBOX_MOUSEDOWN_BORDER,
                selectionBackground);

            return new AnkhThemePalette(
                surfaceBackground,
                surfaceForeground,
                inputBackground,
                inputForeground,
                disabledText,
                border,
                selectionBackground,
                selectionForeground,
                hoverBackground,
                pressedBackground,
                focusBorder);
        }

        Color GetVsColor(__VSSYSCOLOREX colorId, Color fallback)
        {
            Color color;
            IAnkhVSColor colors = ColorSvc;

            return colors != null && colors.TryGetColor(colorId, out color)
                ? color
                : fallback;
        }
    }
}
