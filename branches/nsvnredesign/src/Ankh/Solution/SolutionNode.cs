// $Id$
using System;
using System.IO;

using EnvDTE;
using NSvn.Core;
using System.Collections;

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
            EnvDTE.Solution solution = this.Explorer.DTE.Solution;
            this.solutionFile = this.Explorer.StatusCache[solution.FullName];

            this.solutionFolder = this.Explorer.StatusCache[
                Path.GetDirectoryName( solution.FullName )];

            StatusChanged del  = new StatusChanged( this.ChildOrResourceChanged );
            this.solutionFile.Changed += del;
            this.solutionFolder.Changed += del;

            explorer.SetSolution( this );

            this.FindChildren();
        }   
    
        public override void GetResources( IList list, 
            bool getChildItems, ResourceFilterCallback filter )
        {
            if ( filter == null || filter( this.solutionFolder ) )
                list.Add( this.solutionFolder );

            if ( filter == null || filter( this.solutionFile ) )
                list.Add( this.solutionFile );

            this.GetChildResources(list, getChildItems, filter );
        }

        
        

        /// <summary>
        /// The path to the solution folder.
        /// </summary>
        protected override string Directory
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
                return this.MergeStatuses( this.solutionFolder, this.solutionFile );
            }
        }

        private SvnItem solutionFile;
        private SvnItem solutionFolder;
    }
}
