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
    public class SolutionNode : SolutionExplorerTreeNode
    {
        public SolutionNode( UIHierarchyItem item, IntPtr hItem, Explorer explorer )
            : base( item, hItem, explorer, null )
        {
            EnvDTE.Solution solution = this.Explorer.DTE.Solution;
            this.solutionFile = this.Explorer.Context.StatusCache[solution.FullName];
            this.parser = new ParsedSolution( solution.FullName, explorer.Context );

            this.solutionFolder = this.Explorer.Context.StatusCache[
                Path.GetDirectoryName( solution.FullName )];

            EventHandler del  = new EventHandler( this.ChildOrResourceChanged );
            this.solutionFile.Changed += del;
            this.solutionFolder.Changed += del;

            this.deletedResources = new ArrayList();
            this.AddDeletions( this.solutionFolder.Path, this.deletedResources, new EventHandler(this.DeletedItemStatusChanged) );

            explorer.SetSolution( this );
                
            this.FindChildren();  
        }

        public override SolutionNode Solution
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this; }
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
            foreach( SvnItem item in this.deletedResources )
            {
                if ( filter == null || filter( item ) )
                    list.Add( item );
            }


            this.GetChildResources(list, getChildItems, filter );
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
                return MergeStatuses(
                    MergeStatuses( this.solutionFolder, this.solutionFile),
                        MergeStatuses(this.deletedResources));
            }
        }

        protected override bool RemoveTreeNodeIfResourcesDeleted()
        {
            // You can't delete a solution from VS.
            return false;
        }

        protected override void UnhookEvents()
        {
            UnhookEvents( new SvnItem[] { this.solutionFile, this.solutionFolder }, new EventHandler( this.ChildOrResourceChanged ) );
            UnhookEvents( this.deletedResources, new EventHandler( this.ChildOrResourceChanged ) );
        }

        /// <summary>
        /// The path to the solution folder.
        /// </summary>
        public override string Directory
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get{ return this.solutionFolder.Path; }
        }

		public ParsedSolution Parser
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get{ return this.parser; }
		}

        protected override IList DeletedItems
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return this.deletedResources; }
        }

		private ParsedSolution parser;
        private SvnItem solutionFile;
        private SvnItem solutionFolder;
        private ArrayList deletedResources;
    }
}
