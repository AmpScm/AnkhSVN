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

        const string KeyProjectProperties = "Defs.Properties";
        const string KeyProjectCategories = "Defs.Categories";
        const string KeyCategoryProperties = "Defs.CatProprts";

        public void WriteSolutionProperties(IPropertyMap map)
        {
            if (!HasSolutionData)
                return;

            if (_categoryTypes.Count > 0)
                map.SetValue(KeyProjectCategories, string.Join(",", ToArray(_categoryTypes.Values)));
            if (_categoryProps.Count > 0)
                map.SetValue(KeyCategoryProperties, string.Join(",", ToArray(_categoryProps.Values)));
            if (_projectProps.Count > 0)
                map.SetValue(KeyProjectProperties, string.Join(",", ToArray(_projectProps.Values)));

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
            HybridCollection<string> catsToWrite = new HybridCollection<string>();
            foreach (string project in GetAllProjects())
            {
                SccProjectSettings ps;
                if (!_actualToProject.TryGetValue(project, out ps))
                    continue;

                if (!ps.ShouldPersist)
                    continue;

                string name;
                if (SvnItem.IsValidPath(project))
                    name = PackageUtilities.MakeRelative(file, project);
                else
                    name = project;

                map.SetQuoted(string.Format("Project.{0}", ps.ProjectId), name);

                foreach (KeyValuePair<string, string> kv in ps.Properties)
                {
                    string k;
                    if (!_projectProps.TryGetValue(kv.Key, out k))
                        continue;

                    string key = string.Format("Project.{0}.{1}", ps.ProjectId, k);

                    if (kv.Value != null)
                        map.SetQuoted(key, kv.Value);
                }

                foreach (KeyValuePair<string, string> kv in ps.Categories)
                {
                    string k;
                    if (!_categoryTypes.TryGetValue(kv.Key, out k))
                        continue;

                    string key = string.Format("Project.{0}:{1}", ps.ProjectId, k);

                    if (kv.Value != null)
                    {
                        map.SetValue(key, kv.Value); // No need to quote a guid

                        if (!catsToWrite.Contains(kv.Value))
                            catsToWrite.Add(kv.Value);
                    }
                }
            }
            map.Flush();
            foreach (string cat in catsToWrite)
            {
                SccCategorySettings ct;
                if (!_categories.TryGetValue(cat, out ct))
                    continue;

                if (!ct.ShouldPersist)
                    continue;

                if (!string.IsNullOrEmpty(ct.Name))
                {
                    map.SetQuoted(string.Format("Category.{0}", ct.CategoryId), ct.Name);
                }

                foreach (KeyValuePair<string, string> kv in ct.Properties)
                {
                    string k;
                    if (!_categoryProps.TryGetValue(kv.Key, out k))
                        continue;

                    string key = string.Format("Category.{0}.{1}", ct.CategoryId, k);

                    if (kv.Value != null)
                    {
                        map.SetQuoted(key, kv.Value);
                    }
                }
            }
        }

        public void ReadSolutionProperties(IPropertyMap map)
        {
            _solutionPropsBefore = true;
            string value;

            // Do this even when reloading
            if (map.TryGetValue(KeyProjectCategories, out value))
                LoadPropNames(_categoryTypes, value);
            if (map.TryGetValue(KeyCategoryProperties, out value))
                LoadPropNames(_categoryProps, value);
            if (map.TryGetValue(KeyProjectProperties, out value))
                LoadPropNames(_projectProps, value);

            if (_solutionRead)
                return;

            string dir;
            string file;
            string userfile;
            IVsSolution2 sol = GetService<IVsSolution2>(typeof(SVsSolution));
            if (!ErrorHandler.Succeeded(sol.GetSolutionInfo(out dir, out file, out userfile)))
                return;

            List<string> projects = new List<string>(GetAllProjects());

            foreach (string project in projects)
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

                LoadProjectData(prj, map);
            }

            foreach (SccCategorySettings cat in _categories.Values)
            {
                string key = string.Format("Category.{0}", cat.CategoryId);                

                if (map.TryGetQuoted(key, out value))
                    cat.Name = value;

                foreach (string k in _categoryProps.Values)
                {
                    key = string.Format("Category.{0}.{1}", cat.CategoryId, k);

                    if (!map.TryGetQuoted(key, out value))
                        continue;

                    cat.Properties[k] = value;
                }
            }
        }

        private void LoadProjectData(SccProjectSettings prj, IPropertyMap map)
        {
            foreach (string i in _projectProps.Values)
            {
                string key = string.Format("Project.{0}.{1}", prj.ProjectId, i);

                string value;
                if (!map.TryGetQuoted(key, out value))
                    continue;

                prj.Properties[key] = value;
            }

            foreach (string i in _categoryTypes.Values)
            {
                string key = string.Format("Project.{0}:{1}", prj.ProjectId, i);

                string value;
                if (!map.TryGetQuoted(key, out value))
                    continue;

                Guid gv;
                if(!TryParseGuid(value, out gv))
                    continue;

                prj.Categories[key] = value;

                if (!_categories.ContainsKey(value))
                {
                    SccCategorySettings st = new SccCategorySettings(this, value);

                    // Automatically added to category list
                }
            }
        }

        internal void OnProjectRenamed(string oldLocation, string newLocation)
        {
            SccProjectSettings sps;
            if (_actualToProject.TryGetValue(oldLocation, out sps))
            {
                sps.ActualProjectReference = newLocation;
                sps.SolutionProjectReference = newLocation;
            }
        }

        private void LoadPropNames(SortedList<string, string> list, string value)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            else if (value == null)
                throw new ArgumentNullException("value");

            foreach (string ix in value.Split(','))
            {
                string name = ix.Trim();

                if(string.IsNullOrEmpty(name))
                    continue;

                bool skip = false;
                for (int i = 0; i < name.Length; i++)
                {
                    if (!char.IsLetterOrDigit(name, i) && 0 > "_-".IndexOf(name[i]))
                    {
                        skip = true;
                        break; // Ignore unsafe names
                    }
                }

                if (skip)
                    continue;

                list[name] = name;
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
