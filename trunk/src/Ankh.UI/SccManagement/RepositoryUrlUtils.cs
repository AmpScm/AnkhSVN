using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.UI.SccManagement
{
    // TODO: BH - Refactor to a more logical location
    public class RepositoryLayoutInfo
    {
        public Uri WorkingRoot;
        public Uri WholeProjectRoot;
        public Uri BranchesRoot;
    }

    public static class RepositoryUrlUtils
    {
        public static bool TryGuessLayout(IAnkhServiceProvider context, Uri uri, out RepositoryLayoutInfo info)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (uri == null)
                throw new ArgumentNullException("uri");

            info = null;

            uri = SvnTools.GetNormalizedUri(uri);

            GC.KeepAlive(context); // Allow future external hints

            string path = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);

            if (string.IsNullOrEmpty(path))
                return false;

            if (path[0] != '/')
                path = '/' + path;
            if (path[path.Length - 1] != '/')
                path += '/';
            

            if (string.IsNullOrEmpty(path) || path.Length == 1)
                return false;

            string r = path;

            while (r.Length > 0 && !r.EndsWith("/trunk/", StringComparison.OrdinalIgnoreCase))
            {
                int n = r.LastIndexOf('/', r.Length - 1);

                if (n >= 0)
                {
                    int lastCharIndex = r.Length - 1;
                    // if '/' is the last character, strip and continue
                    // otherwise include '/' to give "/trunk/" check a chance
                    r = r.Substring(0, (n == lastCharIndex) ? n : (n+1));
                }
                else
                    r = "";
            }

            if (!string.IsNullOrEmpty(r))
            {
                info = new RepositoryLayoutInfo();
                info.WorkingRoot = new Uri(uri, r);
                info.WholeProjectRoot = new Uri(uri, r.Substring(0, r.Length - 6));
                info.BranchesRoot = new Uri(info.WholeProjectRoot, "branches/");
                return true;
            }

            r = path;

            while (r.Length > 0 && !r.EndsWith("/branches/", StringComparison.OrdinalIgnoreCase))
            {
                int n = r.LastIndexOf('/', r.Length - 1);

                if (n >= 0)
                {
                    int lastCharIndex = r.Length - 1;
                    // if '/' is the last character, strip and continue
                    // otherwise include '/' to give "/branches/" check a chance
                    r = r.Substring(0, (n == lastCharIndex) ? n : (n + 1));
                }
                else
                    r = "";
            }

            if (!string.IsNullOrEmpty(r))
            {
                info = new RepositoryLayoutInfo();

                string dir = (path.Length > r.Length) ? path.Substring(0, path.IndexOf('/', r.Length)) : path;

                info.WorkingRoot = new Uri(uri, dir);
                info.WholeProjectRoot = new Uri(uri, r.Substring(0, r.Length - 9));
                info.BranchesRoot = new Uri(info.WholeProjectRoot, r.Substring(r.Length - 9, 9)); // 'branches/' but with repos casing
                return true;
            }

            r = path;

            while (r.Length > 0 && !r.EndsWith("/tags/", StringComparison.OrdinalIgnoreCase))
            {
                int n = r.LastIndexOf('/', r.Length - 1);

                if (n >= 0)
                {
                    int lastCharIndex = r.Length - 1;
                    // if '/' is the last character, strip and continue
                    // otherwise include '/' to give "/tags/" check a chance
                    r = r.Substring(0, (n == lastCharIndex) ? n : (n + 1));
                }
                else
                    r = "";
            }

            if (!string.IsNullOrEmpty(r))
            {
                info = new RepositoryLayoutInfo();

                string dir = (path.Length > r.Length) ? path.Substring(0, path.IndexOf('/', r.Length)) : path;

                info.WorkingRoot = new Uri(uri, dir);
                info.WholeProjectRoot = new Uri(uri, r);
                info.BranchesRoot = new Uri(info.WholeProjectRoot, r.Substring(r.Length - 5, 5)); // 'tags/' but with repos casing
                return true;
            }

            info = new RepositoryLayoutInfo();
            info.WorkingRoot = uri;
            info.WholeProjectRoot = new Uri(uri, "../");
            info.BranchesRoot = new Uri(info.WorkingRoot, "branches/");
            return true;
        }
    }
}
