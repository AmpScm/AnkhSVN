using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using Ankh.Scc;
using System.IO;

namespace Ankh
{
    internal class WorkingCopyOperations : IWorkingCopyOperations
    {
        IAnkhServiceProvider _context;

        public WorkingCopyOperations(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        public bool IsWorkingCopyPath(string path)
        {
            if (path == null)
                return false;

            string dir = path;
            if (!Directory.Exists(path))
                dir = Path.GetDirectoryName(path);

            return SvnTools.IsManagedPath(path);
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
            return Path.GetDirectoryName(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }
    }
}
