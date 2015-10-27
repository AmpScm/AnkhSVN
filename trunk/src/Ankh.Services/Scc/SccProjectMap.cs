using System;
using System.Collections;
using System.Collections.Generic;
using Ankh.Scc.ProjectMap;
using Ankh.Selection;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;

namespace Ankh.Scc
{
    [CLSCompliant(false)]
    public abstract partial class SccProjectMap : AnkhService
    {
        readonly Dictionary<IVsSccProject2, SccProjectData> _projectMap = new Dictionary<IVsSccProject2, SccProjectData>();

        protected SccProjectMap(IAnkhServiceProvider context)
            : base(context)
        {

        }

        [CLSCompliant(false)]
        public bool TryGetSccProject(IVsSccProject2 project, out SccProjectData data)
        {
            return _projectMap.TryGetValue(project, out data);
        }

        public IEnumerable<SccProjectData> AllSccProjects
        {
            get { return _projectMap.Values; }
        }

        public void Clear()
        {
            _projectMap.Clear();
            _fileMap.Clear();
            _sccExcluded.Clear();
        }

        public bool ContainsProject(IVsSccProject2 project)
        {
            return _projectMap.ContainsKey(project);
        }

        void AddProject(IVsSccProject2 sccProject, SccProjectData sccProjectData)
        {
            _projectMap.Add(sccProject, sccProjectData);
        }

        public int ProjectCount
        {
            get { return _projectMap.Count; }
        }

        public bool RemoveProject(IVsSccProject2 project)
        {
            return _projectMap.Remove(project);
        }

        public void EnsureSccProject(IVsSccProject2 rawProject, out SccProjectData projectData)
        {
            if (!TryGetSccProject(rawProject, out projectData))
            {
                // This method is called before the OpenProject calls
                AddProject(rawProject, projectData = new SccProjectData(this, rawProject));
            }
        }

        internal protected abstract Selection.SccProject CreateProject(SccProjectData sccProjectData);

        internal SccProjectFile GetFile(string path)
        {
            SccProjectFile projectFile;

            if (!TryGetFile(path, out projectFile))
            {
                AddFile(path, projectFile = new SccProjectFile(this, path));

                AddedToSolution(path);
            }

            return projectFile;
        }

        protected virtual void AddedToSolution(string path)
        {
        }

        protected virtual void OnRemovedFile(string fileName)
        {
        }

        internal protected abstract int GetSccGlyph(string[] namesArray, VsStateIcon[] newGlyphs, uint[] sccState);

        public IEnumerable<Selection.SccProject> GetAllProjectsContaining(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            path = SvnTools.GetNormalizedFullPath(path);

            SccProjectFile file;
            if (TryGetFile(path, out file))
            {
                foreach (SccProjectData pd in file.GetOwnerProjects())
                {
                    yield return pd.SvnProject;
                }
            }

            if (string.Equals(path, SolutionFilename, StringComparison.OrdinalIgnoreCase))
                yield return SccProject.Solution;
        }

        public IEnumerable<SccProject> GetAllProjectsContaining(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            Hashtable projects = new Hashtable();
            foreach (string path in paths)
            {
                string nPath = SvnTools.GetNormalizedFullPath(path);

                SccProjectFile file;
                if (TryGetFile(nPath, out file))
                {
                    foreach (SccProjectData pd in file.GetOwnerProjects())
                    {
                        if (projects.Contains(pd))
                            continue;

                        projects.Add(pd, pd);

                        yield return pd.SvnProject;
                    }
                }

                if (!projects.Contains(SccProject.Solution)
                    && string.Equals(path, SolutionFilename, StringComparison.OrdinalIgnoreCase))
                {
                    projects.Add(SccProject.Solution, SccProject.Solution);
                    yield return SccProject.Solution;
                }
            }
        }

        public IEnumerable<SccProject> GetAllSccProjects()
        {
            foreach (SccProjectData pd in AllSccProjects)
            {
                if (!pd.ExcludedFromScc)
                    yield return pd.SvnProject;
            }
        }

        public IEnumerable<SccProject> GetAllUIProjects()
        {
            foreach (SccProjectData pd in AllSccProjects)
            {
                if (!pd.DontAddToProjectWindow)
                    yield return pd.SvnProject;
            }
        }

        public SccProject ResolveRawProject(SccProject project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            if (project.RawHandle == null && !project.IsSolution)
            {
                SccProjectFile file;

                if (TryGetFile(project.FullPath, out file))
                {
                    foreach (SccProjectData p in file.GetOwnerProjects())
                    {
                        return p.SvnProject;
                    }
                }
            }

            return project;
        }

        public IEnumerable<string> GetAllFilesOf(SccProject project, bool exceptExcluded)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            if (project.IsSolution)
            {
                string sf = SolutionFilename;

                if (sf != null && (!exceptExcluded || !IsSccExcluded(SolutionFilename)))
                    yield return sf;

                yield break;
            }

            project = ResolveRawProject(project);

            IVsSccProject2 scc = project.RawHandle;
            SccProjectData data;

            if (scc == null || !TryGetSccProject(scc, out data))
                yield break;

            foreach (string file in data.GetAllFiles())
            {
                if (file[file.Length - 1] != '\\') // Don't return paths
                {
                    if (exceptExcluded && IsSccExcluded(file))
                        continue;

                    yield return file;
                }
            }
        }

        public IEnumerable<string> GetAllFilesOf(ICollection<SccProject> projects, bool exceptExcluded)
        {
            SortedList<string, string> files = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
            Hashtable handled = new Hashtable();
            foreach (SccProject p in projects)
            {
                SccProject project = ResolveRawProject(p);

                IVsSccProject2 scc = project.RawHandle;
                SccProjectData data;

                if (scc == null || !TryGetSccProject(scc, out data))
                {
                    if (p.IsSolution && SolutionFilename != null && !files.ContainsKey(SolutionFilename))
                    {
                        files.Add(SolutionFilename, SolutionFilename);

                        if (exceptExcluded && IsSccExcluded(SolutionFilename))
                            continue;

                        yield return SolutionFilename;
                    }

                    continue;
                }

                if (handled.Contains(data))
                    continue;

                handled.Add(data, data);

                foreach (string file in data.GetAllFiles())
                {
                    if (file[file.Length - 1] == '\\') // Don't return paths
                        continue;

                    if (files.ContainsKey(file))
                        continue;

                    files.Add(file, file);

                    if (exceptExcluded && IsSccExcluded(file))
                        continue;

                    yield return file;
                }
            }
        }

        public ICollection<string> GetAllFilesOfAllProjects(bool exceptExcluded)
        {
            List<string> files = new List<string>(UniqueFileCount + 1);

            if (SolutionFilename != null && !ContainsFile(SolutionFilename))
                files.Add(SolutionFilename);

            foreach (string file in AllFiles)
            {
                if (file[file.Length - 1] == '\\') // Don't return paths
                    continue;

                if (exceptExcluded && IsSccExcluded(file))
                    continue;

                files.Add(file);
            }

            return files.AsReadOnly();
        }

        public ISccProjectInfo GetProjectInfo(SccProject project)
        {
            if (project == null)
                return null;

            project = ResolveRawProject(project);

            if (project == null || project.RawHandle == null)
                return null;

            SccProjectData pd;
            if (TryGetSccProject(project.RawHandle, out pd))
            {
                return new WrapProjectInfo(pd);
            }

            return null;
        }


        /// <summary>
        /// Wrapper class providing a public api to the data contained within <see cref="SccProjectData"/>
        /// </summary>
        /// <remarks>Showing the raw properties of SccProjectData has side-effects. We wrap the class to hide this problem</remarks>
        sealed class WrapProjectInfo : ISccProjectInfo
        {
            readonly SccProjectData _data;

            /// <summary>
            /// Initializes a new instance of the <see cref="WrapProjectInfo"/> class.
            /// </summary>
            /// <param name="data">The data.</param>
            public WrapProjectInfo(SccProjectData data)
            {
                if (data == null)
                    throw new ArgumentNullException("data");

                _data = data;
            }

            /// <summary>
            /// Gets the name of the project.
            /// </summary>
            /// <value>The name of the project.</value>
            public string ProjectName
            {
                get { return _data.ProjectName; }
            }

            /// <summary>
            /// Gets the project directory.
            /// </summary>
            /// <value>The project directory.</value>
            public string ProjectDirectory
            {
                get { return _data.ProjectDirectory; }
            }

            #region ISvnProjectInfo Members


            /// <summary>
            /// Gets the project file.
            /// </summary>
            /// <value>The project file.</value>
            public string ProjectFile
            {
                get { return _data.ProjectFile; }
            }

            /// <summary>
            /// Gets the full name of the project (the project prefixed by the folder it is under)
            /// </summary>
            /// <value>The full name of the project.</value>
            public string UniqueProjectName
            {
                get { return _data.UniqueProjectName; }
            }

            /// <summary>
            /// Gets the SCC base directory.
            /// </summary>
            /// <value>The SCC base directory.</value>
            public string SccBaseDirectory
            {
                get { return _data.SccBaseDirectory; }
                set { throw new InvalidOperationException(); }
            }

            /// <summary>
            /// Gets or sets the SCC base URI.
            /// </summary>
            /// <value>The SCC base URI.</value>
            public Uri SccBaseUri
            {
                get { return null; }
                set { throw new InvalidOperationException(); }
            }

            public bool IsSccBindable
            {
                get { return _data.IsSccBindable; }

            }

            #endregion
        }

        public ProjectIconReference GetPathIconHandle(string path)
        {
            SccProjectFile file;

            if (!TryGetFile(path, out file))
            {
                if (string.Equals(path, SolutionFilename, StringComparison.OrdinalIgnoreCase))
                {
                    // TODO: Fetch real solution icon
                    return null;
                }

                return null;
            }

            foreach (SccProjectFileReference fr in file.GetAllReferences())
            {
                ProjectIconReference icon;
                if (fr.TryGetIcon(out icon))
                    return icon;
            }

            return null;
        }

        public bool IsProjectFileOrSolution(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (string.Equals(path, SolutionFilename, StringComparison.OrdinalIgnoreCase))
                return true; // A solution file can be part of a project

            SccProjectFile file;
            if (!TryGetFile(path, out file))
                return false;

            return file.IsProjectFile;
        }

        public bool IgnoreEnumerationSideEffects(IVsSccProject2 sccProject)
        {
            SccProjectData projectData;
            if (TryGetSccProject(sccProject, out projectData))
            {
                // We have to know its contents to provide SCC info
                // TODO: BH: Maybe only enable while reloading?
                return projectData.WebLikeFileHandling;
            }

            return false;
        }
    }
}
