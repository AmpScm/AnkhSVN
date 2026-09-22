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

using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    internal enum QueryEditFileAction
    {
        None,
        QueueMustLock,
        RejectMustLock,
        QueueReadOnly,
        RejectReadOnly
    }

    internal static class QueryEditLogic
    {
        public static bool AllowsUI(tagVSQueryEditFlags queryFlags)
        {
            const tagVSQueryEditFlags noUiFlags =
                tagVSQueryEditFlags.QEF_SilentMode
                | tagVSQueryEditFlags.QEF_ReportOnly
                | tagVSQueryEditFlags.QEF_ForceEdit_NoPrompting;

            return (queryFlags & noUiFlags) == 0;
        }

        public static bool NeedsReadOnlyNonSccPolicy(
            bool isReadOnlyMustLock,
            bool isDirectory,
            bool isReadOnly)
        {
            return !(isReadOnlyMustLock && !isDirectory) && isReadOnly;
        }

        public static QueryEditFileAction GetFileAction(
            bool isReadOnlyMustLock,
            bool isDirectory,
            bool isReadOnly,
            bool allowUI,
            bool allowReadOnlyNonSccWrites)
        {
            if (isReadOnlyMustLock && !isDirectory)
            {
                return allowUI
                    ? QueryEditFileAction.QueueMustLock
                    : QueryEditFileAction.RejectMustLock;
            }

            if (!isReadOnly || allowReadOnlyNonSccWrites)
                return QueryEditFileAction.None;

            return allowUI
                ? QueryEditFileAction.QueueReadOnly
                : QueryEditFileAction.RejectReadOnly;
        }
    }
}
