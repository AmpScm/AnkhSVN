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
using System.Collections.Generic;

namespace Ankh.UI.RepositoryOpen
{
    internal sealed class CheckoutProjectLoadPlan
    {
        public CheckoutProjectLoadPlan(
            Uri repositoryRootUri,
            IList<Uri> candidates)
        {
            RepositoryRootUri = repositoryRootUri;
            Candidates = candidates;
        }

        public Uri RepositoryRootUri { get; private set; }
        public IList<Uri> Candidates { get; private set; }
    }

    internal static class CheckoutProjectLogic
    {
        public static CheckoutProjectLoadPlan BuildLoadPlan(
            Uri repositoryRootUri,
            Uri projectUri,
            Uri projectTop)
        {
            if (repositoryRootUri == null)
                throw new ArgumentNullException("repositoryRootUri");
            if (projectUri == null)
                throw new ArgumentNullException("projectUri");

            Uri inner = projectTop ?? new Uri(projectUri, "./");
            Uri info = repositoryRootUri.MakeRelativeUri(inner);

            if (info.IsAbsoluteUri ||
                info.ToString().StartsWith("../", StringComparison.Ordinal))
            {
                repositoryRootUri = new Uri(inner, "/");
            }

            List<Uri> candidates = new List<Uri>();

            while (inner != repositoryRootUri)
            {
                candidates.Add(inner);
                inner = new Uri(inner, "../");
            }

            candidates.Add(inner);

            return new CheckoutProjectLoadPlan(
                repositoryRootUri,
                candidates);
        }

        public static int SelectDefaultIndex(
            IList<Uri> candidates,
            Uri guessedWorkingRoot)
        {
            if (candidates == null)
                throw new ArgumentNullException("candidates");

            if (guessedWorkingRoot != null)
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    if (candidates[i] == guessedWorkingRoot)
                        return i;
                }
            }

            for (int i = 0; i < candidates.Count; i++)
            {
                if (EndsWith(candidates[i], "/trunk/"))
                    return i;
            }

            for (int i = 0; i < candidates.Count; i++)
            {
                if (EndsWith(candidates[i], "/branches/") ||
                    EndsWith(candidates[i], "/tags/") ||
                    EndsWith(candidates[i], "/releases/"))
                {
                    if (i > 1)
                        return i - 1;
                }
            }

            for (int i = 0; i < candidates.Count; i++)
            {
                if (EndsWith(candidates[i], "/src/") ||
                    EndsWith(candidates[i], "/source/") ||
                    EndsWith(candidates[i], "/sourcecode/"))
                {
                    if (i < candidates.Count - 1)
                        return i + 1;
                }
            }

            return candidates.Count > 0 ? 0 : -1;
        }

        static bool EndsWith(Uri uri, string suffix)
        {
            return uri != null &&
                uri.ToString().EndsWith(
                    suffix,
                    StringComparison.OrdinalIgnoreCase);
        }
    }
}
