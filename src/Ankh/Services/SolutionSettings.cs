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
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using SharpSvn;
using Ankh.Configuration;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.UI;
using Ankh.VS;

namespace Ankh.Settings
{
    [GlobalService(typeof(IAnkhSolutionSettings))]
    partial class SolutionSettings : AnkhService, IAnkhSolutionSettings
    {
        bool _inRanu = false;
        string _vsUserRoot;
        string _vsAppRoot;
        string _hiveSuffix;

        public SolutionSettings(IAnkhServiceProvider context)
            : base(context)
        {
            IVsShell shell = GetService<IVsShell>(typeof(SVsShell));

            if (shell == null)
                throw new InvalidOperationException("IVsShell not available");

            object r;
            if (VSErr.Succeeded(shell.GetProperty((int)__VSSPROPID.VSSPROPID_VirtualRegistryRoot, out r)))
                _vsUserRoot = (string)r;
            else
                _vsUserRoot = @"SOFTWARE\Microsoft\VisualStudio\8.0";

            string baseName = _vsUserRoot;

            if (_vsUserRoot.EndsWith(@"\UserSettings", StringComparison.OrdinalIgnoreCase))
            {
                _inRanu = true;
                baseName = _vsUserRoot.Substring(0, _vsUserRoot.Length - 13);
                _vsAppRoot = baseName + @"\Configuration";
            }
            else
                _vsAppRoot = _vsUserRoot;

            if (baseName.StartsWith(@"SOFTWARE\", StringComparison.OrdinalIgnoreCase))
                baseName = baseName.Substring(9); // Should always trigger

            if (baseName.StartsWith(@"Microsoft\", StringComparison.OrdinalIgnoreCase))
                baseName = baseName.Substring(10); // Give non-ms hives a prefix

            _hiveSuffix = baseName;
        }

        class SettingsCache
        {
            public string SolutionFilename;
            public string ProjectRoot;
            public Uri ProjectRootUri;
            public SvnItem ProjectRootItem;

            public int SolutionCookie;
            public int RootCookie;

            public Uri RepositoryRoot;

            public bool? BugTrackAppend;
            public string BugTrackLabel;
            public string BugTrackLogRegexes;
            public string BugTrackMessage;
            public bool? BugTrackNumber;
            public string BugTrackUrl;
            public bool? BugTrackWarnIfNoIssue;
            public string RevisionRegex;
            public string RevisionUrl;
            public string RevisionPathUrl;

            public int? LogMessageMinSize;
            public int? LockMessageMinSize;
            public int? LogWidth;
            public string LogSummary;

            public ReadOnlyCollection<string> LogRegexes;
            public Regex AllInOneRe;
            public Regex LogPrepareRe;
            public Regex LogSplitRe;
            public Regex LogSummaryRe;
            public Regex RevisionRe;

            public bool BrokenRegex;

            public string IssueRepositoryConnectorName;
            public string IssueRepositoryUri;
            public string IssueRepositoryId;
            public string IssueRepositoryPropertyNames;
            public string IssueRepositoryPropertyValues;
        }

        ISelectionContext _selectionContext;
        ISvnStatusCache _statusCache;
        ISelectionContext SelectionContext
        {
            get { return _selectionContext ?? (_selectionContext = GetService<ISelectionContext>()); }
        }

        ISvnStatusCache StatusCache
        {
            get { return _statusCache ?? (_statusCache = GetService<ISvnStatusCache>()); }
        }


        SettingsCache _cache = new SettingsCache();

        bool IsDirty()
        {
            string solutionFile = SelectionContext.SolutionFilename;

            SettingsCache cache = _cache;
            if (cache == null)
                return true;

            if (cache.SolutionFilename != solutionFile)
                return true;

            if (solutionFile == null)
                return false;

            SvnItem item = StatusCache[solutionFile];

            if (item == null || item.ChangeCookie != cache.SolutionCookie)
                return true;

            string path = _cache.ProjectRoot;

            if (string.IsNullOrEmpty(path))
                return false;

            item = StatusCache[_cache.ProjectRoot];

            if (item == null || item.ChangeCookie != cache.RootCookie)
                return true;

            return false;
        }

        private void RefreshIfDirty()
        {
            if (!IsDirty() && _cache != null)
                return;

            _cache = null;
            SettingsCache cache = new SettingsCache();
            try
            {

                string solutionFile = SelectionContext.SolutionFilename;

                if (string.IsNullOrEmpty(solutionFile))
                    return;

                SvnItem item = StatusCache[solutionFile];

                if (item == null)
                    return;

                cache.SolutionFilename = item.FullPath;
                cache.SolutionCookie = item.ChangeCookie;

                if (!item.Exists)
                    return;

                SvnWorkingCopy wc = item.WorkingCopy;
                SvnItem parent;
                if (wc != null)
                    parent = StatusCache[wc.FullPath];
                else
                    parent = item.Parent;

                if (parent != null)
                {
                    cache.ProjectRoot = parent.FullPath;
                    cache.ProjectRootUri = parent.Uri;
                    cache.ProjectRootItem = parent;
                }

                bool loaded = LoadSolutionProperties(cache, item);

                if (cache.ProjectRoot != null)
                {
                    parent = StatusCache[cache.ProjectRoot];

                    if (parent == null)
                        return;

                    cache.ProjectRootItem = parent;
                    cache.RootCookie = parent.ChangeCookie;

                    if (!loaded)
                        LoadRootProperties(cache, parent);
                }
            }
            finally
            {
                _cache = cache;
            }
        }

        private bool LoadSolutionProperties(SettingsCache cache, SvnItem item)
        {
            // Subversion loads all properties in memory at once; loading them 
            // all is always faster than loading a few
            // We must change this algorithm if Subversions implementation changes

            using (SvnClient client = GetService<ISvnClientPool>().GetNoUIClient())
            {
                string value;
                if (client.TryGetProperty(item.FullPath, AnkhSccPropertyNames.ProjectRoot, out value))
                    SetProjectRootViaProperty(cache, value);

                SvnPropertyCollection pc;
                if (client.TryGetAllInheritedProperties(item.FullPath, out pc))
                {
                    foreach (SvnPropertyValue pv in pc)
                    {
                        LoadPropertyBoth(cache, pv);
                    }

                    return true; // Got properties for solution and parent dirs
                }

                return false;
            }
        }

        private void SetProjectRootViaProperty(SettingsCache cache, string value)
        {
            string dir = value;
            string solutionFile = cache.SolutionFilename;

            SvnItem directory = StatusCache[solutionFile].Parent;
            if (directory == null)
                return;

            SvnWorkingCopy wc = directory.WorkingCopy;

            int up = 0;

            while (dir.StartsWith("../"))
            {
                up++;
                dir = dir.Substring(3);
                if (directory != null)
                    directory = directory.Parent;
            }

            if (directory == null)
                return; // Invalid value

            if (directory.WorkingCopy != wc)
                return; // Outside workingcopy

            if (dir.Length == 0)
            {
                cache.ProjectRoot = directory.FullPath;
                cache.ProjectRootUri = directory.Uri;
            }
        }

        private void LoadRootProperties(SettingsCache cache, SvnItem item)
        {
            // Subversion loads all properties in memory at once; loading them 
            // all at once is always faster than loading a few
            using (SvnClient client = GetService<ISvnClientPool>().GetNoUIClient())
            {
                SvnPropertyCollection pc;
                if (client.TryGetAllInheritedProperties(item.FullPath, out pc))
                {
                    foreach (SvnPropertyValue pv in pc)
                    {
                        LoadPropertyBoth(cache, pv);
                    }
                }
            }
        }

        private void LoadPropertyBoth(SettingsCache cache, SvnPropertyValue pv)
        {
            bool boolValue;
            int intValue;
            switch (pv.Key)
            {
                case SvnPropertyNames.BugTrackAppend:
                    if (!cache.BugTrackAppend.HasValue && TryParseBool(pv, out boolValue))
                        cache.BugTrackAppend = boolValue;
                    break;
                case SvnPropertyNames.BugTrackLabel:
                    if (cache.BugTrackLabel == null)
                        cache.BugTrackLabel = pv.StringValue;
                    break;
                case SvnPropertyNames.BugTrackLogRegex:
                    if (cache.BugTrackLogRegexes == null)
                        cache.BugTrackLogRegexes = pv.StringValue;
                    break;
                case SvnPropertyNames.BugTrackMessage:
                    if (cache.BugTrackMessage == null)
                        cache.BugTrackMessage = pv.StringValue.Replace("\r", "");
                    break;
                case SvnPropertyNames.BugTrackNumber:
                    if (!cache.BugTrackNumber.HasValue && TryParseBool(pv, out boolValue))
                        cache.BugTrackNumber = boolValue;
                    break;
                case SvnPropertyNames.BugTrackUrl:
                    if (cache.BugTrackUrl == null)
                        cache.BugTrackUrl = pv.StringValue;
                    break;
                case SvnPropertyNames.BugTrackWarnIfNoIssue:
                    if (!cache.BugTrackWarnIfNoIssue.HasValue && TryParseBool(pv, out boolValue))
                        cache.BugTrackWarnIfNoIssue = boolValue;
                    break;
                case SvnPropertyNames.TortoiseSvnLogMinSize:
                    if (!cache.LogMessageMinSize.HasValue && !string.IsNullOrEmpty(pv.StringValue)
                        && int.TryParse(pv.StringValue, out intValue))
                    {
                        cache.LogMessageMinSize = intValue;
                    }
                    break;
                case SvnPropertyNames.TortoiseSvnLockMsgMinSize:
                    if (!cache.LockMessageMinSize.HasValue && !string.IsNullOrEmpty(pv.StringValue)
                        && int.TryParse(pv.StringValue, out intValue))
                    {
                        cache.LockMessageMinSize = intValue;
                    }
                    break;
                case SvnPropertyNames.TortoiseSvnLogWidthLine:
                    if (!cache.LogWidth.HasValue && !string.IsNullOrEmpty(pv.StringValue)
                        && int.TryParse(pv.StringValue, out intValue))
                    {
                        cache.LogWidth = intValue;
                    }
                    break;
                case SvnPropertyNames.TortoiseSvnLogSummary:
                    if (cache.LogSummary == null)
                        cache.LogSummary = pv.StringValue;
                    break;
                case SvnPropertyNames.TortoiseSvnLogRevRegex:
                    if (cache.RevisionRegex == null)
                        cache.RevisionRegex = pv.StringValue;
                    break;
                case SvnPropertyNames.WebViewerRevision:
                    if (cache.RevisionUrl == null)
                        cache.RevisionUrl = pv.StringValue;
                    break;
                case SvnPropertyNames.WebViewerPathRevision:
                    if (cache.RevisionPathUrl == null)
                        cache.RevisionPathUrl = pv.StringValue;
                    break;
                case AnkhSccPropertyNames.IssueRepositoryConnector:
                    cache.IssueRepositoryConnectorName = pv.StringValue;
                    break;
                case AnkhSccPropertyNames.IssueRepositoryUri:
                    cache.IssueRepositoryUri = pv.StringValue;
                    break;
                case AnkhSccPropertyNames.IssueRepositoryId:
                    cache.IssueRepositoryId = pv.StringValue;
                    break;
                case AnkhSccPropertyNames.IssueRepositoryPropertyNames:
                    cache.IssueRepositoryPropertyNames = pv.StringValue;
                    break;
                case AnkhSccPropertyNames.IssueRepositoryPropertyValues:
                    cache.IssueRepositoryPropertyValues = pv.StringValue;
                    break;
            }
        }

        private bool TryParseBool(SvnPropertyValue pv, out bool boolValue)
        {
            string val = pv.StringValue;

            boolValue = false;
            if (string.IsNullOrEmpty(val))
                return false;

            if (string.Equals(val, "true", StringComparison.OrdinalIgnoreCase)
                || string.Equals(val, "yes", StringComparison.OrdinalIgnoreCase)
                || string.Equals(val, SvnPropertyNames.SvnBooleanValue))
            {
                boolValue = true;
                return true;
            }

            if (string.Equals(val, "false", StringComparison.OrdinalIgnoreCase)
                || string.Equals(val, "no", StringComparison.OrdinalIgnoreCase))
            {
                boolValue = false;
                return true;
            }

            return false;
        }

        public string SolutionFilename
        {
            get
            {
                RefreshIfDirty();

                return _cache.SolutionFilename;
            }
        }

        public string ProjectRoot
        {
            get
            {
                RefreshIfDirty();

                SettingsCache cache = _cache;
                if (cache != null)
                    return cache.ProjectRoot;
                else
                    return null;
            }
            set
            {
                if (string.Equals(value, ProjectRoot, StringComparison.OrdinalIgnoreCase))
                    return;

                SettingsCache cache = _cache;
                if (cache == null || string.IsNullOrEmpty(SolutionFilename))
                    throw new InvalidOperationException();

                SetProjectRootValue(value);
            }
        }

        void SetProjectRootValue(string value)
        {
            if (SolutionFilename == null)
                return;

            string sd = SvnTools.PathToRelativeUri(SvnTools.GetNormalizedDirectoryName(SolutionFilename).TrimEnd('\\') + '\\').ToString();
            string v = SvnTools.PathToRelativeUri(SvnTools.GetNormalizedFullPath(value)).ToString();

            if (!v.EndsWith("/"))
                v += "/";

            if (!sd.StartsWith(v, StringComparison.OrdinalIgnoreCase))
                return;

            Uri solUri;
            Uri resUri;

            if (!Uri.TryCreate("file:///" + sd.Replace('\\', '/'), UriKind.Absolute, out solUri)
                || !Uri.TryCreate("file:///" + v.Replace('\\', '/'), UriKind.Absolute, out resUri))
                return;

            using (SvnClient client = GetService<ISvnClientPool>().GetNoUIClient())
            {
                SvnSetPropertyArgs ps = new SvnSetPropertyArgs();
                ps.ThrowOnError = false;

                client.SetProperty(SolutionFilename, AnkhSccPropertyNames.ProjectRoot, solUri.MakeRelativeUri(resUri).ToString(), ps);

                GetService<ISvnStatusCache>().MarkDirty(SolutionFilename);
                // The getter will reload the settings for us
            }

            _cache = null;
        }

        public Uri ProjectRootUri
        {
            get
            {
                RefreshIfDirty();

                return _cache.ProjectRootUri;
            }
        }

        public SvnItem ProjectRootSvnItem
        {
            get
            {
                RefreshIfDirty();

                return _cache.ProjectRootItem;
            }
        }

        string _allProjectTypesFilter;
        public string AllProjectExtensionsFilter
        {
            get
            {
                if (_allProjectTypesFilter != null)
                    return _allProjectTypesFilter;

                IVsSolution solution = GetService<IVsSolution>(typeof(SVsSolution));

                if (solution == null)
                    return null;

                object value;
                if (VSErr.Succeeded(solution.GetProperty((int)__VSPROPID.VSPROPID_RegisteredProjExtns, out value)))
                    _allProjectTypesFilter = value as string;

                return _allProjectTypesFilter;
            }
        }

        string _projectFilterName;
        public string OpenProjectFilterName
        {
            get
            {
                if (_projectFilterName != null)
                    return _projectFilterName;

                IVsSolution solution = GetService<IVsSolution>(typeof(SVsSolution));

                if (solution == null)
                    return null;

                object value;
                if (VSErr.Succeeded(solution.GetProperty((int)__VSPROPID.VSPROPID_OpenProjectFilter, out value)))
                    _projectFilterName = value as string;

                return _projectFilterName;
            }
        }

        public string NewProjectLocation
        {
            get
            {
                IVsShell shell = GetService<IVsShell>(typeof(SVsShell));

                if (shell != null)
                {
                    object r;
                    if (VSErr.Succeeded(shell.GetProperty((int)__VSSPROPID.VSSPROPID_VisualStudioProjDir, out r)))
                    {
                        string path = (string)r;

                        return SvnTools.GetTruePath(path, true) ?? SvnTools.GetNormalizedFullPath(path);
                    }
                }

                return "C:\\";
            }
        }

        #region IAnkhSolutionSettings Members

        public string OpenFileFilter
        {
            get
            {
                IVsShell shell = GetService<IVsShell>(typeof(SVsShell));

                if (shell != null)
                {
                    object r;
                    if (VSErr.Succeeded(shell.GetProperty((int)__VSSPROPID.VSSPROPID_OpenFileFilter, out r)))
                        return ((string)r).Replace('\0', '|').TrimEnd('|');
                }

                return "All Files (*.*)|*.*";
            }
        }

        /// <summary>
        /// Gets a value indicating whether [in ranu mode].
        /// </summary>
        /// <value><c>true</c> if [in ranu mode]; otherwise, <c>false</c>.</value>
        public bool InRanuMode
        {
            get { return _inRanu; }
        }

        /// <summary>
        /// Gets the visual studio registry root.
        /// </summary>
        /// <value>The visual studio registry root.</value>
        public string VisualStudioRegistryRoot
        {
            get { return _vsAppRoot; }
        }

        /// <summary>
        /// Gets the visual studio user registry root.
        /// </summary>
        /// <value>The visual studio user registry root.</value>
        public string VisualStudioUserRegistryRoot
        {
            get { return _vsUserRoot; }
        }

        /// <summary>
        /// Gets the registry hive suffix.
        /// </summary>
        /// <value>The registry hive suffix.</value>
        public string RegistryHiveSuffix
        {
            get { return _hiveSuffix; }
        }

        public Uri RepositoryRoot
        {
            get
            {
                RefreshIfDirty();

                return _cache.RepositoryRoot ?? (_cache.RepositoryRoot = GetRepositoryRoot());
            }
        }

        private Uri GetRepositoryRoot()
        {
            SettingsCache cache = _cache;

            if (cache.SolutionFilename == null)
                return null;

            using (SvnClient client = GetService<ISvnClientPool>().GetNoUIClient())
            {
                return cache.RepositoryRoot = client.GetRepositoryRoot(cache.SolutionFilename);
            }
        }

        #endregion

        IAnkhConfigurationService _config;
        IAnkhConfigurationService Config
        {
            get { return _config ?? (_config = GetService<IAnkhConfigurationService>()); }
        }

        public IEnumerable<Uri> GetRepositoryUris(bool forBrowse)
        {
            HybridCollection<Uri> uris = new HybridCollection<Uri>();

            if (ProjectRootUri != null)
                uris.Add(ProjectRootUri);

            // Global keys (over all versions)
            using (RegistryKey rk = Config.OpenGlobalKey("Repositories"))
            {
                if (rk != null)
                    LoadUris(uris, rk);
            }

            // Per hive
            using (RegistryKey rk = Config.OpenInstanceKey("Repositories"))
            {
                if (rk != null)
                    LoadUris(uris, rk);
            }

            // Per user + Per hive
            using (RegistryKey rk = Config.OpenUserInstanceKey("Repositories"))
            {
                if (rk != null)
                    LoadUris(uris, rk);
            }

            // Finally add the last used list from TortoiseSVN
            try
            {
                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(
                     "SOFTWARE\\TortoiseSVN\\History\\repoURLS", RegistryKeyPermissionCheck.ReadSubTree))
                {
                    if (rk != null)
                        LoadUris(uris, rk);
                }

            }
            catch (SecurityException)
            { /* Ignore no read only access; stupid sysadmins */ }

            IAnkhConfigurationService configSvc = GetService<IAnkhConfigurationService>();
            if (configSvc != null)
            {
                foreach (string u in configSvc.GetRecentReposUrls())
                {
                    Uri uri;
                    if (u != null && Uri.TryCreate(u, UriKind.Absolute, out uri))
                    {
                        if (!uris.Contains(uri))
                            uris.Add(uri);
                    }
                }
            }

            return uris;
        }

        static void LoadUris(HybridCollection<Uri> uris, RegistryKey rk)
        {
            foreach (string name in rk.GetValueNames())
            {
                string value = rk.GetValue(name) as string;

                if (value != null && !value.EndsWith("/"))
                    value += "/";

                Uri uri;
                if (value != null && Uri.TryCreate(value, UriKind.Absolute, out uri))
                {
                    if (!uris.Contains(uri))
                        uris.Add(uri);
                }
            }
        }

        string _solutionFilter;
        /// <summary>
        /// Gets the solution filter.
        /// </summary>
        /// <value>The solution filter.</value>
        public string SolutionFilter
        {
            get
            {
                if (_solutionFilter != null)
                    return _solutionFilter;

                _solutionFilter = "*.sln;*.dsw"; // Hardcoded default :(

                IVsSolution solution = GetService<IVsSolution>(typeof(SVsSolution));

                if (solution != null)
                {
                    object filter;

                    // The VS11+ official route
                    if (VSErr.Succeeded(solution.GetProperty(-8037 /* VSPROPID_SolutionFileExt */, out filter)))
                    {
                        _solutionFilter = "*" + (string)filter;
                    }
                }
               
                return _solutionFilter;
            }
        }

        #region IAnkhSolutionSettings Members

        public void OpenProjectFile(string projectFile)
        {
            OpenOrAddProjectFile(projectFile, false);
        }

        public void AddProjectFile(string projectFile)
        {
            OpenOrAddProjectFile(projectFile, true);
        }

        public void OpenOrAddProjectFile(string projectFile, bool add)
        {
            if (string.IsNullOrEmpty(projectFile))
                throw new ArgumentNullException("projectFile");

            string ext = Path.GetExtension(projectFile);
            bool isSolution = false;
            foreach (string x in SolutionFilter.Split(';'))
            {
                if (string.Equals(ext, Path.GetExtension(x), StringComparison.OrdinalIgnoreCase))
                {
                    isSolution = true;
                    break;
                }
            }

            IVsSolution solution = GetService<IVsSolution>(typeof(SVsSolution));

            int hr;
            if (isSolution && !add)
                hr = solution.OpenSolutionFile(0, projectFile);
            else
            {
                Guid gnull = Guid.Empty;
                Guid gInterface = Guid.Empty;
                IntPtr pProj = IntPtr.Zero;

                hr = solution.CreateProject(ref gnull, projectFile, null, null, (uint)__VSCREATEPROJFLAGS.CPF_OPENFILE, ref gInterface, out pProj);
            }

            if (!VSErr.Succeeded(hr))
                new AnkhMessageBox(this).Show(Marshal.GetExceptionForHR(hr).Message, "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        #endregion
    }
}


