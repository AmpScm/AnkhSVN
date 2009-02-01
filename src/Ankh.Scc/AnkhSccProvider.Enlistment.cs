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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using SharpSvn;

using Ankh.Ids;
using Ankh.Scc.ProjectMap;
using Ankh.Selection;
using Ankh.VS;
using System.Runtime.CompilerServices;
using System.IO;

namespace Ankh.Scc
{
    [InterfaceType(1), ComImport]
    [Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
    interface IMyPropertyBag
    {
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int Read(string pszPropName, out object pVar, IErrorLog pErrorLog, uint VARTYPE, object pUnkObj);

        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int Write(string pszPropName, ref object pVar);
    }

    partial class AnkhSccProvider : IVsAsynchOpenFromScc
    {
        bool _solutionLoaded;
        readonly Dictionary<string, EnlistBase> _enlistStore = new Dictionary<string, EnlistBase>();
        readonly List<EnlistData> _enlistState = new List<EnlistData>();
        bool _enlistCompleted;// = false;
        readonly List<SccTranslateData> _translations = new List<SccTranslateData>();
        readonly Dictionary<string, SccTranslateData> _storedPaths = new Dictionary<string, SccTranslateData>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, SccTranslateData> _sccPaths = new Dictionary<string, SccTranslateData>(StringComparer.OrdinalIgnoreCase);

        void ClearEnlistState()
        {
            _enlistStore.Clear();
            _enlistState.Clear();
            _enlistCompleted = false;

            _translations.Clear();
            _storedPaths.Clear();
            _sccPaths.Clear();
        }

        #region IAnkhSccService Members


        public bool HasSolutionData
        {
            get { return false; }
        }


        #endregion

        protected IEnumerable<IVsHierarchy> GetAllProjectsInSolutionRaw()
        {
            IVsSolution solution = (IVsSolution)Context.GetService(typeof(SVsSolution));

            if (solution == null)
                yield break;

            Guid none = Guid.Empty;
            IEnumHierarchies hierEnum;
            if (!ErrorHandler.Succeeded(solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_ALLPROJECTS, ref none, out hierEnum)))
                yield break;

            IVsHierarchy[] hiers = new IVsHierarchy[32];
            uint nFetched;
            while (ErrorHandler.Succeeded(hierEnum.Next((uint)hiers.Length, hiers, out nFetched)))
            {
                if (nFetched == 0)
                    break;
                for (int i = 0; i < nFetched; i++)
                {
                    yield return hiers[i];
                }
            }
        }

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

        public void ReadSolutionProperties(IPropertyMap propertyBag)
        {
#if DEBUG_ENLISTMENT
            IMyPropertyBag mpb = (IMyPropertyBag)propertyBag; // Stop HResult exception handling
            object value;
            string projects;

            if(!ErrorHandler.Succeeded(mpb.Read("Projects", out value, null, 0, null)))
                return;
            
            projects = ((string)value).Trim();

            if (string.IsNullOrEmpty(projects))
                return;

            foreach(string project in projects.Split(','))
            {
                Guid projectId;

                if (string.IsNullOrEmpty(project))
                    continue;

                if (!TryParseGuid(project, out projectId))
                    continue;

                projectId = new Guid(project);
                string prefix = "Project." + projectId.ToString("B").ToUpperInvariant();

                string path, url, file= null, enlistType=null;

                if (!ErrorHandler.Succeeded(mpb.Read(prefix + ".Path", out value, null, 0, null)))
                    continue;

                path = Unquote((string)value);

                if (ErrorHandler.Succeeded(mpb.Read(prefix + ".Project", out value, null, 0, null)))
                    file = Unquote((string)value);

                if (!ErrorHandler.Succeeded(mpb.Read(prefix + ".Uri", out value, null, 0, null)))
                    continue;

                url = Unquote((string)value);
                Uri uri;

                if (string.IsNullOrEmpty(url) || !Uri.TryCreate(url, UriKind.Relative, out uri))
                    continue;

                if (ErrorHandler.Succeeded(mpb.Read(prefix + ".EnlistType", out value, null, 0, null)))
                {
                    Guid enlistG;
                    enlistType = ((string)value).Trim();

                    if (!TryParseGuid(enlistType, out enlistG))
                        enlistType = null;
                }

                _enlistState.Add(new EnlistData(project, path, file, uri, enlistType));
            }
#endif
        }

        void PerformEnlist()
        {
            _enlistCompleted = true;
            _solutionDirectory = _solutionFile = null; // Clear cache

            if (_enlistState.Count == 0)
                return; // Nothing to do here

            IVsSolutionPersistence ps = GetService<IVsSolutionPersistence>(typeof(SVsSolutionPersistence));
            if (ps != null)
            {
                int hr = ps.LoadPackageUserOpts((IVsPersistSolutionOpts)GetService<Ankh.UI.IAnkhPackage>(), AnkhId.SubversionSccName + "Enlist");

            }

#if DEBUG_ENLISTMENT

            IVsSolution2 sol = GetService<IVsSolution2>(typeof(SVsSolution));
            // TODO: Load previous enlist state

            foreach (EnlistData item in _enlistState)
            {
                if (string.IsNullOrEmpty(item.EnlistType))
                    continue;

                Guid value;

                if(!TryParseGuid(item.EnlistType, out value))
                    continue;

                Guid[] factoryGuid = new Guid[] { value };

                IVsProjectFactory factory;
                if (!ErrorHandler.Succeeded(sol.GetProjectFactory(0, factoryGuid, null, out factory)) || factory == null)
                    continue;

                IVsSccProjectEnlistmentFactory enlistmentFactory = factory as IVsSccProjectEnlistmentFactory;

                if (enlistmentFactory == null)
                    continue;

                GC.KeepAlive(enlistmentFactory);
                string enlistLocal, enlistUnc;
                uint options;

                //SolutionFilePath

                string projectPath;

                if(item.Path.Contains("://"))
                    projectPath = item.Path;
                else
                    projectPath = Path.GetFullPath(Path.Combine(SolutionDirectory, item.Path));

                if(!ErrorHandler.Succeeded(enlistmentFactory.GetDefaultEnlistment(projectPath, out enlistLocal, out enlistUnc)))
                {
                    continue;
                }

                if (!ErrorHandler.Succeeded(enlistmentFactory.GetEnlistmentFactoryOptions(out options)))
                    options = (int)(__VSSCCENLISTMENTFACTORYOPTIONS.VSSCC_EFO_CANBROWSEENLISTMENTPATH | __VSSCCENLISTMENTFACTORYOPTIONS.VSSCC_EFO_CANEDITENLISTMENTPATH);

                EnlistmentState state = new EnlistmentState(
                    0 != (options & (int)(__VSSCCENLISTMENTFACTORYOPTIONS.VSSCC_EFO_CANBROWSEENLISTMENTPATH | __VSSCCENLISTMENTFACTORYOPTIONS.VSSCC_EFO_CANEDITENLISTMENTPATH)),
                    0 != (options & (int)(__VSSCCENLISTMENTFACTORYOPTIONS.VSSCC_EFO_CANEDITDEBUGGINGPATH| __VSSCCENLISTMENTFACTORYOPTIONS.VSSCC_EFO_CANEDITDEBUGGINGPATH)));

                state.Location = state.DebugLocation = enlistUnc;

                if(0 != (options & (int)__VSSCCENLISTMENTFACTORYOPTIONS.VSSCC_EFO_CANBROWSEENLISTMENTPATH))
                    state.BrowseLocation += delegate(object sender, EnlistmentState.EnlistmentEventArgs e)
                    {
                        string location, uncLocation;

                        if(ErrorHandler.Succeeded(enlistmentFactory.BrowseEnlistment(projectPath, e.State.Location, out location, out uncLocation)))
                        {
                            e.State.LocalLocation = location;
                            e.State.Location = uncLocation;
                        }
                    };

                if(0 != (options & (int)__VSSCCENLISTMENTFACTORYOPTIONS.VSSCC_EFO_CANBROWSEDEBUGGINGPATH))
                    state.BrowseDebugLocation += delegate(object sender, EnlistmentState.EnlistmentEventArgs e)
                    {
                        string location, uncLocation;

                        if(ErrorHandler.Succeeded(enlistmentFactory.BrowseEnlistment(projectPath, e.State.DebugLocation, out location, out uncLocation)))
                        {
                            e.State.LocalDebugLocation = location;
                            e.State.DebugLocation = uncLocation;
                        }
                    };

                IUIShell uiShell = GetService<IUIShell>();

                if(!uiShell.EditEnlistmentState(state))
                {
                    continue;
                }                


                GC.KeepAlive(enlistLocal);
            }
#endif
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

                    if (!_enlistStore.ContainsKey(projectId))
                    {   // Don't overwrite; we load 1 or 2 times during solution opening

                        EnlistBase enlist = new EnlistBase(projectId);

                        enlist.LoadUserData(values);
                        _enlistStore[projectId] = enlist;
                    }
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

                foreach (IVsHierarchy hier in GetAllProjectsInSolutionRaw())
                {
                    Guid g;
                    if (!ErrorHandler.Succeeded(sol.GetGuidOfProject(hier, out g)))
                        continue;

                    string projectId = g.ToString("B").ToUpperInvariant();

                    EnlistBase enlist;
                    if (_enlistStore.TryGetValue(projectId, out enlist) && enlist.ShouldSerialize())
                        list.Add(enlist);
                }

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

        private string Unquote(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            value = value.Trim();

            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Length >= 2 && value[0] == '\"' && value[value.Length - 1] == '\"')
            {
                value = value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
            }

            return value;
        }

        private bool TryParseGuid(string project, out Guid projectId)
        {
            projectId = Guid.Empty;

            if (string.IsNullOrEmpty(project))
                return false;

            project = project.Trim();

            if (project.Length != 38 || project[0] != '{' || project[37] != '}')
                return false;

            projectId = new Guid(project);
            return true;
        }

        static string RemoveEndSlash(string path, out char end)
        {
            end = '\0';
            if (SvnItem.IsValidPath(path))
            {
                string p = SvnTools.GetNormalizedFullPath(path);

                if (p.Length == path.Length - 1 && string.Equals(p, path.Substring(0, p.Length), StringComparison.OrdinalIgnoreCase))
                {
                    char c = path[path.Length - 1];

                    if (c == '\\' || c == '/')
                    {
                        end = c;
                        return p;
                    }
                }
            }

            return path;
        }

        static string AddEndSlash(string path, char end)
        {
            if (end != '\0')
                return path.TrimEnd(end) + end;
            else
                return path;
        }

        /// <summary>
        /// Translates a physical project path to a (possibly) virtual project path.
        /// </summary>
        /// <param name="lpszEnlistmentPath">[in] The physical path (either the local path or the enlistment UNC path) to be translated.</param>
        /// <param name="pbstrProjectPath">[out] The (possibly) virtual project path.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"/>. If it fails, it returns an error code.
        /// </returns>
        public int TranslateEnlistmentPathToProjectPath(string lpszEnlistmentPath, out string pbstrProjectPath)
        {
            if (!_enlistCompleted)
                PerformEnlist();

            char end;
            string path = RemoveEndSlash(lpszEnlistmentPath, out end);

            SccTranslateData td;
            if (_sccPaths.TryGetValue(lpszEnlistmentPath, out td) && !string.IsNullOrEmpty(td.StoredPathName))
            {
                if (SvnItem.IsValidPath(td.StoredPathName))
                    pbstrProjectPath = AddEndSlash(td.StoredPathName, end);
                else
                    pbstrProjectPath = td.StoredPathName;

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

            if (!_enlistCompleted)
                PerformEnlist();

            char end = '\0';
            string path;

            if (SvnItem.IsValidPath(lpszProjectPath))
            {
                path = RemoveEndSlash(lpszProjectPath, out end);
                path = SvnTools.GetNormalizedFullPath(lpszProjectPath);
            }
            else
                path = lpszProjectPath;

            SccTranslateData td;
            if (_storedPaths.TryGetValue(path, out td) && !string.IsNullOrEmpty(td.SccPathName))
            {
                pbstrEnlistmentPath = AddEndSlash(td.EnlistPathName ?? td.SccPathName, end);
                pbstrEnlistmentPathUNC = AddEndSlash(td.SccPathName, end);

                return VSConstants.S_OK;
            }

            pbstrEnlistmentPath = lpszProjectPath;
            pbstrEnlistmentPathUNC = lpszProjectPath;
            return VSConstants.S_OK;
        }


        /// <summary>
        /// Called by SccTranslateData.set_StoredPath
        /// </summary>
        internal void Translate_SetStoredPath(SccTranslateData translateData, string oldValue, string newValue)
        {
            if (translateData == null)
                throw new ArgumentNullException("translateData");
            else if (string.IsNullOrEmpty(newValue))
                throw new ArgumentNullException("newValue");

            if (string.Equals(oldValue, newValue, StringComparison.OrdinalIgnoreCase))
                return;

            SccTranslateData td;

            if (!string.IsNullOrEmpty(oldValue))
            {
                if (_storedPaths.TryGetValue(oldValue, out td))
                {
                    Debug.Assert(translateData == td, "Same translation");

                    _storedPaths.Remove(translateData.StoredPathName);
                }
            }

            if (!string.IsNullOrEmpty(newValue))
            {
                Debug.Assert(!_storedPaths.ContainsKey(newValue));

                _storedPaths[newValue] = translateData;
            }
        }

        internal SccTranslateData GetTranslateData(Guid projectId, SccEnlistMode createType, string solutionName)
        {
            if (createType > SccEnlistMode.None && string.IsNullOrEmpty(solutionName))
                throw new ArgumentNullException("solutionName");

            foreach (SccTranslateData td in _translations)
            {
                if (td.ProjectId == projectId)
                    return td;
            }
            if (createType > SccEnlistMode.None)
            {
                SccTranslateData td;
                if (createType <= SccEnlistMode.SvnStateOnly)
                    td = new SccTranslateData(this, projectId, solutionName);
                else
                    td = new SccTranslateEnlistData(this, projectId, solutionName);

                _translations.Add(td);

                return td;
            }

            return null;
        }

        /// <summary>
        /// Called by SccTranslateData.set_SccPath
        /// </summary>
        internal bool Translate_SetSccPath(SccTranslateData translateData, string value)
        {
            if (translateData == null)
                throw new ArgumentNullException("translateData");

            SccTranslateData td;

            if (!string.IsNullOrEmpty(translateData.SccPathName) &&
                !string.Equals(translateData.SccPathName, translateData.EnlistPathName, StringComparison.OrdinalIgnoreCase))
            {
                if (_sccPaths.TryGetValue(translateData.SccPathName, out td))
                {
                    Debug.Assert(translateData == td, "Same translation");

                    // Keep the value in the list if the EnlistPathName still has this value
                    _storedPaths.Remove(translateData.StoredPathName);
                }
            }

            if (!string.IsNullOrEmpty(value))
            {
                if (!_storedPaths.TryGetValue(value, out td))
                    _storedPaths.Add(value, translateData);
                else if (td != translateData)
                    throw new InvalidOperationException("Can't store a path twice");
            }

            return true;
        }

        #region IVsAsynchOpenFromScc Members

        public int IsLoadingContent(IVsHierarchy pHierarchy, out int pfIsLoading)
        {
            pfIsLoading = 0; // The project is available
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This method begins loading the specified project asynchronously.
        /// </summary>
        /// <param name="lpszProjectPath">[in] Physical path to the project to be loaded from source control.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"/>. If it fails, it returns an error code.
        /// </returns>
        public int LoadProject(string lpszProjectPath)
        {
            return VSConstants.S_OK; // The project is available
        }

        /// <summary>
        /// This method determines whether a specified project must be loaded asynchronously.
        /// </summary>
        /// <param name="lpszProjectPath">[in] Physical path to the specified project.</param>
        /// <param name="pReturnValue">[out] Returns nonzero (true) if the project must be loaded asynchronously. Otherwise, returns zero (false) if the project can be loaded synchronously.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"/>. If it fails, it returns an error code.
        /// </returns>
        public int LoadProjectAsynchronously(string lpszProjectPath, out int pReturnValue)
        {
            pReturnValue = 0; // Project shouldn't be loaded asynchronous
            return VSConstants.S_OK;
        }

        #endregion
    }
}
