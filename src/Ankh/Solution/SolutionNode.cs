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
            this.solutionFile = (WorkingCopyFile)SvnResource.FromLocalPath( solution.FullName );
            this.solutionFile.Context = explorer.Context;

            explorer.SetSolution( this );
        }

        
        public WorkingCopyFile SolutionFile
        {
            get{ return this.solutionFile; }
        }


        public override void VisitResources( ILocalResourceVisitor visitor, bool recursive )
        {
            this.solutionFile.Accept( visitor );
            if ( recursive )
                this.VisitChildResources( visitor);
        } 


        /// <summary>
        /// Accept an INodeVisitor.
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept( INodeVisitor visitor )
        {
            visitor.VisitSolutionNode( this );
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
                    this.VisitChildResources( v );
                    if ( v.Modified )
                        status = StatusKind.Modified;
                }

                return status;
            }
        }

        private WorkingCopyFile solutionFile;
    }
}
