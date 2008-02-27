using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using Utils.Services;
using System.IO;

namespace Ankh
{
    internal class WorkingCopyOperations : IWorkingCopyOperations
    {
        public WorkingCopyOperations(SvnClient client)
        {
            this.client = client;
        }

        public bool IsWorkingCopyPath(string path)
        {
            if (path == null)
                return false;

            string dir = path;
            if (!Directory.Exists(path))
                dir = Path.GetDirectoryName(path);
            return Directory.Exists(Path.Combine(dir, SvnClient.AdministrativeDirectoryName));
        }

        public string GetWorkingCopyRootedPath(string path)
        {
            if (IsWorkingCopyPath(path) && !IsWorkingCopyPath(GetParentDir(path)))
                return new string(Path.DirectorySeparatorChar, 1);

            string retPath = path;
            int separator = retPath.IndexOf(Path.DirectorySeparatorChar);
            while (true)
            {
                if (separator == -1)
                    throw new SvnException("Path not part of a working copy");

                string dir = path.Substring(0, separator + 1);

                // is our current path a working copy?
                if (IsWorkingCopyPath(dir))
                    break;

                // find the next subcomponent
                separator = path.IndexOf(Path.DirectorySeparatorChar, separator + 1);
            }

            return path.Substring(separator, path.Length - separator);
        }

        static string GetParentDir(string path)
        {
            if (path.Length <= 1)
                return null;
            else return path[path.Length - 1] == Path.DirectorySeparatorChar ?
                Path.GetDirectoryName(path.Substring(0, path.Length - 1)) :
                Path.GetDirectoryName(path);
        }

        private SvnClient client;
    }
}
