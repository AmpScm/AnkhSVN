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

namespace Ankh.Settings
{
    class SolutionSettings : AnkhService, IAnkhSolutionSettings
    {
        int _solutionCookie;
        public SolutionSettings(IAnkhServiceProvider context)
            : base(context)
        {
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


        string GetSvnWcRoot(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentNullException("directory");

            IFileStatusCache cache = GetService<IFileStatusCache>();

            if(cache == null)
                return directory;

            SvnItem item = cache[directory];

            while (item != null)
            {
                SvnItem parent = item.Parent;
                if (parent == null || !parent.IsVersioned)
                    break;

                if (parent.Status.RepositoryId != item.Status.RepositoryId)
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

                    if (client.TryGetProperty(new SvnPathTarget(_cache.SolutionFilename), "vs:project-root", out value)
                            && !string.IsNullOrEmpty(value))
                    {
                        Uri solution, relative;

                        if (!Uri.TryCreate("file:///" + _cache.SolutionFilename.Replace(Path.DirectorySeparatorChar, '/'), UriKind.Absolute, out solution)
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

                        string vv = combined.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);

                        return _cache.ProjectRoot = SvnTools.GetNormalizedFullPath(vv.Replace('/', Path.DirectorySeparatorChar));
                    }

                    return _cache.ProjectRoot = SvnTools.GetNormalizedFullPath(wcRoot);
                }
            }
            set
            {
                ClearIfDirty();
                if (SolutionFilename == null)
                    return;

                string sd = Path.GetDirectoryName(SolutionFilename).TrimEnd('\\') + '\\';
                string v = SvnTools.GetNormalizedFullPath(value);

                if (!v.EndsWith("\\"))
                    v += "\\";

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

                if(!string.IsNullOrEmpty(pr) && pr[pr.Length-1] != Path.DirectorySeparatorChar)
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

                if(solution == null)
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
                ILocalRegistry2 r = Context.GetService<ILocalRegistry2>(typeof(SLocalRegistry));

                string root;

                if (!ErrorHandler.Succeeded(r.GetLocalRegistryRoot(out root)))
                    return null;

                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(root, RegistryKeyPermissionCheck.ReadSubTree))
                {
                    string value = rk.GetValue("VisualStudioProjectsLocation", "C:\\") as string;

                    if (!string.IsNullOrEmpty(value))
                        return SvnTools.GetNormalizedFullPath(value);
                    else
                        return "C:\\";
                }
            }
        }

    }
}
