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

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ankh.Commands
{
    internal static class LockCommandLogic
    {
        static Regex _quotedValueRegex;

        public static bool IsLockCandidate(
            bool isFile,
            bool isVersioned,
            bool isNewAddition,
            bool isLocked)
        {
            return isFile
                && isVersioned
                && !isNewAddition
                && !isLocked;
        }

        public static bool ShouldPrompt(
            bool dontPrompt,
            bool promptUser,
            bool shift,
            bool suppressLockingUI)
        {
            return !dontPrompt
                && (promptUser || !(shift || suppressLockingUI));
        }

        public static string GetLockOwner(
            string reportedOwner,
            string errorMessage)
        {
            if (!string.IsNullOrEmpty(reportedOwner))
                return reportedOwner;

            return GuessUserFromError(errorMessage) ?? "?";
        }

        public static string GuessUserFromError(string message)
        {
            if (_quotedValueRegex == null)
            {
                _quotedValueRegex = new Regex(
                    "['»](?<value>.*?)['«]",
                    RegexOptions.Compiled
                        | RegexOptions.ExplicitCapture
                        | RegexOptions.Singleline);
            }

            MatchCollection values = _quotedValueRegex.Matches(message);
            if (values.Count < 2)
                return null;

            return values[1].Groups["value"].Value;
        }

        public static string BuildAlreadyLockedMessage(
            string heading,
            string itemFormat,
            IEnumerable<KeyValuePair<string, string>> lockedFiles)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine(heading);
            message.AppendLine();

            foreach (KeyValuePair<string, string> item in lockedFiles)
            {
                if (!string.IsNullOrEmpty(item.Value))
                    message.AppendFormat(itemFormat, item.Key, item.Value);
                else
                    message.Append(item.Key);

                message.AppendLine();
            }

            return message.ToString().TrimEnd();
        }
    }
}
