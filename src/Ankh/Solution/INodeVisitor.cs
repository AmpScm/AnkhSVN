// $Id$
using System;

namespace Ankh.Solution
{
    /// <summary>
    /// Summary description for INodeVisitor.
    /// </summary>
    internal interface INodeVisitor
    {
        void VisitProject( ProjectNode node );
        void VisitProjectItem( ProjectItemNode node );
        void VisitSolutionNode( SolutionNode node );		
    }
}
