using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ankh.Scc.SettingMap
{
    [DebuggerDisplay("{SolutionPath} => ({EnlistmentPath}, {EnlistmentUNCPath})")]
    sealed class SccTranslatePathInfo
    {
        readonly string _solutionPath;
        readonly string _enlistUIPath;
        readonly string _enlistUNCPath;

        public SccTranslatePathInfo(string solutionPath, string enlistUIPath, string enlistUNCPath)
        {
            if (string.IsNullOrEmpty(solutionPath))
                throw new ArgumentNullException("solutionPath");
            else if (string.IsNullOrEmpty(enlistUIPath))
                throw new ArgumentNullException("enlistUIPath");
            else if (string.IsNullOrEmpty(enlistUNCPath))
                throw new ArgumentNullException("enlistUNCPath");

            _solutionPath = solutionPath;
            _enlistUIPath = enlistUIPath;
            _enlistUNCPath = enlistUNCPath;
        }

        public string EnlistmentPath
        {
            get { return _enlistUIPath; }
        }

        public string EnlistmentUNCPath
        {
            get { return _enlistUNCPath; }
        }

        public string SolutionPath
        {
            get { return _solutionPath; }
        }
    }
}
