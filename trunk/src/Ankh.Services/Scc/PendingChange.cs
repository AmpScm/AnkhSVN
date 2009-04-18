// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.VS;
using SharpSvn;
using System.IO;

namespace Ankh.Scc
{
    [DebuggerDisplay("File={FullPath}, Change={ChangeText}")]
    public sealed class PendingChange : SvnItemData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PendingChange"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="item">The item.</param>
        public PendingChange(RefreshContext context, SvnItem item)
            : base(context, item)
        {
            Refresh(context, item);
        }  

        [DisplayName("Project"), Category("Visual Studio")]
        public new string Project
        {
            get { return _projects; }
        }

        [DisplayName("Change"), Category("Subversion")]
        public string ChangeText
        {
            get { return _status.Text; }
        }

        protected override string ClassName
        {
            get { return "Pending Change"; }
        }

        protected override string ComponentName
        {
            get { return Name; }
        } 

        /// <summary>
        /// Gets a boolean indicating whether this pending change is clear / is no longer a pending change
        /// </summary>
        [Browsable(false)]
        public bool IsClean
        {
            get { return !PendingChange.IsPending(SvnItem); }
        }

        /// <summary>
        /// Gets a boolean indicating this change should belong to an "ignore" change list.
        /// </summary>
        [Browsable(false)]
        public bool IgnoreOnCommit
        {
            get { return (IsIgnoreOnCommitChangeList(ChangeList)); }
        }

        int _iconIndex;
        string _projects;
        string _relativePath;
        PendingChangeStatus _status;
        PendingChangeKind _kind;
        string _changeList;
        string _fileType;

        [Browsable(false)]
        public int IconIndex
        {
            get { return _iconIndex; }
        }

        [Browsable(false)]
        public PendingChangeKind Kind
        {
            get { return _kind; }
        }

        [Browsable(false)]
        public new PendingChangeStatus Change
        {
            get { return _status; }
        }

        [Browsable(false)]
        public string RelativePath
        {
            get { return _relativePath; }
        }

        [Browsable(false)]
        public string FileType
        {
            get { return _fileType; }
        }

        [Browsable(false)]
        public string LogMessageToolTipText
        {
            get { return String.Format("{0}: {1}", RelativePath, Change.PendingCommitText); }
        }

        /// <summary>
        /// Refreshes the pending change. Returns true if the state was modified, otherwise false
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Refresh(RefreshContext context, SvnItem item)
        {
            bool m = false;

            RefreshValue(ref m, ref _iconIndex, GetIcon(context));
            RefreshValue(ref m, ref _projects, GetProjects(context));
            RefreshValue(ref m, ref _status, GetStatus(context, item));
            RefreshValue(ref m, ref _relativePath, GetRelativePath(context));
            RefreshValue(ref m, ref _changeList, SvnItem.Status.ChangeList);
            RefreshValue(ref m, ref _fileType, GetFileType(context, item));

            return m || (_status == null);
        }

        private string GetFileType(RefreshContext context, SvnItem item)
        {
            return context.IconMapper.GetFileType(item.FullPath);
        }        

        private string GetRelativePath(RefreshContext context)
        {
            string projectRoot = context.SolutionSettings.ProjectRootWithSeparator;

            if (!string.IsNullOrEmpty(projectRoot) && FullPath.StartsWith(projectRoot, StringComparison.OrdinalIgnoreCase))
                return FullPath.Substring(projectRoot.Length).Replace('\\', '/');
            else
                return FullPath;
        }

        string GetProjects(RefreshContext context)
        {
            string name = null;
            foreach (SvnProject project in context.ProjectFileMapper.GetAllProjectsContaining(FullPath))
            {
                ISvnProjectInfo info = context.ProjectFileMapper.GetProjectInfo(project);

                if (info == null)
                {
                    // Handle the case the solution file is in a project (Probably website)
                    if (string.Equals(FullPath, context.SolutionSettings.SolutionFilename))
                        return "<Solution>";
                    continue;
                }

                if (name != null)
                    name += ";" + info.UniqueProjectName;
                else
                    name = info.UniqueProjectName;
            }

            if (!string.IsNullOrEmpty(name))
                return name;
            else if (string.Equals(FullPath, context.SolutionSettings.SolutionFilename))
                return "<Solution>";
            else
                return "<none>";
        }

        int GetIcon(RefreshContext context)
        {
            if (SvnItem.Exists)
            {
                if (SvnItem.IsDirectory || SvnItem.NodeKind == SvnNodeKind.Directory)
                    return context.IconMapper.DirectoryIcon; // Is or was a directory
                else
                    return context.IconMapper.GetIcon(FullPath);
            }
            else if (SvnItem.Status != null && SvnItem.Status.NodeKind == SvnNodeKind.Directory)
                return context.IconMapper.DirectoryIcon;
            else
                return context.IconMapper.GetIconForExtension(SvnItem.Extension);
        }

        PendingChangeStatus GetStatus(RefreshContext context, SvnItem item)
        {
            AnkhStatus status = item.Status;
            _kind = CombineStatus(status.LocalContentStatus, status.LocalPropertyStatus, item.IsTreeConflicted, item);

            if (_kind != PendingChangeKind.None)
                return new PendingChangeStatus(_kind);
            else
                return null;
        }

        static void RefreshValue<T>(ref bool changed, ref T field, T newValue)
            where T : class, IEquatable<T>
        {
            if (field == null || !field.Equals(newValue))
            {
                changed = true;
                field = newValue;
            }
        }

        static void RefreshValue(ref bool changed, ref int field, int newValue)
        {
            if (field != newValue)
            {
                changed = true;
                field = newValue;
            }
        }

        public static bool IsPending(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            bool create = false;
            if (item.IsConflicted)
                create = true; // Tree conflict (unversioned) or other conflict
            else if (item.IsModified)
                create = true; // Must commit
            else if (item.InSolution && !item.IsVersioned && !item.IsIgnored && item.IsVersionable)
                create = true; // To be added
            else if (item.IsVersioned && item.IsDocumentDirty)
                create = true;
            else if (item.IsLocked)
                create = true;

            return create;
        }

        /// <summary>
        /// Dtermines if a change list name is one of the "Ignore On Commit" change lists
        /// </summary>
        public static bool IsIgnoreOnCommitChangeList(string name)
        {
            return !string.IsNullOrEmpty(name)
                && ( false
                     || string.Equals(name, "ignore-on-commit")
                     // TODO other "ignore on commit" change lists 
                   )
                ;
        }

        /// <summary>
        /// Creates if pending.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="isDirty">if set to <c>true</c> [is dirty].</param>
        /// <param name="pc">The pc.</param>
        /// <returns></returns>
        public static bool CreateIfPending(RefreshContext context, SvnItem item, out PendingChange pc)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (item == null)
                throw new ArgumentNullException("item");

            if (IsPending(item))
            {
                pc = new PendingChange(context, item);
                return true;
            }

            pc = null;
            return false;
        }

        public sealed class RefreshContext : IAnkhServiceProvider
        {
            readonly IAnkhServiceProvider _context;
            public RefreshContext(IAnkhServiceProvider context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                _context = context;
            }

            IProjectFileMapper _fileMapper;
            IFileIconMapper _iconMapper;
            IAnkhSolutionSettings _solutionSettings;

            public IProjectFileMapper ProjectFileMapper
            {
                [DebuggerStepThrough]
                get { return _fileMapper ?? (_fileMapper = GetService<IProjectFileMapper>()); }
            }

            public IFileIconMapper IconMapper
            {
                [DebuggerStepThrough]
                get { return _iconMapper ?? (_iconMapper = GetService<IFileIconMapper>()); }
            }

            public IAnkhSolutionSettings SolutionSettings
            {
                [DebuggerStepThrough]
                get { return _solutionSettings ?? (_solutionSettings = GetService<IAnkhSolutionSettings>()); }
            }

            #region IAnkhServiceProvider Members
            [DebuggerStepThrough]
            public T GetService<T>() where T : class
            {
                return _context.GetService<T>();
            }

            [DebuggerStepThrough]
            public T GetService<T>(Type serviceType) where T : class
            {
                return _context.GetService<T>(serviceType);
            }

            [DebuggerStepThrough]
            public object GetService(Type serviceType)
            {
                return _context.GetService(serviceType);
            }
            #endregion
        }

        /// <summary>
        /// Determines whether the bool is a local change only (e.g. locked) and does not need user selection
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is cleanup change]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsChangeForPatching()
        {
            switch (Kind)
            {
                case PendingChangeKind.LockedOnly:
                    return true;
                case PendingChangeKind.TreeConflict:
                    return !SvnItem.IsVersioned;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the item is (below) the specified path
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the item is (below) the specified path; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBelowPath(string path)
        {
            return SvnItem.IsBelowPath(path);
        }

        /// <summary>
        /// Combines the statuses to a single PendingChangeKind status for UI purposes
        /// </summary>
        /// <param name="contentStatus">The content status.</param>
        /// <param name="propertyStatus">The property status.</param>
        /// <param name="treeConflict">if set to <c>true</c> [tree conflict].</param>
        /// <param name="item">The item or null if no on disk representation is availavke</param>
        /// <returns></returns>
        public static PendingChangeKind CombineStatus(SvnStatus contentStatus, SvnStatus propertyStatus, bool treeConflict, SvnItem item)
        {
            // item can be null!
            if (treeConflict || (item != null && item.IsTreeConflicted))
                return PendingChangeKind.TreeConflict;
            else if (contentStatus == SvnStatus.Conflicted || propertyStatus == SvnStatus.Conflicted)
                return PendingChangeKind.Conflicted;

            switch (contentStatus)
            {
                case SvnStatus.NotVersioned:
                    if (item != null && item.InSolution)
                        return PendingChangeKind.New;
                    else
                        return PendingChangeKind.None;
                case SvnStatus.Modified:
                    return PendingChangeKind.Modified;
                case SvnStatus.Replaced:
                    return PendingChangeKind.Replaced;
                case SvnStatus.Added:
                    if (item != null && item.HasCopyableHistory)
                        return PendingChangeKind.Copied;

                    return PendingChangeKind.Added;
                case SvnStatus.Deleted:
                    return PendingChangeKind.Deleted;
                case SvnStatus.Missing:
                    if (item != null && item.IsCasingConflicted)
                        return PendingChangeKind.WrongCasing;
                    else
                        return PendingChangeKind.Missing;
                case SvnStatus.Obstructed:
                    return PendingChangeKind.Obstructed;
                case SvnStatus.Incomplete:
                    return PendingChangeKind.Incomplete;
                case SvnStatus.None:
                case SvnStatus.Normal:
                case SvnStatus.Ignored:
                    // No usefull status / No change
                    break;
                case SvnStatus.External:
                    return PendingChangeKind.None;

                case SvnStatus.Zero:
                case SvnStatus.Conflicted:
                case SvnStatus.Merged:
                default: // Give error on missed values
                    throw new ArgumentOutOfRangeException("contentStatus", contentStatus, "Invalid content status");
            }

            switch (propertyStatus)
            {
                case SvnStatus.Modified:
                    return PendingChangeKind.PropertyModified;
                case SvnStatus.Normal:
                case SvnStatus.None:
                    // No usefull status / No change
                    break;
                case SvnStatus.Zero:
                case SvnStatus.Conflicted:
                default: // Give error on missed values
                    throw new ArgumentOutOfRangeException("propertyStatus", propertyStatus, "Invalid content status");
            }

            if (item != null)
            {
                if (item.IsDocumentDirty)
                    return PendingChangeKind.EditorDirty;
                else if (item.IsLocked)
                    return PendingChangeKind.LockedOnly;
            }

            return PendingChangeKind.None;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can be applied to a working copy
        /// </summary>
        /// <value><c>true</c> if this instance can apply; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        public bool CanApply
        {
            get 
            {
                switch (Kind)
                {
                    case PendingChangeKind.New:
                        return true;
                    case PendingChangeKind.WrongCasing:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class PendingChangeCollection : KeyedCollection<string, PendingChange>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PendingChangeCollection"/> class.
        /// </summary>
        public PendingChangeCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Extracts the FullPath from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>The key for the specified element.</returns>
        protected override string GetKeyForItem(PendingChange item)
        {
            return item.FullPath;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out PendingChange value)
        {
            if (Dictionary != null)
                return Dictionary.TryGetValue(key, out value);

            foreach (PendingChange p in this)
            {
                if (String.Equals(p.FullPath, key, StringComparison.OrdinalIgnoreCase))
                {
                    value = p;
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}
