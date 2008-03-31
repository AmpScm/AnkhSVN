using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VS;
using Ankh.Selection;
using Ankh.Scc;
using SharpSvn;
using System.IO;

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
        #region IAnkhSolutionSettings Members

        public string SolutionFilename
        {
            get
            {
                ClearIfDirty();

                return _cache.SolutionFilename ?? (_cache.SolutionFilename = GetService<ISelectionContext>().SolutionFilename);
            }
        }

        public string ProjectRoot
        {
            get
            {
                ClearIfDirty();

                if (_cache.ProjectRoot != null)
                    return _cache.ProjectRoot;

                string sd = Path.GetDirectoryName(SolutionFilename); // Contains ClearIfDirty();

                using (SvnClient client = GetService<ISvnClientPool>().GetNoUIClient())
                {
                    string value;
                    if (client.TryGetProperty(new SvnPathTarget(_cache.SolutionFilename), "vs:project-root", out value))
                    {
                        Uri solution, relative;

                        if (!Uri.TryCreate("file:///" + _cache.SolutionFilename.Replace(Path.DirectorySeparatorChar, '/'), UriKind.Absolute, out solution))
                            return _cache.ProjectRoot = sd;

                        if (value != "./")
                        {
                            string r = value;
                            while (r.StartsWith("../"))
                                r = r.Substring(3);

                            if (!string.IsNullOrEmpty(r))
                                return _cache.ProjectRoot = sd;
                        }

                        if (!Uri.TryCreate(value, UriKind.Relative, out relative))
                            return null;

                        Uri combined = new Uri(solution, relative);

                        return _cache.ProjectRoot = combined.AbsolutePath.Replace('/', '\\').TrimEnd('\\') + '\\';
                    }

                    return _cache.ProjectRoot = sd;
                }
            }
            set
            {
                ClearIfDirty();
                if (SolutionFilename == null)
                    return;

                string sd = Path.GetDirectoryName(SolutionFilename).TrimEnd('\\') + '\\';
                string v = Path.GetFullPath(value).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

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
                }
            }
        }

        #endregion
    }
}
