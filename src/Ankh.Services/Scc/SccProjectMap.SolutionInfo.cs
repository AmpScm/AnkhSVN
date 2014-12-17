using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;

namespace Ankh.Scc
{
    partial class SccProjectMap
    {
        string _solutionFile;
        string _solutionDirectory;
        string _rawSolutionDirectory;

        internal protected void ClearSolutionInfo()
        {
            _solutionFile = null;
            _solutionDirectory = null;
            _rawSolutionDirectory = null;
        }

        public string SolutionFilename
        {
            get
            {
                if (_solutionFile == null)
                    LoadSolutionInfo();

                return _solutionFile.Length > 0 ? _solutionFile : null;
            }
        }

        public string SolutionDirectory
        {
            get
            {
                if (_solutionFile == null)
                    LoadSolutionInfo();

                return _solutionDirectory;
            }
        }

        public string RawSolutionDirectory
        {
            get
            {
                if (_solutionFile == null)
                    LoadSolutionInfo();

                return _rawSolutionDirectory;
            }
        }

        void LoadSolutionInfo()
        {
            string dir, path, user;

            _rawSolutionDirectory = null;
            _solutionDirectory = null;
            _solutionFile = "";

            IVsSolution sol = GetService<IVsSolution>(typeof(SVsSolution));

            if (sol == null ||
                !VSErr.Succeeded(sol.GetSolutionInfo(out dir, out path, out user)))
            {
                return;
            }

            if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(path))
            {
                // Cache negative result; will be returned as null
            }
            else
            {
                if (IsSafeSccPath(dir))
                {
                    _rawSolutionDirectory = dir;
                    _solutionDirectory = SvnTools.GetTruePath(dir, true) ?? SvnTools.GetNormalizedFullPath(dir);
                }

                if (IsSafeSccPath(path))
                    _solutionFile = SvnTools.GetTruePath(path, true) ?? SvnTools.GetNormalizedFullPath(path);
            }
        }

        string _tempPath;
        public string TempPathWithSeparator
        {
            get
            {
                if (_tempPath == null)
                {
                    string p = System.IO.Path.GetTempPath();

                    if (p.Length > 0 && p[p.Length - 1] != Path.DirectorySeparatorChar)
                        p += Path.DirectorySeparatorChar;

                    _tempPath = p;
                }
                return _tempPath;
            }
        }

        public bool IsSafeSccPath(string file)
        {
            if (string.IsNullOrEmpty(file))
                return false;
            else if (!SvnItem.IsValidPath(file))
                return false;
            else if (file.StartsWith(TempPathWithSeparator, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
