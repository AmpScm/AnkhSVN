using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc.SettingMap
{
    class SccProjectSettings
    {
        readonly SortedList<string, string> _props = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly SortedList<string, string> _cats = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly AnkhSccSettingStorage _store;
        string _projectId;

        public SccProjectSettings(AnkhSccSettingStorage store, string slnProjectName, string id)
        {
            if (store == null)
                throw new ArgumentNullException("store");
            else if (string.IsNullOrEmpty(slnProjectName))
                throw new ArgumentNullException("slnProjectName");
            else if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            _store = store;
            _projectId = id;

            SolutionProjectReference = slnProjectName;
            ActualProjectReference = slnProjectName; // Good default
        }
        
        string _slnReference;
        public string SolutionProjectReference
        {
            get { return _slnReference; }
            set
            {
                if (_slnReference != null && value != _slnReference)
                {
                    _store.UpdateSolutionReference(this, value);
                    _slnReference = value;
                }
            }
        }

        string _actualReference;
        public string ActualProjectReference
        {
            get { return _actualReference; }
            set
            {
                if (_actualReference != null && value != _actualReference)
                {
                    _store.UpdateActualReference(this, value);
                    _actualReference = value;
                }
            }
        }

        public IDictionary<string, string> Properties
        {
            get { return _props; }
        }

        public IDictionary<string,string> Categories
        {
            get { return _cats; }
        }

        public string ProjectId
        {
            get { return _projectId; }
        }

        public bool ShouldPersist
        {
            get { return !string.IsNullOrEmpty(ProjectId) && true; /* (_props.Count > 0 || _cats.Count > 0); */}
        }
    }
}
