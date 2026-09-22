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
using Ankh.Scc;
using SharpSvn;

namespace Ankh.Commands
{
    internal sealed class DiffLocalItemSelectionInfo
    {
        public bool IsVersioned { get; private set; }
        public SvnStatus LocalNodeStatus { get; private set; }
        public bool IsCopied { get; private set; }
        public bool IsModified { get; private set; }
        public bool IsDocumentDirty { get; private set; }
        public bool IsLocalDiffAvailable { get; private set; }
        public bool IsDeleteScheduled { get; private set; }
        public bool Exists { get; private set; }

        public DiffLocalItemSelectionInfo(
            bool isVersioned,
            SvnStatus localNodeStatus,
            bool isCopied,
            bool isModified,
            bool isDocumentDirty,
            bool isLocalDiffAvailable,
            bool isDeleteScheduled,
            bool exists)
        {
            IsVersioned = isVersioned;
            LocalNodeStatus = localNodeStatus;
            IsCopied = isCopied;
            IsModified = isModified;
            IsDocumentDirty = isDocumentDirty;
            IsLocalDiffAvailable = isLocalDiffAvailable;
            IsDeleteScheduled = isDeleteScheduled;
            Exists = exists;
        }

        public static DiffLocalItemSelectionInfo From(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return new DiffLocalItemSelectionInfo(
                item.IsVersioned,
                item.Status.LocalNodeStatus,
                item.Status.IsCopied,
                item.IsModified,
                item.IsDocumentDirty,
                item.IsLocalDiffAvailable,
                item.IsDeleteScheduled,
                item.Exists);
        }
    }

    internal static class DiffLocalItemLogic
    {
        public static bool ShouldInclude(
            AnkhCommand command,
            DiffLocalItemSelectionInfo item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (!item.IsVersioned ||
                (item.LocalNodeStatus == SvnStatus.Added && !item.IsCopied))
            {
                return false;
            }

            if (command == AnkhCommand.ItemCompareBase ||
                command == AnkhCommand.ItemShowChanges)
            {
                if (!(item.IsModified || item.IsDocumentDirty) ||
                    !item.IsLocalDiffAvailable)
                {
                    return false;
                }
            }

            if (command == AnkhCommand.DiffLocalItem &&
                (item.IsDeleteScheduled || !item.Exists))
            {
                return false;
            }

            return true;
        }

        public static SvnRevisionRange GetDefaultRevisionRange(AnkhCommand command)
        {
            switch (command)
            {
                case AnkhCommand.ItemCompareBase:
                case AnkhCommand.ItemShowChanges:
                case AnkhCommand.DocumentShowChanges:
                    return new SvnRevisionRange(SvnRevision.Base, SvnRevision.Working);

                case AnkhCommand.ItemCompareCommitted:
                    return new SvnRevisionRange(SvnRevision.Committed, SvnRevision.Working);

                case AnkhCommand.ItemCompareLatest:
                    return new SvnRevisionRange(SvnRevision.Head, SvnRevision.Working);

                case AnkhCommand.ItemComparePrevious:
                    return new SvnRevisionRange(SvnRevision.Previous, SvnRevision.Working);

                default:
                    // DiffLocalItem and ItemCompareSpecific require the selector.
                    return null;
            }
        }

        public static bool RequiresDocumentSave(SvnRevisionRange revisionRange)
        {
            return revisionRange != null &&
                (revisionRange.StartRevision.RevisionType == SvnRevisionType.Working ||
                 revisionRange.EndRevision.RevisionType == SvnRevisionType.Working);
        }

        public static bool ShouldUseCopyOrigin(
            bool isCopied,
            bool isReplaced,
            SvnRevisionRange revisionRange)
        {
            if (revisionRange == null || (!isCopied && !isReplaced))
                return false;

            return !revisionRange.StartRevision.RequiresWorkingCopy ||
                !revisionRange.EndRevision.RequiresWorkingCopy;
        }
    }
}
