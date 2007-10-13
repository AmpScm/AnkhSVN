using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Solution
{
    interface IHierarchyVisitor
    {
        void VisitProject(HierarchyProject project);
        void VisitProjectItem(HierarchyItem parent, HierarchyProjectItem projectItem);
    }
}
