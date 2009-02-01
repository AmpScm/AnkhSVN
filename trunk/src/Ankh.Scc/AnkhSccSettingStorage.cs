using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ankh.Scc.Native;
using Ankh.Scc.SettingMap;
using Ankh.UI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.Text;

namespace Ankh.Scc
{
    /// <summary>
    /// This class is responsible for mapping the settings stored in the solution file
    /// to the real values used by the project system. It implements IVsSccEnlistmentPathTranslation
    /// for the SccProvider
    /// </summary>
    [GlobalService(typeof(ISccSettingsStore))]
    sealed partial class AnkhSccSettingStorage : AnkhService, ISccSettingsStore, IVsSccEnlistmentPathTranslation
    {
        /// <summary>
        /// Properties stored in the solution
        /// </summary>
        readonly SortedList<string, string> _projectProps = new SortedList<string, string>();
        /// <summary>
        /// Containers stored in the solution
        /// </summary>
        readonly SortedList<string, string> _categoryTypes = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly SortedList<string, string> _categoryProps = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);


        bool _loaded;
        bool _solutionRead;
        bool _solutionPropsBefore;
        bool _solutionPropsDirty;

        public AnkhSccSettingStorage(IAnkhServiceProvider context)
            : base(context)
        {
        }

        internal void OnSolutionClosed()
        {
            Clear();
        }

        private void Clear()
        {
            _loaded = false;
            _solutionRead = false;
            // Maps
            _solutionToProject.Clear();
            _actualToProject.Clear();
            _categories.Clear();
            // Used properties
            _projectProps.Clear();
            _categoryProps.Clear();
            _categoryTypes.Clear();
        }

        void EnsureLoaded()
        {
            if (_loaded)
                return;

            foreach (string s in GetAllProjects())
            {
                GC.KeepAlive(s);
            }

            _loaded = true;
            //GetService<IAnkhPackage>().LoadSolutionProperties(true);
        }

        #region IVsSccEnlistmentPathTranslation Members

        public int TranslateEnlistmentPathToProjectPath(string lpszEnlistmentPath, out string pbstrProjectPath)
        {
            EnsureLoaded();

            SccProjectSettings st;
            if (!_actualToProject.TryGetValue(lpszEnlistmentPath, out st))
            {
                pbstrProjectPath = lpszEnlistmentPath;
                return VSConstants.S_OK;
            }

            pbstrProjectPath = st.SolutionProjectReference;
            return VSConstants.S_OK;
        }

        public int TranslateProjectPathToEnlistmentPath(string lpszProjectPath, out string pbstrEnlistmentPath, out string pbstrEnlistmentPathUNC)
        {
            EnsureLoaded();

            SccProjectSettings st;

            if (!_solutionToProject.TryGetValue(lpszProjectPath, out st))
            {
                pbstrEnlistmentPath = pbstrEnlistmentPathUNC = lpszProjectPath;
                return VSConstants.S_OK;
            }

            pbstrEnlistmentPath = st.ActualProjectReference;
            pbstrEnlistmentPathUNC = st.ActualProjectReference;
            return VSConstants.S_OK;
        }

        #endregion

        #region ISccSettingsStore Members

        /// <summary>
        /// Gets a boolean indicating whether te solution should be saved for changed scc settings
        /// </summary>
        /// <value></value>
        public bool IsSolutionDirty
        {
            get { return _solutionPropsDirty || (!HasSolutionData && _solutionPropsBefore); }
        }

        public bool HasSolutionData
        {
            get { return true; }
        }

        public IPropertyMap GetMap(Microsoft.VisualStudio.OLE.Interop.IPropertyBag propertyBag)
        {
            return new PropertyBag(propertyBag);
        }

        IEnumerable<string> GetAllProjects()
        {
            IVsSolution2 sol = GetService<IVsSolution2>(typeof(SVsSolution));
            string[] projects;
            uint count;
            if (!ErrorHandler.Succeeded(sol.GetProjectFilesInSolution(0, 0, null, out count)))
                yield break;

            projects = new string[count];

            if (!ErrorHandler.Succeeded(sol.GetProjectFilesInSolution(0, count, projects, out count)))
                yield break;

            string dir;
            string file;
            string userfile;
            if (!ErrorHandler.Succeeded(sol.GetSolutionInfo(out dir, out file, out userfile)))
                yield break;

            foreach (string project in projects)
            {
                if (string.IsNullOrEmpty(project))
                    continue;
                else if (!SvnItem.IsValidPath(project))
                {
                    yield return project;
                    continue;
                }

                yield return PackageUtilities.MakeRelative(file, project);
            }
        }

        public void WriteSolutionProperties(IPropertyMap map)
        {
            map.SetValue("_v", "1.0");
            if (_categoryTypes.Count > 0)
                map.SetValue("PrjCats", string.Join(",", ToArray(_categoryTypes.Values)));
            if (_categoryProps.Count > 0)
                map.SetValue("PrjCatProps", string.Join(",", ToArray(_categoryProps.Values)));
            if (_projectProps.Count > 0)
                map.SetValue("PrjProps", string.Join(",", ToArray(_projectProps.Values)));

            string dir;
            string file;
            string userfile;
            IVsSolution2 sol = GetService<IVsSolution2>(typeof(SVsSolution));
            if (!ErrorHandler.Succeeded(sol.GetSolutionInfo(out dir, out file, out userfile)))
                return;

            foreach (string project in GetAllProjects())
            {
                SccProjectSettings set;
                if (!_actualToProject.TryGetValue(project, out set))
                    continue;

                if (!set.ShouldPersist)
                    continue;

                string name;
                if (SvnItem.IsValidPath(project))
                    name = PackageUtilities.MakeRelative(file, project);
                else
                    name = project;

                map.SetValue("Project:\"" + KeyEscape(name) + "\".Id", set.ProjectId);
            }
            map.Flush();
        }

        public void ReadSolutionProperties(IPropertyMap map)
        {
            _solutionPropsBefore = true;
            string value;

            // Do this even when reloading
            if (map.TryGetValue("PrjCats", out value))
                LoadPropNames(_categoryTypes, value);
            if (map.TryGetValue("PrjCatProps", out value))
                LoadPropNames(_categoryProps, value);
            if (map.TryGetValue("PrjProps", out value))
                LoadPropNames(_projectProps, value);

            if (_solutionRead)
                return;

            string dir;
            string file;
            string userfile;
            IVsSolution2 sol = GetService<IVsSolution2>(typeof(SVsSolution));
            if (!ErrorHandler.Succeeded(sol.GetSolutionInfo(out dir, out file, out userfile)))
                return;

            foreach (string project in GetAllProjects())
            {
                string name;
                if (SvnItem.IsValidPath(project))
                    name = PackageUtilities.MakeRelative(file, project);
                else
                    name = project;

                string id;

                if (!map.TryGetValue("Project:\"" + KeyEscape(name) + "\".Id", out id))
                    continue;

                Guid guid;

                if (!TryParseGuid(id, out guid))
                    continue;

                // Automatically hooks settings
                SccProjectSettings prj = new SccProjectSettings(this, project, id);

                LoadProjectData(prj);
            }
        }

        private void LoadProjectData(SccProjectSettings prj)
        {
            foreach (string prop in _projectProps.Values)
            {

            }
        }

        internal void OnProjectRenamed(string oldLocation, string newLocation)
        {

        }

        private void LoadPropNames(SortedList<string, string> list, string value)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            else if (value == null)
                throw new ArgumentNullException("value");

            foreach (string ix in value.Split(','))
            {
                string i = ix.Trim();

                if (!string.IsNullOrEmpty(i))
                    list[i] = i;
            }
        }

        #endregion

        static string ToString(Guid value)
        {
            return value.ToString("B").ToUpperInvariant();
        }

        static T[] ToArray<T>(ICollection<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            T[] array = new T[list.Count];

            list.CopyTo(array, 0);
            return array;
        }

        static string KeyEscape(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                if ((char.IsLetterOrDigit(value, i) || ("_!()/\\".IndexOf(value[i]) >= 0)) && (int)value[i] < 128)
                    sb.Append(value[i]);
                else if ((int)value[i] < 128)
                    sb.AppendFormat("%{0:x2}", value[i]);
                else
                    sb.Append(Uri.EscapeDataString(value[i].ToString()));
            }

            return sb.ToString();
        }

        static bool TryParseGuid(string project, out Guid projectId)
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
    }
}
