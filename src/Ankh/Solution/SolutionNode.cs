using System;
using NSvn;
using EnvDTE;
using NSvn.Core;

namespace Ankh.Solution
{
    /// <summary>
    /// A node representing a solution.
    /// </summary>
    internal class SolutionNode : TreeNode
    {
        public SolutionNode( UIHierarchyItem item, IntPtr hItem, Explorer explorer )
            : base( item, hItem, explorer, null )
        {
            EnvDTE.Solution solution = explorer.DTE.Solution;
            this.solutionFile = SvnResource.FromLocalPath( solution.FullName );
            this.solutionFile.Context = explorer.Context;

            explorer.SetSolution( this );
        }

        public override void VisitResources( ILocalResourceVisitor visitor )
        {
            this.solutionFile.Accept( visitor );
            this.VisitChildren( visitor );
        } 

        protected override StatusKind GetStatus()
        {
            if ( this.solutionFile == null )
                return StatusKind.None;               
            else
            {
                StatusKind status = StatusFromResource( this.solutionFile );
                if ( status != StatusKind.Normal )
                {
                    // check the status on the projects
                    ModifiedVisitor v = new ModifiedVisitor();
                    this.VisitChildren( v );
                    if ( v.Modified )
                        status = StatusKind.Modified;
                }

                return status;
            }
        }

        private ILocalResource solutionFile;
    }
}
