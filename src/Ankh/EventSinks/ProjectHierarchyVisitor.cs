using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Solution;
using System.Collections;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.EventSinks
{
    class ProjectHierarchyVisitor : IHierarchyVisitor
    {
        public ProjectHierarchyVisitor(IContext context)
        {
            this.items = new Hashtable();
            this.context = context;
        }

        public void VisitProject( HierarchyProject project )
        {
            VSITEMSELECTION sel = new VSITEMSELECTION();
            sel.itemid = project.ItemID;
            sel.pHier = project.Hierarchy;

            this.project = new ProjectNode(sel, (Explorer)context.SolutionExplorer, 
                null,
                context.DTE.Solution.Projects.Item(1) );
            this.items.Add( project, this.project );
        }

        public void VisitProjectItem( HierarchyItem parent, HierarchyProjectItem projectItem )
        {
            SolutionExplorerTreeNode parentNode = this.items[ parent ] as SolutionExplorerTreeNode;

            if ( parentNode != null )
            {
                VSITEMSELECTION sel = new VSITEMSELECTION();
                sel.itemid = projectItem.ItemID;
                sel.pHier = parentNode.Hierarchy;

                ProjectItemNode node = new ProjectItemNode( sel, projectItem.ItemID,
                    (Explorer)this.context.SolutionExplorer, parentNode, projectItem.ProjectItem, null );
                parentNode.AddChild( node );
                this.items.Add(projectItem, node);

            }
        }

        public void Refresh()
        {
            this.project.Refresh( false );
        }

        private Hashtable items;
        private ProjectNode project;
        private IContext context;
    }
}
