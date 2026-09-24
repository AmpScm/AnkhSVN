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

namespace Ankh.Commands
{
    internal static class AnnotateCommandLogic
    {
        public static bool IsAnnotatableItem(
            bool isFile,
            bool isVersioned,
            bool hasCopyableHistory)
        {
            return isFile && isVersioned && hasCopyableHistory;
        }

        public static bool ShouldPrompt(
            bool dontPrompt,
            bool shiftPressed,
            bool promptUser)
        {
            return (!dontPrompt && !shiftPressed) || promptUser;
        }

        public static bool ShouldSaveDocument(
            bool startIsWorking,
            bool endIsWorking,
            bool targetIsPath)
        {
            return (startIsWorking || endIsWorking) && targetIsPath;
        }

        public static bool TryGetPreviousRevision(long revision, out long previousRevision)
        {
            if (revision <= 0)
            {
                previousRevision = -1;
                return false;
            }

            previousRevision = revision - 1;
            return true;
        }
    }
}
