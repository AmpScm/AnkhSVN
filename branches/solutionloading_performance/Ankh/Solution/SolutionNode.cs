// $Id$
using System;
using System.IO;

using EnvDTE;
using NSvn.Core;
using System.Collections;
using System.Diagnostics;

namespace Ankh.Solution
{
    /// <summary>
    /// A node representing a solution.
    /// </summary>
    public class SolutionNode : TreeNode
    {
        public SolutionNode( UIHierarchyItem item, IntPtr hItem, Explorer explorer )
            : base( item, hItem, explorer, null )
        {
            EnvDTE.Solution solution = this.Explorer.DTE.Solution;
            this.solutionFile = this.Explorer.Context.StatusCache[solution.FullName];

            this.solutionFolder = this.Explorer.Context.StatusCache[
                Path.GetDirectoryName( solution.FullName )];

            this.solutionFile.Node = this;
            this.solutionFolder.Node = this;

            StatusChanged del  = new StatusChanged( this.ChildOrResourceChanged );
            this.solutionFile.Changed += del;
            this.solutionFolder.Changed += del;

            this.additionalResources = new ArrayList();
            this.AddDeletions( this.solutionFolder.Path, this.additionalResources, del );

            explorer.SetSolution( this );
        }   

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitSolutionNode(this);
        }

    
        public override void GetResources( IList list, 
            bool getChildItems, ResourceFilterCallback filter )
        {
            if ( filter == null || filter( this.solutionFolder ) )
                list.Add( this.solutionFolder );

            if ( filter == null || filter( this.solutionFile ) )
                list.Add( this.solutionFile );

            // add deleted items.
            foreach( SvnItem item in this.additionalResources )
            {
                if ( filter == null || filter( item ) )
                    list.Add( item );
            }

            this.GetChildResources(list, getChildItems, filter );
        }

        public override void InitializeStatus()
        {
            this.Explorer.Context.StatusCache.Status( solutionFolder.Path );
            Refresh( false );
        }
        

        /// <summary>
        /// The path to the solution folder.
        /// </summary>
        public override string Directory
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get{ return this.solutionFolder.Path; }
        }



        /// <summary>
        /// Get the status for this node, not including children.
        /// </summary>
        /// <returns></returns>
        protected override NodeStatus ThisNodeStatus()
        {
            if ( this.solutionFile == null )
                return NodeStatus.None;               
            else
            {
                return this.MergeStatuses(
                    this.MergeStatuses( this.solutionFolder, this.solutionFile ),
                    this.MergeStatuses( this.additionalResources ) );
            }
        }

        private SvnItem solutionFile;
        private SvnItem solutionFolder;
        private ArrayList additionalResources;
    }
}
