using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Selection
{
    public class SvnHierarchy : IEquatable<SvnHierarchy>
    {
        readonly IVsHierarchy _hierarchy;

        [CLSCompliant(false)]
        public SvnHierarchy(IVsHierarchy hierarchy)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            _hierarchy = hierarchy;
        }

        const uint VSITEMID_ROOT = 4294967294;

        string _name;
        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(_name))
                    return _name.Length > 0 ? _name : null;

                IVsProject2 vsProject = _hierarchy as IVsProject2;

                if (vsProject != null)
                {
                    if (VSErr.Succeeded(vsProject.GetMkDocument(VSITEMID_ROOT, out _name)))
                        return _name;
                }

                if (VSErr.Succeeded(_hierarchy.GetCanonicalName(VSITEMID_ROOT, out _name)))
                    return _name;

                _name = "";
                return null;
            }
        }

        [CLSCompliant(false)]
        public IVsHierarchy Hierarchy
        {
            get { return _hierarchy; }
        }

        public bool Equals(SvnHierarchy other)
        {
            if (other == null)
                return false;

            if (Hierarchy == other.Hierarchy)
                return true;
            if (Name == other.Name)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
        }
    }
}
