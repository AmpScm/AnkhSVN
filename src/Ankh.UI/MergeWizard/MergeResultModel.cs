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
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    internal enum MergeResultActionKind
    {
        Existed,
        Skipped,
        Added,
        Deleted,
        Replaced,
        Modified,
        Conflicted,
        Merged
    }

    internal sealed class MergeNotificationInfo
    {
        public MergeNotificationInfo(
            string fullPath,
            SvnNotifyAction action,
            SvnNodeKind nodeKind,
            SvnNotifyState contentState,
            SvnNotifyState propertyState)
        {
            FullPath = fullPath;
            Action = action;
            NodeKind = nodeKind;
            ContentState = contentState;
            PropertyState = propertyState;
        }

        public string FullPath { get; private set; }
        public SvnNotifyAction Action { get; private set; }
        public SvnNodeKind NodeKind { get; private set; }
        public SvnNotifyState ContentState { get; private set; }
        public SvnNotifyState PropertyState { get; private set; }

        public static MergeNotificationInfo From(SvnNotifyEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            return new MergeNotificationInfo(
                e.FullPath,
                e.Action,
                e.NodeKind,
                e.ContentState,
                e.PropertyState);
        }
    }

    internal sealed class MergePathResult
    {
        readonly List<MergeResultActionKind> _contentActions = new List<MergeResultActionKind>();
        readonly List<MergeResultActionKind> _propertyActions = new List<MergeResultActionKind>();

        public MergePathResult(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }
        public IList<MergeResultActionKind> ContentActions { get { return _contentActions; } }
        public IList<MergeResultActionKind> PropertyActions { get { return _propertyActions; } }

        internal bool AddContent(MergeResultActionKind action)
        {
            if (_contentActions.Contains(action))
                return false;

            _contentActions.Add(action);
            return true;
        }

        internal bool AddProperty(MergeResultActionKind action)
        {
            if (_propertyActions.Contains(action))
                return false;

            _propertyActions.Add(action);
            return true;
        }
    }

    internal sealed class MergeResultModel
    {
        readonly Dictionary<string, MergePathResult> _paths =
            new Dictionary<string, MergePathResult>();

        public IDictionary<string, MergePathResult> Paths { get { return _paths; } }

        public long FileUpdated { get; private set; }
        public long FileAdded { get; private set; }
        public long FileExisted { get; private set; }
        public long FileDeleted { get; private set; }
        public long FileMerged { get; private set; }
        public long FileConflicted { get; private set; }
        public long FileResolved { get; private set; }
        public long FileSkippedDirectories { get; private set; }
        public long FileSkippedFiles { get; private set; }

        public long PropertyUpdated { get; private set; }
        public long PropertyMerged { get; private set; }
        public long PropertyConflicted { get; private set; }
        public long PropertyResolved { get; private set; }

        public static MergeResultModel Build(
            IEnumerable<MergeNotificationInfo> mergeActions,
            IDictionary<string, List<SvnConflictType>> resolvedConflicts)
        {
            MergeResultModel model = new MergeResultModel();

            if (mergeActions != null)
            {
                foreach (MergeNotificationInfo action in mergeActions)
                    model.Apply(action);
            }

            if (resolvedConflicts != null)
            {
                foreach (KeyValuePair<string, List<SvnConflictType>> resolution in resolvedConflicts)
                {
                    if (resolution.Value == null)
                        continue;

                    // Preserve the original dialog behavior: a path resolved for both
                    // content and properties is counted as a content resolution.
                    if (resolution.Value.Contains(SvnConflictType.Content))
                        model.FileResolved++;
                    else if (resolution.Value.Contains(SvnConflictType.Property))
                        model.PropertyResolved++;
                }
            }

            return model;
        }

        void Apply(MergeNotificationInfo action)
        {
            if (action == null || String.IsNullOrEmpty(action.FullPath))
                return;

            switch (action.Action)
            {
                case SvnNotifyAction.Exists:
                    if (AddContent(action.FullPath, MergeResultActionKind.Existed))
                        FileExisted++;
                    break;

                case SvnNotifyAction.Skip:
                    if (AddContent(action.FullPath, MergeResultActionKind.Skipped))
                    {
                        if (action.NodeKind == SvnNodeKind.Directory)
                            FileSkippedDirectories++;
                        else if (action.NodeKind == SvnNodeKind.File)
                            FileSkippedFiles++;
                    }
                    break;

                case SvnNotifyAction.UpdateAdd:
                    if (AddContent(action.FullPath, MergeResultActionKind.Added))
                        FileAdded++;
                    break;

                case SvnNotifyAction.UpdateDelete:
                    if (AddContent(action.FullPath, MergeResultActionKind.Deleted))
                        FileDeleted++;
                    break;

                case SvnNotifyAction.UpdateReplace:
                    // The existing dialog counts replacements in the Added total.
                    if (AddContent(action.FullPath, MergeResultActionKind.Replaced))
                        FileAdded++;
                    break;

                case SvnNotifyAction.UpdateUpdate:
                    ApplyContentState(action.FullPath, action.ContentState);
                    ApplyPropertyState(action.FullPath, action.PropertyState);
                    break;
            }
        }

        void ApplyContentState(string path, SvnNotifyState state)
        {
            switch (state)
            {
                case SvnNotifyState.Changed:
                    if (AddContent(path, MergeResultActionKind.Modified))
                        FileUpdated++;
                    break;

                case SvnNotifyState.Conflicted:
                    if (AddContent(path, MergeResultActionKind.Conflicted))
                        FileConflicted++;
                    break;

                case SvnNotifyState.Merged:
                    if (AddContent(path, MergeResultActionKind.Merged))
                        FileMerged++;
                    break;
            }
        }

        void ApplyPropertyState(string path, SvnNotifyState state)
        {
            switch (state)
            {
                case SvnNotifyState.Changed:
                    if (AddProperty(path, MergeResultActionKind.Modified))
                        PropertyUpdated++;
                    break;

                case SvnNotifyState.Conflicted:
                    if (AddProperty(path, MergeResultActionKind.Conflicted))
                        PropertyConflicted++;
                    break;

                case SvnNotifyState.Merged:
                    if (AddProperty(path, MergeResultActionKind.Merged))
                        PropertyMerged++;
                    break;
            }
        }

        bool AddContent(string path, MergeResultActionKind action)
        {
            return GetPath(path).AddContent(action);
        }

        bool AddProperty(string path, MergeResultActionKind action)
        {
            return GetPath(path).AddProperty(action);
        }

        MergePathResult GetPath(string path)
        {
            MergePathResult result;
            if (!_paths.TryGetValue(path, out result))
            {
                result = new MergePathResult(path);
                _paths.Add(path, result);
            }

            return result;
        }

        public static string FormatActions(IList<MergeResultActionKind> actions)
        {
            if (actions == null || actions.Count == 0)
                return MergeStrings.Unchanged;

            string[] text = new string[actions.Count];
            for (int i = 0; i < actions.Count; i++)
                text[i] = GetActionText(actions[i]);

            return String.Join(", ", text);
        }

        static string GetActionText(MergeResultActionKind action)
        {
            switch (action)
            {
                case MergeResultActionKind.Existed:
                    return MergeStrings.Existed;
                case MergeResultActionKind.Skipped:
                    return MergeStrings.Skipped;
                case MergeResultActionKind.Added:
                    return MergeStrings.Added;
                case MergeResultActionKind.Deleted:
                    return MergeStrings.Deleted;
                case MergeResultActionKind.Replaced:
                    return MergeStrings.Replaced;
                case MergeResultActionKind.Modified:
                    return MergeStrings.Modified;
                case MergeResultActionKind.Conflicted:
                    return MergeStrings.Conflicted;
                case MergeResultActionKind.Merged:
                    return MergeStrings.Merged;
                default:
                    throw new ArgumentOutOfRangeException("action");
            }
        }
    }
}
