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
using System.IO;

namespace Ankh.Scc
{
    internal static class ProjectTrackerOriginLogic
    {
        public static SortedList<string, string> BuildNameToItem(
            string newName,
            IEnumerable<KeyValuePair<string, string>> origins)
        {
            SortedList<string, string> result =
                new SortedList<string, string>();

            result[Path.GetFileName(newName)] = newName;

            foreach (KeyValuePair<string, string> pair in origins)
            {
                if (pair.Value != null)
                    continue;

                result[Path.GetFileName(pair.Key)] = pair.Key;
            }

            return result;
        }

        public static string InferOrigin(
            string newName,
            IEnumerable<KeyValuePair<string, string>> origins)
        {
            bool first = true;
            string path = null;

            foreach (KeyValuePair<string, string> pair in origins)
            {
                if (pair.Value == null)
                    continue;

                if (!SvnItem.IsBelowRoot(pair.Key, newName))
                    continue;

                string itemRoot = pair.Value.Substring(
                    0,
                    pair.Value.Length - pair.Key.Length + newName.Length);

                if (first)
                {
                    path = itemRoot;
                    first = false;
                }
                else if (path != itemRoot)
                    return null;
            }

            return path;
        }
    }
}
