using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VS;
using Ankh.Selection;
using Ankh.Scc;
using SharpSvn;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.Win32;
using Ankh.UI;
using System.Security;
using FileVersionInfo=System.Diagnostics.FileVersionInfo;

namespace Ankh.Settings
{
    [GlobalService(typeof(IAnkhSolutionSettings))]
    class SolutionSettings : AnkhService, IAnkhSolutionSettings
    {
        int _solutionCookie;
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
            if (ErrorHandler.Succeeded(shell.GetProperty((int)__VSSPROPID.VSSPROPID_VirtualRegistryRoot, out r)))
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
        }

        SettingsCache _cache = new SettingsCache();

        private void ClearIfDirty()
        {
            ISelectionContext selection = GetService<ISelectionContext>();
            IFileStatusCache cache = GetService<IFileStatusCache>();
            bool dirty = false;
            SvnItem solutionItem = null;

            if (string.IsNullOrEmpty(selection.SolutionFilename))
                dirty = true;
            else if (null == (solutionItem = cache[selection.SolutionFilename]))
                dirty = true;
            else if (solutionItem.ChangeCookie != _solutionCookie)
                dirty = true;

            if (dirty)
            {
                _cache = new SettingsCache();

                if (solutionItem != null)
                    _solutionCookie = solutionItem.ChangeCookie;
            }
        }

        public string SolutionFilename
        {
            get
            {
                ClearIfDirty();

                return _cache.SolutionFilename ?? (_cache.SolutionFilename = GetService<ISelectionContext>().SolutionFilename);
            }
        }

        bool SameRepository(SvnItem item1, SvnItem item2)
        {
            bool oneNull = (item1 == null || item1.Status == null);
                bool twoNull = (item2 == null || item2.Status == null);

            if(oneNull || twoNull)
                return oneNull && twoNull;

            AnkhStatus s1 = item1.Status;
            AnkhStatus s2 = item2.Status;

            if(s1.RepositoryId != null && s2.RepositoryId != null)
                return s1.RepositoryId == s2.RepositoryId;

            if (!item1.IsVersioned || !item2.IsVersioned)
                return false;

            using(SvnClient svn = GetService<ISvnClientPool>().GetNoUIClient())
            {
                Uri r1 = svn.GetRepositoryRoot(item1.FullPath);
                Uri r2 = svn.GetRepositoryRoot(item2.FullPath);

                return r1 == r2;
            }
        }


        string GetSvnWcRoot(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentNullException("directory");

            IFileStatusCache cache = GetService<IFileStatusCache>();

            if (cache == null)
                return directory;

            SvnItem item = cache[directory];

            while (item != null && item.Status != null && item.Status.Uri != null)
            {
                SvnItem parent = item.Parent;
                if (parent == null || !parent.IsVersioned || parent.Status == null || parent.Status.Uri == null)
                    break;

                if (!SameRepository(parent, item))
                    break;

                Uri uri = new Uri(item.Status.Uri, "../");

                if (parent.Status.Uri != uri)
                    break;

                item = parent;
            }

            return item.FullPath;
        }



        public string ProjectRoot
        {
            get
            {
                ClearIfDirty();

                if (_cache.ProjectRoot != null)
                    return _cache.ProjectRoot;

                string solutionFilename = SolutionFilename;

                if (string.IsNullOrEmpty(solutionFilename))
                    return null;

                string wcRoot = GetSvnWcRoot(Path.GetDirectoryName(SolutionFilename)).TrimEnd('\\') + '\\';

                using (SvnClient client = GetService<ISvnClientPool>().GetNoUIClient())
                {
                    string value;

                    if (client.TryGetProperty(new SvnPathTarget(_cache.SolutionFilename), "vs:project-root", out value))                            
                    {
                        if (string.IsNullOrEmpty(value) && value != null)
                            value = "./";

                        if (!string.IsNullOrEmpty(value))
                        {
                            Uri solution, relative;

                            if (!Uri.TryCreate("file:///" + SvnTools.PathToRelativeUri(_cache.SolutionFilename.Replace(Path.DirectorySeparatorChar, '/')).ToString(), UriKind.Absolute, out solution)
                                || !Uri.TryCreate(value, UriKind.Relative, out relative))
                            {
                                return _cache.ProjectRoot = SvnTools.GetNormalizedFullPath(wcRoot);
                            }

                            if (value != "./")
                            {
                                string r = value;
                                while (r.StartsWith("../"))
                                    r = r.Substring(3);

                                if (!string.IsNullOrEmpty(r))
                                    return _cache.ProjectRoot = SvnTools.GetNormalizedFullPath(wcRoot);
                            }

                            if (!Uri.TryCreate(value, UriKind.Relative, out relative))
                                return null;

                            Uri combined = new Uri(solution, relative);

                            string vv = SvnTools.UriPartToPath(combined.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped));

                            return _cache.ProjectRoot = SvnTools.GetNormalizedFullPath(vv);
                        }
                    }

                    return _cache.ProjectRoot = SvnTools.GetNormalizedFullPath(wcRoot);
                }
            }
            set
            {
                ClearIfDirty();
                if (SolutionFilename == null)
                    return;

                string sd = SvnTools.PathToRelativeUri(Path.GetDirectoryName(SolutionFilename).TrimEnd('\\') + '\\').ToString();
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

                    client.SetProperty(SolutionFilename, "vs:project-root", solUri.MakeRelativeUri(resUri).ToString(), ps);

                    GetService<IFileStatusCache>().MarkDirty(SolutionFilename);
                    // The getter will reload the settings for us
                }
            }
        }

        public string ProjectRootWithSeparator
        {
            get
            {
                string pr = ProjectRoot;

                if (!string.IsNullOrEmpty(pr) && pr[pr.Length - 1] != Path.DirectorySeparatorChar)
                    return pr + Path.DirectorySeparatorChar;
                else
                    return pr;
            }
        }

        public Uri ProjectRootUri
        {
            get
            {
                ClearIfDirty();

                if (_cache.ProjectRootUri != null)
                    return _cache.ProjectRootUri;

                string projectroot = ProjectRoot;

                if (string.IsNullOrEmpty(projectroot))
                    return null;

                IFileStatusCache cache = GetService<IFileStatusCache>();

                SvnItem rootItem = cache[projectroot];

                return _cache.ProjectRootUri = rootItem.Status.Uri;
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
                if (ErrorHandler.Succeeded(solution.GetProperty((int)__VSPROPID.VSPROPID_RegisteredProjExtns, out value)))
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
                if (ErrorHandler.Succeeded(solution.GetProperty((int)__VSPROPID.VSPROPID_OpenProjectFilter, out value)))
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
                    if (ErrorHandler.Succeeded(shell.GetProperty((int)__VSSPROPID.VSSPROPID_VisualStudioProjDir, out r)))
                        return SvnTools.GetNormalizedFullPath((string)r);
                }

                return "C:\\";
            }
        }

        Version _vsVersion;
        public Version VisualStudioVersion
        {
            get
            {
                if (_vsVersion == null)
                {
                    IVsShell shell = GetService<IVsShell>(typeof(SVsShell));

                    if(shell != null)
                    {
                        object r;
                        if(ErrorHandler.Succeeded(shell.GetProperty((int)__VSSPROPID.VSSPROPID_InstallDirectory, out r)))
                        {
                            string path = r as string;

                            if(!string.IsNullOrEmpty(path) && SvnItem.IsValidPath(path))
                                path = Path.Combine(path, "msenv.dll");
                            else
                                path = null;

                            if(path != null && File.Exists(path))
                            {
                                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(path);

                                string s = fvi.ProductVersion;

                                if (s != null)
                                {
                                    int i = 0;

                                    while (i < s.Length && (char.IsDigit(s, i) || s[i] == '.'))
                                        i++;

                                    if (i < s.Length)
                                        s = s.Substring(0, i);
                                }

                                if(!string.IsNullOrEmpty(s))
                                    _vsVersion = new Version(s);
                            }
                        }
                    }
                }

                return _vsVersion;
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
                    if (ErrorHandler.Succeeded(shell.GetProperty((int)__VSSPROPID.VSSPROPID_OpenFileFilter, out r)))
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

            return uris;
        }

        static void LoadUris(HybridCollection<Uri> uris, RegistryKey rk)
        {
            foreach (string name in rk.GetValueNames())
            {
                string value = rk.GetValue(name) as string;

                Uri uri;
                if (value != null && Uri.TryCreate(value, UriKind.Absolute, out uri))
                {
                    if (!uris.Contains(uri))
                        uris.Add(uri);
                }
            }
        }
    }
}


