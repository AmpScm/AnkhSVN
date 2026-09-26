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

using Ankh.Commands;
using Ankh.Scc;

namespace Ankh.UI.PendingChanges.Conflicts
{
    internal static class PendingConflictLogic
    {
        internal static bool IsConflict(PendingChangeKind kind)
        {
            return kind == PendingChangeKind.Conflicted
                || kind == PendingChangeKind.PropertyConflicted
                || kind == PendingChangeKind.TreeConflict;
        }

        internal static string FormatHeader(int versionConflicts, int treeConflicts)
        {
            int total = versionConflicts + treeConflicts;
            if (total == 0)
                return "No conflicts.";

            return string.Format(
                "{0} conflict{1}: {2} version conflict{3}, {4} tree conflict{5}.",
                total,
                total == 1 ? "" : "s",
                versionConflicts,
                versionConflicts == 1 ? "" : "s",
                treeConflicts,
                treeConflicts == 1 ? "" : "s");
        }

        internal static string GetConflictType(
            PendingChangeKind kind,
            bool textConflict,
            bool propertyConflict)
        {
            if (kind == PendingChangeKind.TreeConflict)
                return "Tree";
            if (textConflict && propertyConflict)
                return "Text + Property";
            if (textConflict)
                return "Text";
            if (propertyConflict || kind == PendingChangeKind.PropertyConflicted)
                return "Property";

            return "Version";
        }

        internal static string GetConflictDescription(
            PendingChangeKind kind,
            bool textConflict,
            bool propertyConflict)
        {
            if (kind == PendingChangeKind.TreeConflict)
                return "Tree conflict";
            if (textConflict && propertyConflict)
                return "Text and property conflict";
            if (textConflict)
                return "Text conflict";
            if (propertyConflict || kind == PendingChangeKind.PropertyConflicted)
                return "Property conflict";

            return "Version conflict";
        }

        internal static bool CanExecuteResolution(
            AnkhCommand command,
            bool hasSelection,
            bool anyTreeConflict,
            bool allTextFiles,
            bool singleTextConflict)
        {
            if (!hasSelection)
                return false;

            switch (command)
            {
                case AnkhCommand.ItemConflictEdit:
                    return !anyTreeConflict && singleTextConflict;

                case AnkhCommand.ItemResolveWorking:
                    // Subversion permits accepting the working state for tree
                    // conflicts; the other automatic choices do not apply.
                    return true;

                case AnkhCommand.ItemResolveMerge:
                case AnkhCommand.ItemResolveBase:
                case AnkhCommand.ItemResolveMineFull:
                case AnkhCommand.ItemResolveTheirsFull:
                    return !anyTreeConflict;

                case AnkhCommand.ItemResolveMineConflict:
                case AnkhCommand.ItemResolveTheirsConflict:
                    return !anyTreeConflict && allTextFiles;

                default:
                    return false;
            }
        }
    }
}
