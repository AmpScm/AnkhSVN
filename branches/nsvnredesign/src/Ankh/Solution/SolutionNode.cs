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
        protected override StatusKind NodeStatus()
        {
            if ( this.solutionFile == null )
                return StatusKind.None;               
            else
            {
                StatusKind fileStatus = this.GenerateStatus(this.solutionFile.Status);
                StatusKind folderStatus = this.GenerateStatus(this.solutionFolder.Status);
                
                return fileStatus == StatusKind.Normal ? folderStatus : fileStatus;
            }
        }

        private SvnItem solutionFile;
        private SvnItem solutionFolder;
    }
}
