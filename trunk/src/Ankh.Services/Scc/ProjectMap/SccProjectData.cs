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
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using SharpSvn;
using Ankh.Configuration;
using Ankh.Selection;
using Ankh.VS;


namespace Ankh.Scc.ProjectMap
{
    [Flags]
    public enum SccProjectFlags
    {
        None,
        WebLikeFileHandling = 0x0010,
        ForceSccGlyphChange = 0x0100,
        StoredInSolution = 0x1000,
        SolutionInfrastructure = 0x2000
    }

    public enum SccEnlistChoice
    {
        Never,
        Optional,
        Required
    }

    [DebuggerDisplay("Project={ProjectName}, Flags={_projectFlags}")]
    public sealed partial class SccProjectData : IDisposable
    {
        readonly IAnkhServiceProvider _context;
        readonly IVsSccProject2 _sccProject;
        readonly IVsHierarchy _hierarchy;
        readonly IVsProject _vsProject;
        readonly SccProjectFlags _projectFlags;
        readonly SccProjectFileCollection _files;
        bool _isManaged;
        bool _isRegistered;
        bool _loaded;
        string _projectFile;
        bool _checkedProjectFile;
        readonly SccProjectMap _map;
        SccProject _svnProjectInstance;
        string _projectLocation;
        string _projectName;
        string _projectDirectory;
        Guid? _projectGuid;
        Guid? _projectTypeGuid;
        bool _unloading;

        [CLSCompliant(false)]
        public SccProjectData(SccProjectMap context, IVsSccProject2 project)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (project == null)
                throw new ArgumentNullException("project");

            _context = context;
            _map = context;

            // Project references to speed up marshalling
            _sccProject = project;
            _hierarchy = (IVsHierarchy)project; // A project must be a hierarchy in VS
            _vsProject = (IVsProject)project; // A project must be a VS project

            _projectFlags = GetProjectFlags(ProjectTypeGuid);
            _files = new SccProjectFileCollection();
        }

        [DebuggerStepThrough]
        private T GetService<T>()
            where T : class
        {
            return _context.GetService<T>();
        }

        [DebuggerStepThrough]
        private T GetService<T>(Type serviceType)
            where T : class
        {
            return _context.GetService<T>(serviceType);
        }

        [CLSCompliant(false)]
        public IVsSccProject2 SccProject
        {
            get { return _sccProject; }
        }

        [CLSCompliant(false)]
        public IVsProject VsProject
        {
            get { return _vsProject; }
        }

        [CLSCompliant(false)]
        public IVsHierarchy ProjectHierarchy
        {
            get { return _hierarchy; }
        }

        public bool IsManaged
        {
            get { return _isManaged; }

            // Called by IVsSccManager.RegisterSccProject() when we were preregistered
            set { _isManaged = value; }
        }

        public string ProjectName
        {
            get
            {
                if (_projectName == null && _hierarchy != null)
                {
                    _projectName = "";
                    object name;

                    if (VSErr.Succeeded(_hierarchy.GetProperty(VSItemId.Root, (int)__VSHPROPID.VSHPROPID_Name, out name)))
                    {
                        _projectName = name as string;
                    }
                }

                return _projectName;
            }
        }

        /// <summary>
        /// Gets the guid of the project within the solution
        /// </summary>
        public Guid ProjectGuid
        {
            get
            {
                if (!_projectGuid.HasValue)
                {
                    IVsSolution solution = GetService<IVsSolution>(typeof(SVsSolution));

                    Guid value;
                    if (VSErr.Succeeded(solution.GetGuidOfProject(ProjectHierarchy, out value)))
                        _projectGuid = value;
                }

                return _projectGuid.HasValue ? _projectGuid.Value : Guid.Empty;
            }
        }

        /// <summary>
        /// Gets the guid of the project within the solution
        /// </summary>
        public Guid ProjectTypeGuid
        {
            get
            {
                if (!_projectTypeGuid.HasValue)
                {
                    Guid value;
                    if (VSErr.Succeeded(_hierarchy.GetGuidProperty(VSItemId.Root, (int)__VSHPROPID.VSHPROPID_TypeGuid, out value)))
                        _projectTypeGuid = value;
                }

                return _projectTypeGuid.HasValue ? _projectTypeGuid.Value : Guid.Empty;
            }
        }

        /// <summary>
        /// Gets the project directory.
        /// </summary>
        /// <value>The project directory or null if the project does not have one</value>
        public string ProjectDirectory
        {
            get
            {
                if (_projectDirectory == null && _hierarchy != null)
                {
                    _projectDirectory = "";
                    object name;

                    if (VSErr.Succeeded(_hierarchy.GetProperty(VSItemId.Root, (int)__VSHPROPID.VSHPROPID_ProjectDir, out name)))
                    {
                        string dir = name as string;

                        if (!string.IsNullOrEmpty(dir) && SvnItem.IsValidPath(dir, true))
                            dir = SvnTools.GetNormalizedFullPath(dir);
                        else
                        {
                            // Ok, we have users reporting they get here via Analysis services

                            // Wild guess: If we have a valid project file assume its folder 
                            //              is the project directory.
                            dir = ProjectFile;

                            if (dir != null)
                                dir = SvnTools.GetNormalizedDirectoryName(dir);
                            else
                                dir = ""; // Cache as invalid
                        }

                        _projectDirectory = dir;
                    }
                }

                return String.IsNullOrEmpty(_projectDirectory) ? null : _projectDirectory;
            }
        }

        public string ProjectLocation
        {
            get
            {
                GC.KeepAlive(ProjectFile);
                return _projectLocation;
            }
        }

        public string ProjectFile
        {
            get
            {
                if (!_checkedProjectFile && _vsProject != null)
                {
                    _checkedProjectFile = true;
                    string name;

                    if (VSErr.Succeeded(_vsProject.GetMkDocument(VSItemId.Root, out name)))
                    {
                        _projectLocation = name;
                        if (SvnItem.IsValidPath(name, true))
                            _projectFile = name;
                    }
                }
                return _projectFile;
            }
        }

        string _sccBaseDirectory;
        /// <summary>
        /// Gets or sets the SCC base directory.
        /// </summary>
        /// <value>The SCC base directory.</value>
        public string SccBaseDirectory
        {
            get { return _sccBaseDirectory ?? (_sccBaseDirectory = GetSccBaseDirectory()); }
            set { _sccBaseDirectory = value; }
        }

        string GetSccBaseDirectory()
        {
            string projectDir = ProjectDirectory;

            if (projectDir == null)
                return null;

            // TODO: Insert project local settings check

            IAnkhSolutionSettings settings = GetService<IAnkhSolutionSettings>();
            IFileStatusCache cache = GetService<IFileStatusCache>();

            SvnItem solutionRoot = settings.ProjectRootSvnItem;

            if (solutionRoot == null)
                return null; // Fix the solution before we try any SCC, thanks!

            SvnItem projectDirItem = cache[projectDir];

            if (projectDirItem != null && projectDirItem.WorkingCopy != null)
            {
                // Project is below standard workingcopy
                if (projectDirItem.IsBelowPath(solutionRoot))
                {
                    if (solutionRoot.WorkingCopy == projectDirItem.WorkingCopy)
                    {
                        // Project is in the same working copy.. use the solution root
                        // to automatically handle in-between directory levels
                        return solutionRoot.FullPath;
                    }

                    // Project has its own workingcopy below the solution root
                    // -> Use the complete workingcopy (might be shared with multiple projects)
                    return projectDirItem.WorkingCopy.FullPath;
                }

                if (solutionRoot.WorkingCopy != projectDirItem.WorkingCopy)
                {
                    // Project is below a root of its own.. use its workingcopy
                    return projectDirItem.WorkingCopy.FullPath;
                }

                // The user deliberately choose not to have the item in the solution root
                // -> Default to the project directory                
            }

            return ProjectDirectory;
        }

        public bool IsSccBindable
        {
            get
            {
                IVsSccProjectProviderBinding providerBinding = VsProject as IVsSccProjectProviderBinding;

                VSSCCPROVIDERBINDING[] ppb = new VSSCCPROVIDERBINDING[1];
                if (providerBinding != null &&
                    VSErr.Succeeded(providerBinding.GetProviderBinding(ppb)))
                {
                    VSSCCPROVIDERBINDING pb = ppb[0];

                    if (pb == VSSCCPROVIDERBINDING.VSSCC_PB_CUSTOM_DISABLED || pb == VSSCCPROVIDERBINDING.VSSCC_PB_STANDARD_DISABLED)
                        return false;
                }

                return !string.IsNullOrEmpty(ProjectDirectory);
            }
        }

        string _uniqueName;
        public string UniqueProjectName
        {
            get
            {
                if (_uniqueName != null)
                    return _uniqueName;
                IVsSolution3 solution = GetService<IVsSolution3>(typeof(SVsSolution));

                if (solution != null)
                {
                    string name;

                    if (VSErr.Succeeded(solution.GetUniqueUINameOfProject(ProjectHierarchy, out name)))
                        _uniqueName = name;
                }
                return _uniqueName ?? ProjectName;
            }
        }

        public SccProject SvnProject
        {
            get
            {
                if (_svnProjectInstance == null)
                {
                    if ((_projectFlags & SccProjectFlags.ForceSccGlyphChange) == SccProjectFlags.ForceSccGlyphChange)
                        _svnProjectInstance = Map.CreateProject(this);
                    else
                        _svnProjectInstance = new SccProject(ProjectFile, SccProject);
                }

                return _svnProjectInstance;
            }
        }

        public SccProjectFlags SccFlags
        {
            get { return _projectFlags; }
        }

        /// <summary>
        /// Checks whether the project might have been renamed
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        [CLSCompliant(false)]
        public void CheckProjectRename(IVsSccProject2 project, string oldName, string newName)
        {
            if (_checkedProjectFile && oldName == ProjectFile)
            {
                _checkedProjectFile = false;
                _projectFile = null;
                _svnProjectInstance = null;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the project thinks its registered on the scc provider
        /// </summary>
        /// <remarks>
        /// True if the project registered itself on the Scc provider, otherwise false
        /// </remarks>
        public bool IsRegistered
        {
            get { return _isRegistered; }
            set { _isRegistered = value; }
        }

        public bool IsStoredInSolution
        {
            get { return (_projectFlags & SccProjectFlags.StoredInSolution) != SccProjectFlags.None; }
        }

        public bool WebLikeFileHandling
        {
            get { return (_projectFlags & SccProjectFlags.WebLikeFileHandling) != SccProjectFlags.None; }
        }

        public bool IsSolutionInfrastructure
        {
            get { return (_projectFlags & SccProjectFlags.SolutionInfrastructure) != SccProjectFlags.None; }
        }

        public void SetManaged(bool managed)
        {
            if (managed == IsManaged)
                return;

            bool ok;

            if (managed)
            {
                // Set some constant strings as marker
                ok = VSErr.Succeeded(SccProject.SetSccLocation("Svn", "Svn", "Svn", AnkhId.SubversionSccName));
            }
            else
            {
                // The managed package framework assumes empty strings for clearing; null will fail there
                ok = VSErr.Succeeded(SccProject.SetSccLocation("", "", "", ""));
            }

            if (ok)
            {
                IsManaged = managed;
            }
        }

        public void OnClose()
        {
            Hook(false);
            while (_files.Count > 0)
            {
                SccProjectFileReference r = _files[0];
                while (r.ReferenceCount > 0)
                {
                    r.ReleaseReference();
                }
            }
            _loaded = false;
        }

        bool _disposed;
        public void Dispose()
        {
            _disposed = true;
            Hook(false);
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        bool _opened;
        public void OnOpened()
        {
            _opened = true;
        }

        bool _inLoad;
        public void Load()
        {
            if (_loaded || !_opened)
                return;

            _inLoad = true;
            try
            {
                Debug.Assert(_files.Count == 0);

                _checkedProjectFile = false;
                _projectFile = null;
                _svnProjectInstance = null;
                _loaded = true;

                ISccProjectWalker walker = GetService<ISccProjectWalker>();

                if (walker != null)
                {
                    Dictionary<string, uint> ids = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
                    foreach (string file in walker.GetSccFiles(ProjectHierarchy, VSItemId.Root, ProjectWalkDepth.AllDescendantsInHierarchy, ids))
                    {
                        AddPath(file, ids); // GetSccFiles returns normalized paths
                    }
                }

                IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();
                if (monitor != null)
                {
                    // Make sure we see all files as possible pending changes
                    monitor.ScheduleGlyphUpdate(GetAllFiles());
                }
                Hook(true);
            }
            finally
            {
                _inLoad = false;
            }
        }

        public void Reload()
        {
            _projectName = null;
            _uniqueName = null;
            OnClose();
            Load();
        }

        public bool TrackProjectChanges()
        {
            return _loaded && !_inLoad;
        }

        public IEnumerable<string> GetAllFiles()
        {
            foreach (SccProjectFileReference r in _files)
            {
                yield return r.Filename;
            }
        }

        public void AddPath(string path)
        {
            bool alreadyLoaded = _loaded && !_inLoad;

            if (alreadyLoaded && WebLikeFileHandling)
            {
                uint fid = VSItemId.Nil;

                if (TryGetProjectFileId(path, out fid))
                {
                    // OK: Websites try to add everything they see below their directory
                    // We only add these files if they are actually SCC items

                    bool add = false;
                    ISccProjectWalker walker = GetService<ISccProjectWalker>();

                    foreach (string file in walker.GetSccFiles(ProjectHierarchy, fid, ProjectWalkDepth.SpecialFiles, null))
                    {
                        if (string.Equals(file, path, StringComparison.OrdinalIgnoreCase))
                        {
                            add = true;
                            break;
                        }
                    }

                    if (!add)
                        return;
                }
                else
                {
                    // File is either not in project or a scc special file
                    // Pass
                }
            }

            if (alreadyLoaded && GetService<IAnkhConfigurationService>().Instance.AutoAddEnabled)
            {
                GetService<IFileStatusMonitor>().ScheduleAddFile(path);
            }

            SccProjectFileReference reference;
            if (_files.TryGetValue(path, out reference))
                reference.AddReference();
            else
            {
                reference = new SccProjectFileReference(_context, this, Map.GetFile(path));
                _files.Add(reference);

                if (alreadyLoaded)
                    GetService<IFileStatusMonitor>().ScheduleGlyphUpdate(path);
            }

            if (string.Equals(path, ProjectFile, StringComparison.OrdinalIgnoreCase))
            {
                reference.IsProjectFile = true;
            }

            if (alreadyLoaded && !string.IsNullOrEmpty(ProjectFile))
            {
                ISccProjectWalker walker = GetService<ISccProjectWalker>();

                if (walker != null)
                    walker.SetPrecreatedFilterItem(null, VSItemId.Nil);

                ClearIdCache();

                SetDirty();
            }
        }

        private void AddPath(string path, Dictionary<string, uint> ids)
        {
            AddPath(path);

            uint id;
            if (ids != null && ids.TryGetValue(path, out id))
            {
                SccProjectFileReference pf;
                if (_files.TryGetValue(path, out pf))
                    pf.SetId(id);
            }
        }

        public void RemovePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccProjectFileReference pf;
            if (!_files.TryGetValue(path, out pf))
                return;

            pf.ReleaseReference();

            ClearIdCache();

            if (!_inLoad && _loaded && !string.IsNullOrEmpty(ProjectFile))
            {
                SetDirty();
            }
        }

        #region File list management code
        internal void InvokeRemoveReference(SccProjectFileReference sccProjectFileReference)
        {
            _files.Remove(sccProjectFileReference);
        }
        #endregion


        #region Helper code

        SccProjectMap Map
        {
            get { return _map; }
        }

        SccEnlistChoice? _sccEnlistChoice;
        public SccEnlistChoice EnlistMode
        {
            get
            {
                if (!_sccEnlistChoice.HasValue)
                {
                    IVsSccProjectEnlistmentChoice pec = VsProject as IVsSccProjectEnlistmentChoice;

                    VSSCCENLISTMENTCHOICE[] choiceList = new VSSCCENLISTMENTCHOICE[1];
                    if (pec == null || !VSErr.Succeeded(pec.GetEnlistmentChoice(choiceList)))
                    {
                        _sccEnlistChoice = SccEnlistChoice.Never;

                        return SccEnlistChoice.Never; // 99% case. Only Websites appear to use this
                    }

                    VSSCCENLISTMENTCHOICE choice = choiceList[0];

                    switch (choice)
                    {
                        case VSSCCENLISTMENTCHOICE.VSSCC_EC_NEVER:
                        default:
                            _sccEnlistChoice = SccEnlistChoice.Never;
                            break; // Valid for most projects
                        case VSSCCENLISTMENTCHOICE.VSSCC_EC_OPTIONAL:
                            _sccEnlistChoice = SccEnlistChoice.Optional;
                            break;
                        case VSSCCENLISTMENTCHOICE.VSSCC_EC_COMPULSORY:
                            _sccEnlistChoice = SccEnlistChoice.Required;
                            break;
                    }
                }

                return _sccEnlistChoice.Value;
            }
        }

        // Switch to dictionary when we have more than a handfull of items
        static SortedList<Guid, SccProjectFlags> _projectFlagMap = new SortedList<Guid, SccProjectFlags>();
        static bool _projectFlagMapLoaded;
        SccProjectFlags GetProjectFlags(Guid projectId)
        {
            lock (_projectFlagMap)
            {
                if (!_projectFlagMapLoaded)
                    LoadProjectFlagMap();

                SccProjectFlags flags;
                if (!_projectFlagMap.TryGetValue(projectId, out flags))
                    return SccProjectFlags.None;

                return flags;
            }
        }

        void LoadProjectFlagMap()
        {
            _projectFlagMapLoaded = true;

            IAnkhConfigurationService configService = GetService<IAnkhConfigurationService>();

            if (configService == null)
                return;

            using (RegistryKey projectHandlingKey = configService.OpenVSInstanceKey("Extensions\\AnkhSVN\\ProjectHandling"))
            {
                if (projectHandlingKey == null)
                    return;

                foreach (string typeValue in projectHandlingKey.GetSubKeyNames())
                {
                    if (typeValue.Length != 38) // No proper guid
                        continue;

                    try
                    {
                        using (RegistryKey projectTypeKey = projectHandlingKey.OpenSubKey(typeValue))
                        {
                            object v = projectTypeKey.GetValue("flags");

                            if (!(v is int))
                                continue;

                            Guid projectType = new Guid(typeValue);
                            SccProjectFlags flags = (SccProjectFlags)(int)v;
                            _projectFlagMap.Add(projectType, flags);
                        }
                    }
                    catch
                    { /* Parse Error */ }
                }
            }
        }
        #endregion

        /// <summary>
        /// Determines whether the project contains the specified file
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the project contains the file; otherwise, <c>false</c>.
        /// </returns>
        internal bool ContainsFile(string path)
        {
            return _files.Contains(path);
        }

        void ClearIdCache()
        {
            foreach (SccProjectFileReference r in _files)
            {
                r.ClearIdCache();
            }
        }

        bool _fetchedImgList;
        IntPtr _projectImgList;
        internal IntPtr ProjectImageList
        {
            get
            {
                if (_fetchedImgList)
                    return _projectImgList;

                _fetchedImgList = true;
                object value;
                if (VSErr.Succeeded(ProjectHierarchy.GetProperty(VSItemId.Root,
                    (int)__VSHPROPID.VSHPROPID_IconImgList, out value)))
                {
                    _projectImgList = (IntPtr)(int)value; // Marshalled by VS as 32 bit integer
                }

                return _projectImgList;
            }
        }

        /// <summary>
        /// Tries to get the hierarchy id of a file within a project
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public bool TryGetProjectFileId(string path, out uint itemId)
        {
            int found;
            uint id;
            VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];

            if (VSErr.Succeeded(
                VsProject.IsDocumentInProject(path, out found, prio, out id)))
            {
                // Priority also returns information on whether the file can be added
                if (found != 0 && prio[0] >= VSDOCUMENTPRIORITY.DP_Standard && id != 0)
                {
                    itemId = id;
                    return true;
                }
            }

            itemId = VSItemId.Nil;
            return false;
        }

        public override string ToString()
        {
            return UniqueProjectName;
        }

        public bool Unloading
        {
            get { return _unloading; }
            set { _unloading = value; }
        }

        [DebuggerNonUserCode]
        public void NotifyGlyphsChanged()
        {
            try
            {
                if ((_projectFlags & SccProjectFlags.ForceSccGlyphChange) == 0)
                    SccProject.SccGlyphChanged(0, null, null, null);
                else
                    ForceGlyphChanges();
            }
            catch { }
        }

        internal void ForceGlyphChanges()
        {
            uint[] idsArray;
            string[] namesArray;
            {
                List<uint> ids = new List<uint>(_files.Count);
                List<string> names = new List<string>(_files.Count);

                foreach (SccProjectFileReference r in _files)
                {
                    uint id = r.ProjectItemId;
                    if (id == VSItemId.Nil)
                        continue;

                    string name = r.ProjectFile.FullPath;
                    if (string.IsNullOrEmpty(name))
                        continue;

                    ids.Add(id);
                    names.Add(name);
                }
                idsArray = ids.ToArray();
                namesArray = names.ToArray();
            }

            VsStateIcon[] newGlyphs = new VsStateIcon[idsArray.Length];
            uint[] sccState = new uint[idsArray.Length];

            if (0 == Map.GetSccGlyph(namesArray, newGlyphs, sccState))
                SccProject.SccGlyphChanged(idsArray.Length, idsArray, newGlyphs, sccState);
        }
    }
}
