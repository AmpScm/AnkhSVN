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

namespace Ankh.Commands
{
    internal enum UpdateCommandScope
    {
        Project,
        Solution,
        Folder
    }

    internal sealed class UpdateGroup
    {
        readonly string _wcroot;
        readonly System.Collections.Generic.List<string> _files;

        public UpdateGroup(string wcroot)
        {
            if (string.IsNullOrEmpty(wcroot))
                throw new ArgumentNullException("wcroot");

            _wcroot = wcroot;
            _files = new System.Collections.Generic.List<string>();
        }

        public string WorkingCopyRoot
        {
            get { return _wcroot; }
        }

        public System.Collections.Generic.List<string> Nodes
        {
            get { return _files; }
        }
    }

    internal sealed class SolutionUpdatePlan
    {
        readonly System.Collections.Generic.Dictionary<string, bool> _included =
            new System.Collections.Generic.Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        readonly System.Collections.Generic.SortedList<string, UpdateGroup> _groups =
            new System.Collections.Generic.SortedList<string, UpdateGroup>(StringComparer.OrdinalIgnoreCase);

        public System.Collections.Generic.IEnumerable<UpdateGroup> Groups
        {
            get { return _groups.Values; }
        }

        public int GroupCount
        {
            get { return _groups.Count; }
        }

        public bool TryAddRoot(
            string fullPath,
            bool isVersioned,
            bool isHeadCommand,
            Uri selectedRepositoryRoot,
            Uri itemRepositoryRoot,
            string workingCopyRoot)
        {
            if (!SolutionUpdateLogic.ShouldIncludeUpdateRoot(
                    _included.ContainsKey(fullPath),
                    isVersioned,
                    isHeadCommand,
                    selectedRepositoryRoot,
                    itemRepositoryRoot))
            {
                return false;
            }

            UpdateGroup group;
            if (!_groups.TryGetValue(workingCopyRoot, out group))
            {
                group = new UpdateGroup(workingCopyRoot);
                _groups.Add(workingCopyRoot, group);
            }

            group.Nodes.Add(fullPath);
            _included.Add(fullPath, true);
            return true;
        }
    }

    internal static class SolutionUpdateLogic
    {
        public static UpdateCommandScope GetScope(AnkhCommand command)
        {
            switch (command)
            {
                case AnkhCommand.SolutionUpdateLatest:
                case AnkhCommand.SolutionUpdateSpecific:
                case AnkhCommand.PendingChangesUpdateLatest:
                    return UpdateCommandScope.Solution;

                case AnkhCommand.FolderUpdateLatest:
                case AnkhCommand.FolderUpdateSpecific:
                    return UpdateCommandScope.Folder;

                default:
                    return UpdateCommandScope.Project;
            }
        }

        public static bool IsHeadCommand(AnkhCommand command)
        {
            switch (command)
            {
                case AnkhCommand.SolutionUpdateLatest:
                case AnkhCommand.ProjectUpdateLatest:
                case AnkhCommand.PendingChangesUpdateLatest:
                case AnkhCommand.FolderUpdateLatest:
                    return true;

                default:
                    return false;
            }
        }

        public static bool UsesImplicitHeadRevision(
            AnkhCommand command,
            bool dontPrompt)
        {
            return dontPrompt || IsHeadCommand(command);
        }

        public static Uri GetCommonAncestorUri(Uri left, Uri right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            string leftPath = left.AbsolutePath;
            string rightPath = right.AbsolutePath;

            int i = 0;
            while (i < leftPath.Length &&
                   i < rightPath.Length &&
                   leftPath[i] == rightPath[i])
            {
                i++;
            }

            while (i > 0 && leftPath[i - 1] != '/')
                i--;

            return new Uri(left, leftPath.Substring(0, i));
        }

        public static bool ShouldIncludeUpdateRoot(
            bool alreadyIncluded,
            bool isVersioned,
            bool isHeadCommand,
            Uri selectedRepositoryRoot,
            Uri itemRepositoryRoot)
        {
            if (alreadyIncluded || !isVersioned)
                return false;

            if (!isHeadCommand &&
                selectedRepositoryRoot != null &&
                itemRepositoryRoot != null &&
                itemRepositoryRoot != selectedRepositoryRoot)
            {
                return false;
            }

            return true;
        }
    }
}
