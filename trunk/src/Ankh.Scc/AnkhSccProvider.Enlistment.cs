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
        bool _enlistCompleted;// = false;

        readonly Dictionary<string, string> _trueNameMap = new Dictionary<string, string>();
        readonly Dictionary<string, SccSvnOrigin> _originMap = new Dictionary<string, SccSvnOrigin>();
        int IVsSccEnlistmentPathTranslation.TranslateEnlistmentPathToProjectPath(string lpszEnlistmentPath, out string pbstrProjectPath)
        {
            if (_trueNameMap.TryGetValue(lpszEnlistmentPath, out pbstrProjectPath))
            {
                // Already set the path
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
                pbstrEnlistmentPathUNC = info.EnlistmentPathUNC;
                return VSConstants.S_OK;
            }

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
            _enlistCompleted = false;
            _trueNameMap.Clear();
            _originMap.Clear();
            _translationMap.Clear();
        }

        Dictionary<string, SccTranslatePathInfo> _translationMap = new Dictionary<string, SccTranslatePathInfo>();
        private bool TryGetTranslation(string path, out SccTranslatePathInfo info)
        {
            if (string.IsNullOrEmpty(path))
            {
                info = null;
                return false;
            }

            return _translationMap.TryGetValue(path, out info);
        }

        bool MapProject(IVsHierarchy pHierarchy, out string location, out SccProjectData data)
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

            if (data != null && !string.IsNullOrEmpty(data.ProjectLocation))
            {
                location = data.ProjectLocation;
                return true;
            }

            IVsSolution2 sln = GetService<IVsSolution2>(typeof(SVsSolution));

            if (sln != null
                && ErrorHandler.Succeeded(sln.GetUniqueNameOfProject(pHierarchy, out location)))
                return !string.IsNullOrEmpty(location);

            location = null;
            return false;
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

            string originalLocation;

            if (!_trueNameMap.TryGetValue(location, out originalLocation))
                originalLocation = location;

            return _originMap.ContainsKey(originalLocation);
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
                origin.Enlist = data.EnlistMode.ToString();

            origin.Write(map);
        }

        public void ReadProjectProperties(IVsHierarchy pHierarchy, string pszProjectName, string pszProjectMk, IPropertyMap map)
        {
            SccSvnOrigin origin = new SccSvnOrigin();
            origin.Load(map);

            _originMap[pszProjectMk] = origin;

            if (!string.IsNullOrEmpty(origin.Enlist))
                EnsureEnlistment(pszProjectName, pszProjectMk, origin);
        }

        void EnsureEnlistment(string pszProjectName, string pszProjectMk, SccSvnOrigin origin)
        {
            EnsureEnlistUserSettings();

            SccTranslatePathInfo tpi;
            if (TryGetTranslation(pszProjectMk, out tpi))
                return; // We have existing local data

            IVsSolution sol = GetService<IVsSolution>(typeof(SVsSolution));

            if (sol == null)
                return;

            IVsProjectFactory factory;
            if (!ErrorHandler.Succeeded(sol.GetProjectFactory(0, null, pszProjectMk, out factory)))
                return;

            IVsSccProjectEnlistmentFactory enlistFactory = factory as IVsSccProjectEnlistmentFactory;
            if (enlistFactory == null)
                return;

            string enlistPath, enlistPathUNC;
            if (!ErrorHandler.Succeeded(enlistFactory.GetDefaultEnlistment(pszProjectMk, out enlistPath, out enlistPathUNC)))
                return;

            tpi = new SccTranslatePathInfo();
            tpi.SolutionPath = pszProjectMk;
            tpi.EnlistmentPath = enlistPath;
            tpi.EnlistmentPathUNC = SvnTools.GetTruePath(enlistPathUNC, true) ?? enlistPathUNC;

            _translationMap[pszProjectMk] = tpi;

            if (enlistPathUNC != pszProjectMk)
                _trueNameMap[enlistPathUNC] = pszProjectMk;
        }

        private void EnsureEnlistUserSettings()
        {
            //throw new NotImplementedException();
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
            if (!IsActive || !IsSolutionManaged)
                return;
#if DEBUG_ENLISTMENT
            SortedList<string, string> projects = new SortedList<string, string>(StringComparer.Ordinal);
            SortedList<string, string> values = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);

            string projectDir = SolutionDirectory;

            IAnkhSolutionSettings ss = GetService<IAnkhSolutionSettings>();
            Uri solutionUri = null;

            if (ss != null)
                projectDir = ss.ProjectRootWithSeparator;
            else
                projectDir = projectDir.TrimEnd('\\') + '\\';

            string normalizedProjectDir = SvnTools.GetNormalizedFullPath(projectDir);

            foreach (SccProjectData project in _projectMap.Values)
            {
                if (string.IsNullOrEmpty(project.ProjectDirectory) || !project.IsManaged)
                    continue; // Solution folder or unmanaged?

                bool enlist = false;
                bool enlistOptional = true;
                IVsSccProjectEnlistmentChoice projectChoice = project.VsProject as IVsSccProjectEnlistmentChoice;

                if (projectChoice != null)
                {
                    VSSCCENLISTMENTCHOICE[] choice = new VSSCCENLISTMENTCHOICE[1];

                    if (ErrorHandler.Succeeded(projectChoice.GetEnlistmentChoice(choice)))
                    {
                        switch (choice[0])
                        {
                            case VSSCCENLISTMENTCHOICE.VSSCC_EC_NEVER:
                                // Don't take any enlistment actions
                                break;
                            case VSSCCENLISTMENTCHOICE.VSSCC_EC_COMPULSORY:
                                enlist = true;
                                enlistOptional = false;
                                break;
                            case VSSCCENLISTMENTCHOICE.VSSCC_EC_OPTIONAL:
                                enlistOptional = enlist = true;
                                break;
                        }
                    }
                }

                string dir = SvnTools.GetNormalizedFullPath(project.ProjectDirectory);
                string file = project.ProjectFile;

                if (!enlist && dir.StartsWith(projectDir, StringComparison.OrdinalIgnoreCase)
                    || normalizedProjectDir.Equals(dir, StringComparison.OrdinalIgnoreCase))
                {
                    // The directory is below our project root, we can ignore it
                    //  - Yes we can, unless the directory is switched or nested below the root

                    // TODO: Check those conditions somewhere else and reuse here                    
                    continue;
                }

                SvnItem item = StatusCache[dir];

                if (solutionUri == null)
                {
                    SvnItem solDirItem = StatusCache[SolutionDirectory];

                    if (solDirItem != null && solDirItem.IsVersioned && solDirItem.Status != null && solDirItem.Status.Uri != null)
                        solutionUri = solDirItem.Status.Uri;
                }

                if (item == null || !item.IsVersioned || item.Status == null || item.Status.Uri == null)
                    continue;

                Uri itemUri = item.Status.Uri;

                if (solutionUri != null)
                    itemUri = solutionUri.MakeRelativeUri(itemUri);

                if (StatusCache.IsValidPath(file))
                    file = PackageUtilities.MakeRelative(dir, file);

                // This should match the directory as specified in the solution!!!
                // (It currently does, but only because we don't really support virtual folders yet)
                dir = PackageUtilities.MakeRelative(projectDir, dir);

                string prefix = project.ProjectGuid.ToString("B").ToUpperInvariant();
                projects.Add(prefix, prefix);

                prefix = "Project." + prefix;

                values[prefix + ".Path"] = Quote(dir);

                if (!string.IsNullOrEmpty(file))
                    values[prefix + ".Project"] = Quote(PackageUtilities.MakeRelative(dir, file));

                values[prefix + ".Uri"] = Quote(Uri.EscapeUriString(itemUri.ToString()));
                if (enlist)
                {
                    // To enlist a project we need its project type (to get to the project factory)
                    values[prefix + ".EnlistType"] = project.ProjectTypeGuid.ToString("B").ToUpperInvariant();
                }
            }

            IVsSolution solution = null;
            foreach (IVsHierarchy hier in GetAllProjectsInSolutionRaw())
            {
                IVsSccProject2 scc = hier as IVsSccProject2;

                if (scc != null && _projectMap.ContainsKey(scc))
                    continue;

                // OK: 2 options
                //  * Unloaded project
                //    -> Keep state from previous version
                //  * Not scc capable project
                //    -> TODO: Look at our options

                if(solution == null)
                    solution = GetService<IVsSolution>(typeof(SVsSolution));

                Guid projectGuid;
                if(ErrorHandler.Succeeded(solution.GetGuidOfProject(hier, out projectGuid)))
                {
                    string id = projectGuid.ToString("B").ToUpperInvariant();
                    foreach(EnlistData data in _enlistState)
                    {
                        if(data.ProjectId == id)
                        {
                            projects.Add(id, id);
                            string prefix = "Project." + id;
                            
                            projects[prefix + ".Path"] = Quote(data.Directory);
                            if(!string.IsNullOrEmpty(data.RawFile))
                                projects[prefix + ".File"] =  Quote(data.RawFile);

                            if(!string.IsNullOrEmpty(data.EnlistType))
                                projects[prefix + ".EnlistType"] = Quote(data.EnlistType);

                            projects[prefix + ".Uri"] = Quote(Uri.EscapeUriString(data.Uri.ToString()));
                            break;
                        }
                    }
                }
            }

            // We write all values in alphabetical order to make sure we don't change the solution unnecessary
            StringBuilder projectString = new StringBuilder();
            foreach (string s in projects.Values)
            {
                if (projectString.Length > 0)
                    projectString.Append(", ");

                projectString.Append(s);
            }

            object value = projectString.ToString();
            propertyBag.Write("Projects", ref value);            

            foreach (KeyValuePair<string, string> kv in values)
            {
                value = kv.Value;
                propertyBag.Write(kv.Key, ref value);
            }
#endif
        }

        class EnlistBase
        {
            readonly string _projectId;

            public EnlistBase(string projectId)
            {
                if (string.IsNullOrEmpty(projectId))
                    throw new ArgumentNullException("projectId");

                _projectId = projectId;
            }

            public string ProjectId
            {
                get { return _projectId; }
            }

            string[] _userData;

            public virtual void LoadUserData(List<string> values)
            {
                _userData = values.ToArray();
            }

            internal string[] GetUserData()
            {
                return (string[])(_userData ?? new string[0]).Clone();
            }

            internal bool ShouldSerialize()
            {
                return true;
            }
        }

        sealed class EnlistData : EnlistBase
        {
            readonly string _projectId;
            readonly string _directory;
            readonly string _path;
            readonly string _rawFile;
            readonly Uri _uri;
            readonly string _enlistType;

            public EnlistData(string projectId, string directory, string file, Uri uri, string enlistType)
                : base(projectId)
            {
                if (string.IsNullOrEmpty(directory))
                    throw new ArgumentNullException("directory");
                else if (uri == null)
                    throw new ArgumentNullException("uri");

                _projectId = projectId;
                _directory = directory;
                _rawFile = file;
                if (file == null)
                    _path = directory.TrimEnd('\\') + '\\';
                else
                    _path = System.IO.Path.Combine(directory, file);

                _uri = uri;
                _enlistType = enlistType;
            }

            internal string RawFile
            {
                get { return _rawFile; }
            }

            public string Directory
            {
                get { return _directory; }
            }

            public string Path
            {
                get { return _directory; }
            }

            public string ProjectFile
            {
                get { return _path; }
            }

            public Uri Uri
            {
                get { return _uri; }
            }

            public string EnlistType
            {
                get { return _enlistType; }
            }

            public override void LoadUserData(List<string> values)
            {
                base.LoadUserData(values);
            }
        }

        /// <summary>
        /// Serializes the enlist data.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="writeData">if set to <c>true</c> [write data].</param>
        void IAnkhSccService.SerializeEnlistData(Stream store, bool writeData)
        {
            if (writeData)
                WriteEnlistData(store);
            else
                ReadEnlistUserData(store);
        }

        const int EnlistSerializerVersion = 1;

        private void ReadEnlistUserData(Stream store)
        {
            if (store.Length < 2 * sizeof(int))
                return;

            using (BinaryReader br = new BinaryReader(store))
            {
                int version = br.ReadInt32(); // The enlist version used to write the data
                int requiredVersion = br.ReadInt32(); // All enlist versions older then this should ignore all data

                if ((requiredVersion > EnlistSerializerVersion) || version < requiredVersion)
                    return; // Older versions (we) should ignore this data

                int count = br.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    Guid project = new Guid(br.ReadBytes(16));
                    int stringCount = br.ReadInt32();

                    List<string> values = new List<string>();

                    for (int j = 0; i < stringCount; j++)
                        values.Add(br.ReadString());

                    string projectId = project.ToString("B").ToUpperInvariant();

                    /*if (!_enlistStore.ContainsKey(projectId))
                    {   // Don't overwrite; we load 1 or 2 times during solution opening

                        EnlistBase enlist = new EnlistBase(projectId);

                        enlist.LoadUserData(values);
                        _enlistStore[projectId] = enlist;
                    }*/
                }
            }
        }

        private void WriteEnlistData(Stream store)
        {
            using (BinaryWriter bw = new BinaryWriter(store))
            {
                bw.Write((int)1); // Minimum version required to read these settings; update if incompatible
                bw.Write((int)EnlistSerializerVersion); // Writer version

                List<EnlistBase> list = new List<EnlistBase>();
                IVsSolution sol = GetService<IVsSolution2>(typeof(SVsSolution));

                /*foreach (IVsHierarchy hier in GetAllProjectsInSolutionRaw())
                {
                    Guid g;
                    if (!ErrorHandler.Succeeded(sol.GetGuidOfProject(hier, out g)))
                        continue;

                    string projectId = g.ToString("B").ToUpperInvariant();

                    EnlistBase enlist;
                    if (_enlistStore.TryGetValue(projectId, out enlist) && enlist.ShouldSerialize())
                        list.Add(enlist);
                }*/

                bw.Write((int)list.Count);
                foreach (EnlistBase b in list)
                {
                    string[] values = b.GetUserData();

                    bw.Write((int)values.Length);

                    foreach (string s in values)
                        bw.Write(s);
                }
            }
        }

        string Quote(string value)
        {
            return '\"' + value + '\"';
        }
    }
}
