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

namespace Ankh.UI.PendingChanges.Commits
{
    internal static class PendingChangeDisplayLogic
    {
        internal static string FormatModifiedDate(DateTime utcDateTime)
        {
            if (utcDateTime.Ticks == 0 || utcDateTime.Ticks == 1)
                return "";

            // "Modified" is the filesystem last-write timestamp. Always show
            // both date and time; the previous time-only/date-only split made
            // rows look as though they were reporting different kinds of data.
            return utcDateTime.ToLocalTime().ToString("g");
        }
    }
}
