// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
        public Uri TagsRoot;
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

            if (TryFindBranch(uri, path, "branches", true, out info))
                return true;
            else if (TryFindBranch(uri, path, "tags", false, out info))
                return true;
            else if (TryFindBranch(uri, path, "releases", false, out info))
                return true;

            info = new RepositoryLayoutInfo();
            info.WorkingRoot = uri;
            info.WholeProjectRoot = new Uri(uri, "../");
            info.BranchesRoot = new Uri(info.WorkingRoot, "branches/");
            info.TagsRoot = new Uri(info.WorkingRoot, "tags/");
            return true;
        }

        private static bool TryFindBranch(Uri uri, string path, string name, bool branch, out RepositoryLayoutInfo info)
        {
            info = null;
            string r = path;

            name = '/' + name + '/';

            while (r.Length > 0 && !r.EndsWith(name, StringComparison.OrdinalIgnoreCase))
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
                info.BranchesRoot = new Uri(info.WorkingRoot, "branches/"); // Always set some branches suggestion?
                
                Uri itemRoot = new Uri(info.WholeProjectRoot, r.Substring(r.Length - name.Length + 1, name.Length-1)); // 'tags/' but with repos casing

                if (branch)
                    info.BranchesRoot = itemRoot;
                else
                    info.TagsRoot = itemRoot;
                return true;
            }

            return false;
        }
    }
}
