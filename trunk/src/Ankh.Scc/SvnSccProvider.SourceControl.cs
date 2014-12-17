﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;

using Ankh.Commands;

using EnvDTESourceControl = EnvDTE.SourceControl;
using EnvDTESourceControl2 = EnvDTE80.SourceControl2;
using System.IO;

namespace Ankh.Scc
{
    partial class SvnSccProvider
    {
        public void SerializeSccExcludeData(System.IO.Stream store, bool writeData)
        {
            ProjectMap.SerializeSccExcludeData(store, writeData);
        }

        protected override bool CheckOutItems(string[] itemNames, EnvDTE80.vsSourceControlCheckOutOptions flags)
        {
            HybridCollection<string> mustLockItems = null;

            foreach (string item in itemNames)
            {
                if (!IsSafeSccPath(item))
                    continue;

                SvnItem svnItem = StatusCache[item];

                if (svnItem.IsReadOnlyMustLock)
                {
                    if (mustLockItems == null)
                        mustLockItems = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

                    mustLockItems.Add(svnItem.FullPath);
                }
            }

            if (mustLockItems == null)
                return true;

            CommandService.DirectlyExecCommand(AnkhCommand.SccLock, mustLockItems, CommandPrompt.DoDefault);
            // Only check the original list; the rest of the items in mustLockItems is optional
            foreach (string item in mustLockItems)
            {
                if (StatusCache[item].IsReadOnlyMustLock)
                {
                    // User has probably canceled the lock operation, or it failed.
                    return false;
                }
            }

            return true;
        }

        protected override void ExcludeItems(string projectFile, string[] itemNames)
        {
            List<string> changed = null;
            foreach (string item in itemNames)
            {
                if (!IsSafeSccPath(item))
                    continue;

                string path = SvnTools.GetNormalizedFullPath(item);

                if (!ProjectMap.IsSccExcluded(path))
                {
                    ProjectMap.SccExclude(path);

                    if (changed == null)
                        changed = new List<string>();
                    changed.Add(path);

                    StatusCache.SetSolutionContained(path, ProjectMap.ContainsFile(path), true);
                }
            }

            if (changed != null)
                PendingChanges.Refresh(changed);
        }

        protected override void UndoExcludeItems(string projectFile, string[] itemNames)
        {
            List<string> changed = null;
            foreach (string item in itemNames)
            {
                if (!IsSafeSccPath(item))
                    continue;

                string path = SvnTools.GetNormalizedFullPath(item);

                if (ProjectMap.SccRemoveExcluded(path))
                {
                    if (changed == null)
                        changed = new List<string>();
                    changed.Add(path);
                }
            }

            if (changed != null)
                PendingChanges.Refresh(changed);
        }

        sealed class AnkhSccSourceControlBinding : EnvDTE80.SourceControlBindings
        {
            readonly SvnSccProvider _provider;
            readonly string _path;


            internal AnkhSccSourceControlBinding(SvnSccProvider provider, string path)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                else if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException("path");

                _provider = provider;
                _path = path;
            }

            public EnvDTESourceControl Parent
            {
                get { return _provider; }
            }

            public EnvDTE.DTE DTE
            {
                get { return Parent.DTE; }
            }

            SvnItem SvnItem
            {
                get { return _provider.StatusCache[_path]; }
            }

            public string ProviderName
            {
                get { return AnkhId.SubversionSccName; }
            }

            SvnWorkingCopy WorkingCopy
            {
                get
                {
                    SvnItem item = SvnItem;
                    if (item != null)
                        return item.WorkingCopy;

                    return null;
                }
            }

            public string LocalBinding
            {
                get 
                {
                    SvnWorkingCopy wc = WorkingCopy;

                    if (wc != null)
                        return wc.FullPath;
                    
                    return null;
                }
            }

            public string ProviderRegKey
            {
                get { return ""; }
            }

            public string ServerBinding
            {
                get 
                {
                    SvnItem item = SvnItem;

                    if (item != null)
                        return item.Uri.AbsoluteUri;

                    return null;
                }
            }

            public string ServerName
            {
                get
                {
                    SvnWorkingCopy wc = WorkingCopy;

                    if (wc != null && wc.RepositoryRoot != null)
                        return wc.RepositoryRoot.AbsoluteUri;

                    return null;
                }
            }
        }

        public override EnvDTE80.SourceControlBindings GetBindings(string ItemPath)
        {
            if (!IsSafeSccPath(ItemPath))
                return null;

            return new AnkhSccSourceControlBinding(this, SvnTools.GetNormalizedFullPath(ItemPath));
        }

        public override bool IsItemCheckedOut(string ItemName)
        {
            if (!IsSafeSccPath(ItemName))
                return false;

            SvnItem item = StatusCache[ItemName];

            if (item == null || !item.IsVersionable)
                return false;

            return item.IsVersioned || (item.InSolution && item.IsVersionable && !item.IsSccExcluded);
        }

        public override bool IsItemUnderSCC(string ItemName)
        {
            return true;
        }
    }
}
