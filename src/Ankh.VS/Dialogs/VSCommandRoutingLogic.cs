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

using System.Windows.Forms;

using Ankh.UI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS.Dialogs
{
    internal sealed class VSCommandRoutingKeyPlan
    {
        public VSCommandRoutingKeyPlan(
            bool isKeyboardMessage,
            bool bypassRouting,
            VSContainerMode mode,
            bool useFilterKeys,
            uint translationFlags)
        {
            IsKeyboardMessage = isKeyboardMessage;
            BypassRouting = bypassRouting;
            Mode = mode;
            UseFilterKeys = useFilterKeys;
            TranslationFlags = translationFlags;
        }

        public bool IsKeyboardMessage { get; private set; }
        public bool BypassRouting { get; private set; }
        public VSContainerMode Mode { get; private set; }
        public bool UseFilterKeys { get; private set; }
        public uint TranslationFlags { get; private set; }
    }

    internal static class VSCommandRoutingLogic
    {
        const int WM_KEYFIRST = 0x0100;
        const int WM_IME_KEYLAST = 0x010F;
        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;
        const int WM_SYSKEYDOWN = 0x0104;
        const int WM_SYSKEYUP = 0x0105;

        internal static bool IsKeyboardMessage(int message)
        {
            return message >= WM_KEYFIRST && message <= WM_IME_KEYLAST;
        }

        public static VSCommandRoutingKeyPlan BuildKeyPlan(
            int message,
            Keys key,
            Keys modifiers,
            VSContainerMode mode)
        {
            if (!IsKeyboardMessage(message))
                return Plan(false, false, mode);

            bool bypassRouting = false;
            VSContainerMode effectiveMode = mode;

            if (message == WM_KEYDOWN || message == WM_KEYUP)
            {
                switch (key)
                {
                    case Keys.Tab:
                        bypassRouting = (modifiers & Keys.Control) != 0;
                        break;

                    case Keys.Return:
                        if (modifiers == Keys.Control)
                            effectiveMode = VSContainerMode.Default;
                        break;

                    case Keys.Escape:
                        bypassRouting = true;
                        break;
                }
            }
            else if ((message == WM_SYSKEYDOWN || message == WM_SYSKEYUP)
                && key == Keys.F4)
            {
                bypassRouting = true;
            }

            return Plan(true, bypassRouting, effectiveMode);
        }

        public static bool ShouldConsumeTranslatedCommand(
            int hResult,
            int commandTranslated)
        {
            return hResult == VSConstants.S_OK && commandTranslated != 0;
        }

        static VSCommandRoutingKeyPlan Plan(
            bool isKeyboardMessage,
            bool bypassRouting,
            VSContainerMode mode)
        {
            const VSContainerMode translationModes =
                VSContainerMode.TranslateKeys
                | VSContainerMode.UseTextEditorScope;

            bool useFilterKeys = (mode & translationModes) != 0;
            uint translationFlags = 0;

            if (useFilterKeys)
            {
                translationFlags =
                    (uint)__VSTRANSACCELEXFLAGS.VSTAEXF_AllowModalState;

                if ((mode & VSContainerMode.UseTextEditorScope) != 0)
                {
                    translationFlags |=
                        (uint)__VSTRANSACCELEXFLAGS.VSTAEXF_UseTextEditorKBScope;
                }
            }

            return new VSCommandRoutingKeyPlan(
                isKeyboardMessage,
                bypassRouting,
                mode,
                useFilterKeys,
                translationFlags);
        }
    }
}
