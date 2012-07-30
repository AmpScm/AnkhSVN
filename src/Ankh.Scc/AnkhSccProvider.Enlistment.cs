// $Id$
//
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
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;

using Ankh.Scc.ProjectMap;
using Ankh.Scc.SettingMap;

namespace Ankh.Scc
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComImport]
    [Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
    interface IMyPropertyBag
    {
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int Read(string pszPropName, out object pVar, IErrorLog pErrorLog, uint VARTYPE, object pUnkObj);

        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int Write(string pszPropName, ref object pVar);
    }

    partial class AnkhSccProvider
    {
        bool _solutionLoaded;

        readonly Dictionary<string, string> _trueNameMap = new Dictionary<string, string>();
        int IVsSccEnlistmentPathTranslation.TranslateEnlistmentPathToProjectPath(string lpszEnlistmentPath, out string pbstrProjectPath)
        {
            if (_trueNameMap.TryGetValue(lpszEnlistmentPath, out pbstrProjectPath))
            {
                // Already set the path
                return VSConstants.S_OK;
            }
            else if (_trueNameMap.TryGetValue(lpszEnlistmentPath + "\\", out pbstrProjectPath))
            {
                pbstrProjectPath += "\\";
                return VSConstants.S_OK;
            }
            else if (_trueNameMap.TryGetValue(lpszEnlistmentPath.TrimEnd('\\'), out pbstrProjectPath))
            {
                pbstrProjectPath = pbstrProjectPath.TrimEnd('\\');
                return VSConstants.S_OK;
            }

            if (!IsSafeSccPath(lpszEnlistmentPath))
            {
                pbstrProjectPath = lpszEnlistmentPath;
                return VSConstants.S_OK;
            }

            pbstrProjectPath = lpszEnlistmentPath;

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Translates a possibly virtual project path to a local path and an enlistment physical path.
        /// </summary>
        /// <param name="lpszProjectPath">[in] The project's (possibly) virtual path as obtained from the solution file.</param>
        /// <param name="pbstrEnlistmentPath">[out] The local path used by the solution for loading and saving the project.</param>
        /// <param name="pbstrEnlistmentPathUNC">[out] The path used by the source control system for managing the enlistment ("\\drive\path", "[drive]:\path", "file://server/path").</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"/>. If it fails, it returns an error code.
        /// </returns>
        int IVsSccEnlistmentPathTranslation.TranslateProjectPathToEnlistmentPath(string lpszProjectPath, out string pbstrEnlistmentPath, out string pbstrEnlistmentPathUNC)
        {
            if (!IsSafeSccPath(lpszProjectPath))
            {
                pbstrEnlistmentPath = pbstrEnlistmentPathUNC = lpszProjectPath;
                return VSConstants.S_OK;
            }

            string trueProjectPath = CalculateTruePath(lpszProjectPath);

            if (trueProjectPath != lpszProjectPath)
            {
                _trueNameMap[trueProjectPath] = lpszProjectPath;
            }

            pbstrEnlistmentPath = pbstrEnlistmentPathUNC = trueProjectPath;

            return VSConstants.S_OK;
        }

        private string CalculateTruePath(string lpszProjectPath)
        {
            string trueName = SvnTools.GetTruePath(lpszProjectPath, true);

            if (trueName != lpszProjectPath)
            {
                if (trueName.Length == lpszProjectPath.Length - 1 && lpszProjectPath[trueName.Length] == '\\')
                    trueName += '\\';
            }

            return trueName;
        }

        static string MakeRelative(string relativeFrom, string path)
        {
            return PackageUtilities.MakeRelative(relativeFrom, path);
        }

        string MakeRelativeNoCase(string relativeFrom, string path)
        {
            string rp = MakeRelative(relativeFrom.ToUpperInvariant(), path.ToUpperInvariant());

            if (string.IsNullOrEmpty(rp) || IsSafeSccPath(rp))
                return path;

            int back = rp.LastIndexOf("..\\", StringComparison.Ordinal);
            if (back >= 0)
            {
                int rest = rp.Length - back - 3;

                return rp.Substring(0, back + 3) + path.Substring(path.Length - rest, rest);
            }
            else
                return path.Substring(path.Length - rp.Length, rp.Length);
        }

        internal void Translate_SolutionRenamed(string oldName, string newName)
        {
            string oldDir = Path.GetDirectoryName(oldName);
            string newDir = Path.GetDirectoryName(newName);
            string newNameU = newName.ToUpperInvariant();

            if (oldDir == newDir)
                return;
            string oldDirRoot = Path.GetPathRoot(oldDir);

            Dictionary<string, string> oldNameMap = new Dictionary<string, string>(_trueNameMap);
            _trueNameMap.Clear();

            foreach (KeyValuePair<string, string> kv in oldNameMap)
            {
                if (!IsSafeSccPath(kv.Key) || !IsSafeSccPath(kv.Value))
                {
                    // Just copy translations like http://localhost
                    _trueNameMap.Add(kv.Key, kv.Value);
                    continue;
                }

                string newRel = MakeRelativeNoCase(newName, kv.Key);

                if (IsSafeSccPath(newRel))
                    continue; // Not relative from .sln after

                string newPath = Path.GetFullPath(Path.Combine(newDir, newRel));

                if (newPath == kv.Key)
                    continue; // No translation necessary after the rename
                _trueNameMap[kv.Key] = newPath;
            }
        }

        void Translate_ClearState()
        {
            _trueNameMap.Clear();
        }

        #region IAnkhSccService Members


        public bool HasSolutionData
        {
            get { return IsSolutionManaged; }
        }


        #endregion

        /// <summary>
        /// Writes the enlistment state to the solution
        /// </summary>
        /// <param name="pPropBag">The p prop bag.</param>
        public void WriteSolutionProperties(IPropertyMap propertyBag)
        {
            // Reimplemented in 2.4
        }

        public void ReadSolutionProperties(IPropertyMap propertyBag)
        {
            // Reimplemented in 2.4
        }

        /// <summary>
        /// Serializes the enlist data.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="writeData">if set to <c>true</c> [write data].</param>
        void IAnkhSccService.SerializeEnlistData(Stream store, bool writeData)
        {
            // Reimplemented in 2.4
        }
    }
}
