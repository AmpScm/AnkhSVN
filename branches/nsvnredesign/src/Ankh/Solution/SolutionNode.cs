// $Id$
using System;
using System.IO;

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
            EnvDTE.Solution solution = this.Explorer.DTE.Solution;
            this.solutionFile = this.Explorer.StatusCache[solution.FullName];

            this.solutionFolder = this.Explorer.StatusCache[
                Path.GetDirectoryName( solution.FullName )];

            this.solutionFolder.Changed += new StatusChanged( this.ChildOrResourceChanged );

            explorer.SetSolution( this );

            this.FindChildren();
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
