using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc.SettingMap;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Ankh.Scc
{
    [GlobalService(typeof(ISccStoreMap))]
    partial class AnkhSccSettingStorage : ISccStoreMap
    {
        readonly Dictionary<string, SccProjectSettings> _solutionToProject = new Dictionary<string, SccProjectSettings>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, SccProjectSettings> _actualToProject = new Dictionary<string, SccProjectSettings>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, SccCategorySettings> _categories = new Dictionary<string, SccCategorySettings>(StringComparer.OrdinalIgnoreCase);

        #region ISccStoreMap Members

        public string GetProjectProperty(string project, string key)
        {
            if (string.IsNullOrEmpty(project))
                throw new ArgumentNullException("project");
            else if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (!SccProjectProps.All.Contains(key))
                throw new ArgumentOutOfRangeException("key");

            SccProjectSettings ps;
            if (!_actualToProject.TryGetValue(project, out ps))
                return null;

            string value;
            if (ps.Properties.TryGetValue(key, out value))
                return value;
            else
                return null;
        }

        public void SetProjectProperty(string project, string key, string value)
        {
            if (string.IsNullOrEmpty(project))
                throw new ArgumentNullException("project");
            else if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (!SccProjectProps.All.Contains(key))
                throw new ArgumentOutOfRangeException("key");

            SccProjectSettings ps;
            if (!TryGetProject(project, (value != null), out ps))
                throw new ArgumentOutOfRangeException("project");

            if (ps == null)
                return; // Project exists but no need to create property

            if (!_projectProps.ContainsKey(key))
            {
                if (value == null)
                    return; // No change

                // Add all at once to minimize number of solution changes
                foreach (string k in SccProjectProps.All)
                {
                    _projectProps[k] = k;
                }
            }

            key = _projectProps[key];
            string oldValue;

            if (!ps.Properties.TryGetValue(key, out oldValue))
                oldValue = null;

            if (oldValue != value)
            {
                _solutionPropsDirty = true;

                if (value != null)
                    ps.Properties[key] = value;
                else
                    ps.Properties.Remove(key);
            }
        }

        public string GetCategoryProperty(string category, string key)
        {
            if (string.IsNullOrEmpty(category))
                throw new ArgumentNullException("category");
            else if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (!SccCategoryProps.All.Contains(key))
                throw new ArgumentOutOfRangeException("key");

            SccCategorySettings cat;
            if (!_categories.TryGetValue(category, out cat))
                throw new ArgumentOutOfRangeException("category");

            string value;
            if (cat.Properties.TryGetValue(key, out value))
                return value;
            else
                return null;
        }

        public void SetCategoryProperty(string categoryId, string key, string value)
        {
            if (string.IsNullOrEmpty(categoryId))
                throw new ArgumentNullException("categoryId");
            else if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (!SccCategoryProps.All.Contains(key))
                throw new ArgumentOutOfRangeException("key");

            SccCategorySettings cat;
            if (!_categories.TryGetValue(categoryId, out cat))
                throw new ArgumentOutOfRangeException("categoryId");

            if (!_categoryProps.ContainsKey(key))
            {
                if (value == null)
                    return; // No change

                // Add all at once to minimize number of solution changes
                foreach (string k in SccCategoryProps.All)
                {
                    _categoryProps[k] = k;
                }
            }

            key = _categoryProps[key];
            string oldValue;

            if (!cat.Properties.TryGetValue(key, out oldValue))
                oldValue = null;

            if (oldValue != value)
            {
                _solutionPropsDirty = true;

                if (value != null)
                    cat.Properties[key] = value;
                else
                    cat.Properties.Remove(key);
            }
        }

        public string GetProjectCategory(string project, string category)
        {
            if (string.IsNullOrEmpty(project))
                throw new ArgumentNullException("project");
            else if (string.IsNullOrEmpty(category))
                throw new ArgumentNullException("category");

            if (!SccCategories.All.Contains(category))
                throw new ArgumentOutOfRangeException("category");

            SccProjectSettings ps;

            if (!TryGetProject(project, false, out ps))
                throw new ArgumentOutOfRangeException("project");

            if (ps == null)
                return null; // Project exists but has no data

            if (!_actualToProject.TryGetValue(project, out ps))
                return null;

            string value;
            if (!ps.Categories.TryGetValue(category, out value))
                return null;

            if (_categories.ContainsKey(value))
                return value;
            else
                return null; // Don't return unavailable categories
        }

        public void SetProjectCategory(string project, string category, string categoryId)
        {
            if (string.IsNullOrEmpty(project))
                throw new ArgumentNullException("project");
            else if (string.IsNullOrEmpty(category))
                throw new ArgumentNullException("category");
            else if (string.IsNullOrEmpty(categoryId))
                throw new ArgumentNullException("categoryId");

            if (!SccCategories.All.Contains(category))
                throw new ArgumentOutOfRangeException("category");
            else if (categoryId != null && !_categories.ContainsKey(categoryId))
                throw new ArgumentOutOfRangeException("categoryId");

            SccProjectSettings ps;

            if (!TryGetProject(project, (categoryId != null), out ps))
                throw new ArgumentOutOfRangeException("project");

            if (ps == null)
                return; // No instance data, nothing to clear

            if (!_categoryTypes.ContainsKey(category))
            {
                if (categoryId == null)
                    return;

                // Add all at once to minimize number of solution changes
                foreach (string k in SccCategories.All)
                {
                    _categoryTypes[k] = k;
                }
            }

            category = _categoryTypes[category];

            string oldValue;
            if (!ps.Categories.TryGetValue(category, out oldValue))
                oldValue = null;

            if (oldValue != categoryId)
            {
                _solutionPropsDirty = true;

                if (categoryId != null)
                    ps.Categories[category] = categoryId;
                else
                    ps.Categories.Remove(category);
            }
        }

        #region ISccStoreMap Members

        public string CreateCategory()
        {
            string id = ToString(Guid.NewGuid());

            return (new SccCategorySettings(this, id)).CategoryId;
        }

        #endregion

        #endregion

        internal void UpdateSolutionReference(SccProjectSettings item, string newName)
        {
            if (item != null && item.SolutionProjectReference != null)
                _solutionToProject.Remove(item.SolutionProjectReference);

            _solutionToProject[newName] = item;
        }

        internal void UpdateActualReference(SccProjectSettings item, string newName)
        {
            if (item != null && item.SolutionProjectReference != null)
                _actualToProject.Remove(item.SolutionProjectReference);

            _actualToProject[newName] = item;
        }

        internal void AddCategory(SccCategorySettings category)
        {
            _categories[category.CategoryId] = category;
        }

        #region Project initialization
        IEnumerable<IVsHierarchy> GetAllProjectsInSolutionRaw()
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

        private bool TryGetProject(string project, bool create, out SccProjectSettings settings)
        {
            if (_actualToProject.TryGetValue(project, out settings))
                return true;

            string id = GetProjectId(ref project);

            settings = null;
            if (id == null)
                return false;

            if (create)
                settings = new SccProjectSettings(this, project, id);

            return true;
        }

        private string GetProjectId(ref string project)
        {
            IVsSolution2 sol = GetService<IVsSolution2>(typeof(SVsSolution));

            IVsHierarchy hierFound = null;
            foreach (IVsHierarchy hier in GetAllProjectsInSolutionRaw())
            {
                object nameOb;
                if (ErrorHandler.Succeeded(hier.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_Name, out nameOb)))
                {

                    string name = nameOb as string;

                    if (string.Equals(name, project, StringComparison.OrdinalIgnoreCase))
                    {
                        hierFound = hier;
                        project = name; // Fix casing
                        break;
                    }
                }
            }

            if (hierFound == null)
                return null;

            IVsSolution solution = GetService<IVsSolution>(typeof(SVsSolution));

            Guid value;
            if (ErrorHandler.Succeeded(solution.GetGuidOfProject(hierFound, out value)))
                return ToString(value);

            return null;
        }
        #endregion
    }
}
