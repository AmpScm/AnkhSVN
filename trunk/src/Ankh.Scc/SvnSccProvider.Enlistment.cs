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
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;

using Ankh.Scc.ProjectMap;
using Ankh.Scc.SettingMap;
using Ankh.UI;

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

    partial class SvnSccProvider
    {
        bool _translateDataLoaded;
        readonly Dictionary<string, string> _trueNameMap = new Dictionary<string, string>();
        readonly Dictionary<string, SccSvnOrigin> _originMap = new Dictionary<string, SccSvnOrigin>();

        int IVsSccEnlistmentPathTranslation.TranslateEnlistmentPathToProjectPath(string lpszEnlistmentPath, out string pbstrProjectPath)
        {
            if (_trueNameMap.TryGetValue(lpszEnlistmentPath, out pbstrProjectPath))
            {
                // Already set the path
                return VSErr.S_OK;
            }
            else if (_trueNameMap.TryGetValue(lpszEnlistmentPath + "\\", out pbstrProjectPath))
            {
                pbstrProjectPath += "\\";
                return VSErr.S_OK;
            }
            else if (_trueNameMap.TryGetValue(lpszEnlistmentPath.TrimEnd('\\'), out pbstrProjectPath))
            {
                return VSErr.S_OK;
            }

            if (!IsSafeSccPath(lpszEnlistmentPath))
            {
                pbstrProjectPath = lpszEnlistmentPath;
                return VSErr.S_OK;
            }

            pbstrProjectPath = lpszEnlistmentPath;

            return VSErr.S_OK;
        }

        /// <summary>
        /// Translates a possibly virtual project path to a local path and an enlistment physical path.
        /// </summary>
        /// <param name="lpszProjectPath">[in] The project's (possibly) virtual path as obtained from the solution file.</param>
        /// <param name="pbstrEnlistmentPath">[out] The local path used by the solution for loading and saving the project.</param>
        /// <param name="pbstrEnlistmentPathUNC">[out] The path used by the source control system for managing the enlistment ("\\drive\path", "[drive]:\path", "file://server/path").</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSErr.S_OK"/>. If it fails, it returns an error code.
        /// </returns>
        public int TranslateProjectPathToEnlistmentPath(string lpszProjectPath, out string pbstrEnlistmentPath, out string pbstrEnlistmentPathUNC)
        {
            // Summarized: lpszProjectPath         = Path as stored in the .sln
            //             pbstrEnlistmentPath     = Path used to load the project (e.g. http://localhost/site)
            //             pbstrEnlistmentPathUNC  = UNC Path to the project on disk
            //                                       (e.g. c:\inetpub\wwwroot\site for http://localhost/site
            SccTranslatePathInfo info;
            if (TryGetTranslation(lpszProjectPath, out info))
            {
                pbstrEnlistmentPath = info.EnlistmentPath;
                pbstrEnlistmentPathUNC = info.EnlistmentUNCPath;
                return VSErr.S_OK;
            }

            if (!IsSafeSccPath(lpszProjectPath))
            {
                pbstrEnlistmentPath = pbstrEnlistmentPathUNC = lpszProjectPath;
                return VSErr.S_OK;
            }

            string trueProjectPath = CalculateTruePath(lpszProjectPath);

            if (trueProjectPath != lpszProjectPath)
            {
                _trueNameMap[trueProjectPath] = lpszProjectPath;
            }

            pbstrEnlistmentPath = pbstrEnlistmentPathUNC = trueProjectPath;

            return VSErr.S_OK;
        }

        private string CalculateTruePath(string lpszProjectPath)
        {
            string trueName = SvnTools.GetTruePath(lpszProjectPath, true) ?? SvnTools.GetNormalizedFullPath(lpszProjectPath);

            if (trueName != lpszProjectPath)
            {
                if (trueName.Length == lpszProjectPath.Length - 1 && lpszProjectPath[trueName.Length] == '\\')
                    trueName += '\\';
            }

            return trueName;
        }

        public override void Translate_SolutionRenamed(string oldName, string newName)
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

                string newRel = SvnItem.MakeRelativeNoCase(newName, kv.Key);

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
            _translateDataLoaded = false;
            _trueNameMap.Clear();
            _originMap.Clear();
            _translationMap.Clear();
        }

        public void ProjectLoadFailed(string pszProjectMk)
        {
        }

        Dictionary<string, SccTranslatePathInfo> _translationMap = new Dictionary<string, SccTranslatePathInfo>();
        private bool TryGetTranslation(string path, out SccTranslatePathInfo info)
        {
            EnsureEnlistUserSettings();

            if (string.IsNullOrEmpty(path))
            {
                info = null;
                return false;
            }

            if (_translationMap.TryGetValue(path, out info))
                return true;

            if (_translationMap.TryGetValue(path + "\\", out info))
                return true;

            return false;
        }

        bool MapProject(IVsHierarchy pHierarchy, out string slnLocation, out SccProjectData data)
        {
            IVsSccProject2 sccProject = pHierarchy as IVsSccProject2;

            if (sccProject != null && _projectMap.TryGetValue(sccProject, out data))
            {
                // data valid
            }
            else if (sccProject != null)
            {
                data = new SccProjectData(this, sccProject);
            }
            else
                data = null;

            string projectLocation = null;
            try
            {
                if (data != null && !string.IsNullOrEmpty(data.ProjectLocation))
                {
                    projectLocation = data.ProjectLocation;
                    return true;
                }

                IVsSolution2 sln = GetService<IVsSolution2>(typeof(SVsSolution));

                if (sln != null
                    && VSErr.Succeeded(pHierarchy.GetCanonicalName(VSItemId.Root, out projectLocation)))
                    return !string.IsNullOrEmpty(projectLocation);

                projectLocation = null;
                return false;
            }
            finally
            {
                if (projectLocation == null || !_trueNameMap.TryGetValue(projectLocation, out slnLocation))
                    slnLocation = projectLocation;
            }
        }

        public bool HasProjectProperties(IVsHierarchy pHierarchy)
        {
            string location;
            SccProjectData data;

            if (!MapProject(pHierarchy, out location, out data))
                return false;

            if (data != null)
            {
                if (!data.IsManaged)
                    return false;
                else if (data.EnlistMode != SccEnlistChoice.Never)
                    return true;
            }

            return _originMap.ContainsKey(location);
        }

        public void StoreProjectProperties(IVsHierarchy pHierarchy, IPropertyMap map)
        {
            string location;
            SccProjectData data;

            if (!MapProject(pHierarchy, out location, out data))
                return;

            SccSvnOrigin origin;
            if (!_originMap.TryGetValue(location, out origin))
            {
                if (data == null || data.EnlistMode == SccEnlistChoice.Never)
                    return;

                /* This project type wants to be enlisted.
                 * Store that information now to allow handling it on sln opening
                 */
                _originMap[location] = origin = new SccSvnOrigin();
            }

            if (data != null)
            {
                SccEnlistChoice choice = data.EnlistMode;
                if (choice != SccEnlistChoice.Never)
                {
                    origin.Enlist = choice.ToString();

                    if (string.IsNullOrEmpty(origin.SvnUri))
                        UpdateOriginUri(pHierarchy);
                }
                else
                    origin.Enlist = null;
            }

            origin.Write(map);
        }

        private void UpdateOriginUri(IVsHierarchy pHierarchy)
        {
            string location;
            SccProjectData data;

            if (string.IsNullOrEmpty(SolutionDirectory))
                return;

            if (!MapProject(pHierarchy, out location, out data))
                return;

            if (data == null)
                return;

            SccSvnOrigin origin;
            if (!_originMap.TryGetValue(location, out origin))
                return;

            if (data.ProjectDirectory == null)
                return;

            SvnItem dirItem = StatusCache[data.ProjectDirectory];
            SvnItem slnDirItem = StatusCache[SolutionDirectory];

            if (!dirItem.IsVersioned)
                return;

            Uri dirUri = dirItem.Uri;
            Uri slnDirUri;

            SvnWorkingCopy slnWc = slnDirItem.WorkingCopy;

            if (slnWc == null)
                return;

            if (slnDirItem.IsVersioned)
                slnDirUri = slnDirItem.Uri;
            else
            {
                origin.SvnUri = dirItem.Uri.AbsoluteUri;
                return;
            }

            Uri relUri = slnDirUri.MakeRelativeUri(dirUri);

            if (relUri.IsAbsoluteUri)
                origin.SvnUri = relUri.AbsoluteUri;
            else
                origin.SvnUri = relUri.ToString();
        }

        public void ReadProjectProperties(IVsHierarchy pHierarchy, string pszProjectName, string pszProjectMk, IPropertyMap map)
        {
            string slnProjectName;

            if (!_trueNameMap.TryGetValue(pszProjectMk, out slnProjectName))
                slnProjectName = pszProjectMk;

            SccSvnOrigin origin = new SccSvnOrigin();
            origin.Load(map);

            _originMap[slnProjectName] = origin;

            if (!string.IsNullOrEmpty(origin.Enlist))
                EnsureEnlistment(slnProjectName, pszProjectMk, origin);
        }

        void EnsureEnlistment(string slnProjectName, string pszProjectMk, SccSvnOrigin origin)
        {
            SccTranslatePathInfo tpi;
            if (TryGetTranslation(slnProjectName, out tpi))
                return; // We have existing local data

            IVsSolution sol = GetService<IVsSolution>(typeof(SVsSolution));

            if (sol == null)
                return;

            IVsProjectFactory factory;
            if (!VSErr.Succeeded(sol.GetProjectFactory(0, null, pszProjectMk, out factory)))
                return;

            IVsSccProjectEnlistmentFactory enlistFactory = factory as IVsSccProjectEnlistmentFactory;
            if (enlistFactory == null)
                return;

            string enlistPath, enlistPathUNC;
            if (!VSErr.Succeeded(enlistFactory.GetDefaultEnlistment(pszProjectMk, out enlistPath, out enlistPathUNC)))
                return;

            // ### We should now proceed with the editing operations as documented
            // ### in IVsSccEnlistmentPathTranslation.idl

            // ### But since we don't have per user settings yet we currently just
            // ### pass the defaults as that happens to be the same behavior as
            // ### before

            if (IsSafeSccPath(pszProjectMk)
                && !string.IsNullOrEmpty(SolutionSettings.ProjectRoot)
                /*&& (SvnItem.IsBelowRoot(pszProjectMk, SolutionSettings.ProjectRoot) || string.IsNullOrEmpty(origin.SvnUri))*/
                && StatusCache != null && StatusCache[pszProjectMk].Exists)
            {
                int pfValidEnlistment;
                string chosenUNC;

                // We have a website that has a default location below our project root or without a Svn Uri
                // At least for now we should default to enlist at that location to make
                // sure we don't break backwards compatibility
                string suffix = ".sccEnlistAttemptLocation";

                if (VSErr.Succeeded(enlistFactory.ValidateEnlistmentEdit(0, slnProjectName, pszProjectMk+suffix, out chosenUNC, out pfValidEnlistment))
                    && pfValidEnlistment != 0
                    && chosenUNC.EndsWith(suffix))
                {
                    enlistPath = pszProjectMk;
                    enlistPathUNC = chosenUNC.Substring(0, chosenUNC.Length - suffix.Length);
                }
            }

            string slnProjectMk;

            // Maybe we already translated the path?
            if (!_trueNameMap.TryGetValue(pszProjectMk, out slnProjectMk))
                slnProjectMk = pszProjectMk;

            tpi = new SccTranslatePathInfo(slnProjectMk, enlistPath, CalculateTruePath(enlistPathUNC));

            _translationMap[slnProjectMk] = tpi;

            if (enlistPathUNC != slnProjectMk)
                _trueNameMap[enlistPathUNC] = slnProjectMk;
        }

        public IDictionary<string, object> GetProjectsThatNeedEnlisting()
        {
            if (_translationMap.Count == 0)
                return null;

            Guid guidEmpty = Guid.Empty;
            IVsSolution solution = GetService<IVsSolution>(typeof(SVsSolution));

            if (solution == null)
                return null;

            IEnumHierarchies ppEnum;
            if (!VSErr.Succeeded(solution.GetProjectEnum((uint)Microsoft.VisualStudio.Shell.Interop.__VSENUMPROJFLAGS.EPF_UNLOADEDINSOLUTION, ref guidEmpty, out ppEnum)))
                return null;

            IVsHierarchy[] hiers = new IVsHierarchy[16];

            uint iFetched;
            Dictionary<string, object> map = new Dictionary<string, object>();
            while (VSErr.Succeeded(ppEnum.Next((uint)hiers.Length, hiers, out iFetched)))
            {
                for (uint i = 0; i < iFetched; i++)
                {
                    string name;
                    if (VSErr.Succeeded(hiers[i].GetCanonicalName(VSItemId.Root, out name)))
                    {
                        string slnName;
                        if (!_trueNameMap.TryGetValue(name, out slnName))
                            slnName = name;

                        SccSvnOrigin origin;
                        if (_originMap.TryGetValue(slnName, out origin)
                            && !string.IsNullOrEmpty(origin.Enlist))
                        {
                            map.Add(name, hiers[i]);
                        }
                    }
                }

                if ((int)iFetched < hiers.Length)
                    break;
            }

            return map;
        }

        internal void EnlistAndCheckout(IVsHierarchy vsHierarchy, string pszProjectMk)
        {
            string slnProjectMk;

            if (!_trueNameMap.TryGetValue(pszProjectMk, out slnProjectMk))
                slnProjectMk = pszProjectMk;

            SccSvnOrigin origin;

            if (!_originMap.TryGetValue(slnProjectMk, out origin))
                return;

            if (string.IsNullOrEmpty(origin.SvnUri))
                return;

            IVsSolution sol = GetService<IVsSolution>(typeof(SVsSolution));
            if (sol == null)
                return;

            IVsProjectFactory factory;
            if (!VSErr.Succeeded(sol.GetProjectFactory(0, null, pszProjectMk, out factory)))
                return;

            IVsSccProjectEnlistmentFactory enlistFactory = factory as IVsSccProjectEnlistmentFactory;
            if (enlistFactory == null)
                return;

            string enlistPath, enlistPathUNC;
            if (!VSErr.Succeeded(enlistFactory.GetDefaultEnlistment(pszProjectMk, out enlistPath, out enlistPathUNC)))
                return;

            uint flags;
            int hr;
            // Website projects return E_NOTIMPL on these methods
            if (!VSErr.Succeeded(hr = enlistFactory.GetEnlistmentFactoryOptions(out flags)))
            {
                if (hr != VSErr.E_NOTIMPL)
                    return;
                flags = 0;
            }
            if (!VSErr.Succeeded(hr = enlistFactory.OnBeforeEnlistmentCreate(pszProjectMk, enlistPath, enlistPathUNC)))
                return;

            Uri projectUri;
            if (!Uri.TryCreate(origin.SvnUri, UriKind.Absolute, out projectUri))
            {
                Uri relativeUri;

                if (!Uri.TryCreate(origin.SvnUri, UriKind.Relative, out relativeUri))
                    return;

                // We have a Uri relative from the solution file
                if (StatusCache == null)
                    return;

                SvnItem slnDirItem = StatusCache[SolutionDirectory];

                if (!slnDirItem.IsVersioned || slnDirItem.Uri == null)
                    return;

                projectUri = new Uri(slnDirItem.Uri, relativeUri);
            }

            GetService<IProgressRunner>().RunModal(Resources.CheckingOutProject,
                delegate(object sender, ProgressWorkerArgs e)
                {
                    e.Client.CheckOut(projectUri, SvnTools.GetNormalizedFullPath(enlistPathUNC));
                });

           if (!VSErr.Succeeded(hr = enlistFactory.OnAfterEnlistmentCreate(pszProjectMk, enlistPath, enlistPathUNC)))
                return;

           SccTranslatePathInfo tpi = new SccTranslatePathInfo(slnProjectMk, enlistPath, enlistPathUNC);

            _translationMap[tpi.SolutionPath] = tpi;

            if (tpi.SolutionPath != tpi.EnlistmentUNCPath)
                _trueNameMap[tpi.EnlistmentUNCPath] = tpi.SolutionPath;
        }

        internal void EditEnlistment(IVsHierarchy vsHierarchy, string p)
        {
            throw new NotImplementedException();
        }

        private void EnsureEnlistUserSettings()
        {
            if (_translateDataLoaded)
                return;

            _translateDataLoaded = true;
            IAnkhPackage pkg = GetService<IAnkhPackage>();

            if (pkg != null)
                pkg.ForceLoadUserSettings(AnkhId.SccTranslateStream);
        }

        /// <summary>
        /// Serializes the enlist data.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="writeData">if set to <c>true</c> [write data].</param>
        void IAnkhSccService.SerializeSccTranslateData(Stream store, bool writeData)
        {
            if (writeData)
                WriteTranslateUserData(store);
            else
                ReadTranslateUserData(store);
        }

        const int TranslateSerializerVersion = 1;
        const int MinSupportedTranslateSerializerVersion = 1;

        private void ReadTranslateUserData(Stream store)
        {
            _translateDataLoaded = true;
            if (store.Length < 2 * sizeof(int))
                return;

            using (BinaryReader br = new BinaryReader(store))
            {
                int version = br.ReadInt32(); // The enlist version used to write the data
                int requiredVersion = br.ReadInt32(); // All enlist versions older then this should ignore all data

                if ((requiredVersion > TranslateSerializerVersion) || version < MinSupportedTranslateSerializerVersion)
                    return; // Older versions (we) should ignore this data

                string oldSolutionDir = br.ReadString();
                string slnDir = RawSolutionDirectory ?? oldSolutionDir;
                int nItems = br.ReadInt32();

                for (int iItem = 0; iItem < nItems; iItem++)
                {
                    int nStrings = br.ReadInt32();
                    string[] strings = new string[nStrings];

                    for (int iString = 0; iString < nStrings; iString++)
                    {
                        bool relative = br.ReadBoolean();

                        string path = br.ReadString();

                        if (relative)
                            path = Path.Combine(slnDir, path);

                        strings[iString] = path;
                    }

                    if (strings.Length >= 3)
                    {
                        SccTranslatePathInfo tpi = new SccTranslatePathInfo(strings[0], strings[1], strings[2]);

                        if (!_translationMap.ContainsKey(tpi.SolutionPath))
                        {
                            _translationMap[tpi.SolutionPath] = tpi;

                            if (tpi.SolutionPath != tpi.EnlistmentUNCPath)
                                _trueNameMap[tpi.EnlistmentUNCPath] = tpi.SolutionPath;
                        }
                    }
                }
            }
        }

        private void WriteTranslateUserData(Stream store)
        {
            // I use explicit casts in the writer function to document the type we write

            using (BinaryWriter bw = new BinaryWriter(store))
            {
                bw.Write((int)1); // Minimum version required to read these settings; update if incompatible
                bw.Write((int)TranslateSerializerVersion); // Writer version

                string solutionDir = SolutionDirectory;
                string rootDir = solutionDir;

                if (SolutionSettings != null && !string.IsNullOrEmpty(SolutionSettings.ProjectRoot))
                    rootDir = SolutionSettings.ProjectRoot;

                bw.Write((string)solutionDir);

                bw.Write((int)_translationMap.Count);

                foreach (SccTranslatePathInfo tpi in _translationMap.Values)
                {
                    string[] paths = new string[] { tpi.SolutionPath, tpi.EnlistmentPath, tpi.EnlistmentUNCPath};
 
                    bw.Write((int)paths.Length);

                    foreach(string p in paths)
                    {
                        bool relative = false;
                        string path = p;
                        if (IsSafeSccPath(p) && SvnItem.IsBelowRoot(p, rootDir))
                        {
                            relative = true;
                            path = SvnItem.MakeRelativeNoCase(solutionDir, p);

                            if (p.EndsWith("\\") && !path.EndsWith("\\"))
                                path += '\\';
                        }

                        bw.Write((bool)relative);
                        bw.Write((string)path);
                    }
                }
            }
        }
    }
}
