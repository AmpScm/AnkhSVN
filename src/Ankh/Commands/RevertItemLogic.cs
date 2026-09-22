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
using SharpSvn;

namespace Ankh.Commands
{
    internal static class RevertItemLogic
    {
        public static bool ShouldIncludePrimary(
            bool isModified,
            bool isVersioned,
            bool isDocumentDirty,
            bool isConflicted)
        {
            return isModified ||
                (isVersioned && isDocumentDirty) ||
                isConflicted;
        }

        public static bool ShouldIncludeDescendant(
            bool isModified,
            bool isVersioned,
            bool isDocumentDirty)
        {
            return isModified ||
                (isVersioned && isDocumentDirty);
        }

        public static int CompareForRevert(
            bool leftIsAddOrReplace,
            string leftPath,
            bool rightIsAddOrReplace,
            string rightPath)
        {
            if (leftIsAddOrReplace && !rightIsAddOrReplace)
                return -1;

            if (rightIsAddOrReplace && !leftIsAddOrReplace)
                return 1;

            if (leftIsAddOrReplace && rightIsAddOrReplace)
            {
                // Added/replaced descendants must be reverted before their parents.
                return -StringComparer.OrdinalIgnoreCase.Compare(leftPath, rightPath);
            }

            return StringComparer.OrdinalIgnoreCase.Compare(leftPath, rightPath);
        }

        public static bool HasBlockingChildChange(
            bool conflicted,
            SvnStatus propertyStatus,
            SvnStatus nodeStatus)
        {
            if (conflicted ||
                (propertyStatus != SvnStatus.Normal &&
                 propertyStatus != SvnStatus.None))
            {
                return true;
            }

            switch (nodeStatus)
            {
                case SvnStatus.None:
                case SvnStatus.Normal:
                case SvnStatus.Ignored:
                case SvnStatus.External:
                case SvnStatus.NotVersioned:
                    return false;

                default:
                    return true;
            }
        }
    }
}
