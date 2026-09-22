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

namespace Ankh.VS.LanguageServices.Core
{
    internal sealed class EditorExtensionDecision
    {
        public EditorExtensionDecision(
            bool isSupported,
            bool takeover,
            bool requiresFormatCheck)
        {
            IsSupported = isSupported;
            Takeover = takeover;
            RequiresFormatCheck = requiresFormatCheck;
        }

        public bool IsSupported { get; private set; }
        public bool Takeover { get; private set; }
        public bool RequiresFormatCheck { get; private set; }
    }

    internal enum EditorLanguageAction
    {
        None,
        SetRequested,
        KeepRequested,
        Incompatible
    }

    internal sealed class EditorLanguageDecision
    {
        public EditorLanguageDecision(
            EditorLanguageAction action,
            bool takeover)
        {
            Action = action;
            Takeover = takeover;
        }

        public EditorLanguageAction Action { get; private set; }
        public bool Takeover { get; private set; }
    }

    internal static class AnkhEditorFactoryLogic
    {
        public static bool ShouldRejectEncodingPrompt(
            bool codePagePrompt,
            bool hasExistingDocumentData)
        {
            return codePagePrompt && hasExistingDocumentData;
        }

        public static EditorExtensionDecision EvaluateExtension(
            bool hasMoniker,
            bool openSpecific,
            bool isRegisteredExtension,
            bool isUserDefinedEditor,
            bool canEditAnyway,
            bool checksAllFileTypes)
        {
            if (!hasMoniker)
                return new EditorExtensionDecision(true, false, false);

            if (!isRegisteredExtension
                && !isUserDefinedEditor
                && !canEditAnyway
                && !openSpecific)
            {
                return new EditorExtensionDecision(false, false, false);
            }

            bool takeover =
                checksAllFileTypes && !isRegisteredExtension;

            bool requiresFormatCheck =
                takeover
                && !isUserDefinedEditor
                && !openSpecific;

            return new EditorExtensionDecision(
                true,
                takeover,
                requiresFormatCheck);
        }

        public static EditorLanguageDecision EvaluateLanguageService(
            Guid requestedLanguageService,
            Guid currentLanguageService,
            Guid defaultLanguageService)
        {
            if (requestedLanguageService == Guid.Empty)
            {
                return new EditorLanguageDecision(
                    EditorLanguageAction.None,
                    false);
            }

            if (currentLanguageService == defaultLanguageService)
            {
                return new EditorLanguageDecision(
                    EditorLanguageAction.SetRequested,
                    true);
            }

            if (currentLanguageService == requestedLanguageService)
            {
                return new EditorLanguageDecision(
                    EditorLanguageAction.KeepRequested,
                    true);
            }

            return new EditorLanguageDecision(
                EditorLanguageAction.Incompatible,
                false);
        }
    }
}
