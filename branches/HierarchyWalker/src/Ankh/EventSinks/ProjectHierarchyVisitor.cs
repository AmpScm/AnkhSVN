using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Solution;
using System.Collections;

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
            this.project = new ProjectNode(project.ItemID, (Explorer)context.SolutionExplorer, 
                null,
                context.DTE.Solution.Projects.Item(1), project.Hierarchy );
            this.items.Add( project, this.project );
        }

        public void VisitProjectItem( HierarchyItem parent, HierarchyProjectItem projectItem )
        {
            SolutionExplorerTreeNode parentNode = this.items[ parent ] as SolutionExplorerTreeNode;

            if ( parentNode != null )
            {
                ProjectItemNode node = new ProjectItemNode( projectItem.ProjectItem, projectItem.ItemID,
                    (Explorer)this.context.SolutionExplorer, parentNode, null );
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
