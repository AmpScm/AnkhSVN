// $Id$
using System;

namespace Ankh.Solution
{
    /// <summary>
    /// Summary description for INodeVisitor.
    /// </summary>
    public interface INodeVisitor
    {
        void VisitProject( ProjectNode node );
        void VisitProjectItem( ProjectItemNode node );
        void VisitSolutionNode( SolutionNode node );
        void VisitSolutionFolder( SolutionFolderNode node );
    }
}
