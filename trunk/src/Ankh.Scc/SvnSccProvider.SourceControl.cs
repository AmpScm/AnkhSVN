using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;

using Ankh.Commands;

using EnvDTESourceControl = EnvDTE.SourceControl;
using EnvDTESourceControl2 = EnvDTE80.SourceControl2;
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace Ankh.Scc
{
    partial class SvnSccProvider : EnvDTESourceControl2, EnvDTESourceControl
    {
        readonly HybridCollection<string> _sccExcluded = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

        public void SerializeSccExcludeData(System.IO.Stream store, bool writeData)
        {
            LoadSolutionInfo(); // Force the right data
            string baseDir = SolutionDirectory;

            if (writeData)
            {
                if (_sccExcluded.Count == 0)
                    return;

                using (BinaryWriter bw = new BinaryWriter(store))
                {
                    bw.Write(_sccExcluded.Count);

                    foreach (string path in _sccExcluded)
                    {
                        if (baseDir != null)
                            bw.Write(PackageUtilities.MakeRelative(baseDir, path));
                        else
                            bw.Write(path);
                    }
                }
            }
            else
            {
                if (store.Length == 0)
                    return;

                using (BinaryReader br = new BinaryReader(store))
                {
                    int count = br.ReadInt32();

                    while (count-- > 0)
                    {
                        string path = br.ReadString();

                        if (baseDir != null)
                            path = SvnTools.GetNormalizedFullPath(Path.Combine(baseDir, path));
                        else
                            path = SvnTools.GetNormalizedFullPath(path);

                        if (!_sccExcluded.Contains(path))
                            _sccExcluded.Add(path);
                    }
                }
            }
        }



        public bool IsSccExcluded(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            return _sccExcluded.Contains(path);
        }

        public bool CheckOutItem(string ItemName)
        {
            return CheckOutItem2(ItemName, EnvDTE80.vsSourceControlCheckOutOptions.vsSourceControlCheckOutOptionLocalVersion);
        }

        public bool CheckOutItem2(string ItemName, EnvDTE80.vsSourceControlCheckOutOptions Flags)
        {
            if (string.IsNullOrEmpty(ItemName))
                return false;

            object[] items = new string[] { ItemName };
            return CheckOutItems2(ref items, Flags);
        }

        public bool CheckOutItems(ref object[] ItemNames)
        {
            return CheckOutItems2(ref ItemNames, EnvDTE80.vsSourceControlCheckOutOptions.vsSourceControlCheckOutOptionLocalVersion);
        }

        public bool CheckOutItems2(ref object[] ItemNames, EnvDTE80.vsSourceControlCheckOutOptions Flags)
        {
            HybridCollection<string> mustLockItems = null;

            foreach (string item in ItemNames)
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

        public void ExcludeItem(string ProjectFile, string ItemName)
        {
            if (string.IsNullOrEmpty(ItemName))
                return;

            object[] items = new string[] { ItemName };
            ExcludeItems(ProjectFile, ref items);
        }

        public void ExcludeItems(string ProjectFile, ref object[] ItemNames)
        {
            List<string> changed = null;
            foreach (string item in ItemNames)
            {
                if (!IsSafeSccPath(item))
                    continue;

                string path = SvnTools.GetNormalizedFullPath(item);

                if (!_sccExcluded.Contains(path))
                {
                    _sccExcluded.Add(path);

                    if (changed == null)
                        changed = new List<string>();
                    changed.Add(path);

                    StatusCache.SetSolutionContained(path, _fileMap.ContainsKey(path), _sccExcluded.Contains(path));
                }
            }

            if (changed != null)
                PendingChanges.Refresh(changed);
        }

        public void UndoExcludeItem(string ProjectFile, string ItemName)
        {
            if (string.IsNullOrEmpty(ItemName))
                return;

            object[] items = new string[] { ItemName };
            UndoExcludeItems(ProjectFile, ref items);
        }

        public void UndoExcludeItems(string ProjectFile, ref object[] ItemNames)
        {
            List<string> changed = null;
            foreach (string item in ItemNames)
            {
                if (!IsSafeSccPath(item))
                    continue;

                string path = SvnTools.GetNormalizedFullPath(item);

                if (_sccExcluded.Remove(path))
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

        public EnvDTE80.SourceControlBindings GetBindings(string ItemPath)
        {
            if (!IsSafeSccPath(ItemPath))
                return null;

            return new AnkhSccSourceControlBinding(this, SvnTools.GetNormalizedFullPath(ItemPath));
        }

        public bool IsItemCheckedOut(string ItemName)
        {
            if (!IsSafeSccPath(ItemName))
                return false;

            SvnItem item = StatusCache[ItemName];

            if (item == null || !item.IsVersionable)
                return false;

            return item.IsVersioned || (item.InSolution && item.IsVersionable && !item.IsSccExcluded);
        }

        public bool IsItemUnderSCC(string ItemName)
        {
            return true;
        }                

        #region DTE properties
        EnvDTE.DTE EnvDTESourceControl.DTE
        {
            get { return GetService<EnvDTE.DTE>(typeof(SDTE)); }
        }

        EnvDTE.DTE EnvDTESourceControl2.DTE
        {
            get { return GetService<EnvDTE.DTE>(typeof(SDTE)); }
        }

        EnvDTE.DTE EnvDTESourceControl.Parent
        {
            get { return GetService<EnvDTE.DTE>(typeof(SDTE)); }
        }

        EnvDTE.DTE EnvDTESourceControl2.Parent
        {
            get { return GetService<EnvDTE.DTE>(typeof(SDTE)); }
        }
        #endregion
    }
}
