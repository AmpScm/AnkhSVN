using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS.Selection
{
    sealed class SelectionItem : IEquatable<SelectionItem>
    {
        readonly IVsHierarchy _hierarchy;
        IVsSccProject2 _sccProject;
        readonly uint _id;
        bool _checkedSccProject;

        public SelectionItem(IVsHierarchy hierarchy, uint id)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            _hierarchy = hierarchy;
            _id = id;
        }

        public SelectionItem(IVsHierarchy hierarchy, uint id, IVsSccProject2 project)
            : this(hierarchy, id)
        {
            _sccProject = project;
        }

        public IVsHierarchy Hierarchy
        {
            [DebuggerStepThrough]
            get { return _hierarchy; }
        }

        public IVsSccProject2 SccProject
        {
            get
            {
                if (_sccProject == null && !_checkedSccProject)
                {
                    _checkedSccProject = true;
                    _sccProject = _hierarchy as IVsSccProject2;
                }

                return _sccProject;
            }
        }

        public uint Id
        {
            get { return _id; }
        }

        public bool IsSolution
        {
            get { return (SccProject != null) && SelectionUtils.IsSolutionSccProject(SccProject); }
        }

        #region IEquatable<SelectionItem> Members

        public bool Equals(SelectionItem other)
        {
            if (other == null)
                return false;

            return (other.Id == Id) && (other.Hierarchy == Hierarchy);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SelectionItem);
        }

        public override int GetHashCode()
        {
            return _hierarchy.GetHashCode() ^ _id.GetHashCode();
        }

        #endregion
    }
}
