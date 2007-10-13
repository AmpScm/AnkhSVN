using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Solution
{
    class HierarchyItem
    {
        public HierarchyItem( uint itemID )
        {
            this.itemID = itemID;
        }

        public uint ItemID
        {
            get { return itemID; }
        }

        private uint itemID;

    }


    class HierarchyProjectItem : HierarchyItem
    {
        public HierarchyProjectItem( uint itemID, ProjectItem item )
            : base( itemID )
        {
            this.projectItem = item;
        }

        public ProjectItem ProjectItem
        {
            get { return projectItem; }
            set { projectItem = value; }
        }

        private ProjectItem projectItem;
        
    }

    class HierarchyProject : HierarchyItem
    {
        public HierarchyProject( uint itemID, IVsHierarchy hierarchy, Project project )
            : base( itemID )
        {
            this.hierarchy = hierarchy;
            this.project = project;
        }

        public Project Project
        {
            get { return project; }
        }

        public IVsHierarchy Hierarchy
        {
            get { return this.hierarchy; }
        }


        private IVsHierarchy hierarchy;
        private Project project;


    }
}
