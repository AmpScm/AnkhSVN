using System;
using System.Collections;
using Microsoft.VisualStudio.Shell.Interop;


namespace Ankh.Solution
{
    public sealed class SolutionFolderNode : SolutionExplorerTreeNode
    {
        public SolutionFolderNode(VSITEMSELECTION selection, Explorer explorer, SolutionExplorerTreeNode parent, EnvDTE.Project project)
            : base(selection, explorer, parent)
        {
            this.FindChildren();
        }

        public override string Directory
        {
            get { return null; }
        }


        public override void Refresh(bool rescan)
        {
            foreach (SolutionExplorerTreeNode child in this.Children)
                child.Refresh(rescan);

            base.Refresh(rescan);
        }

        public override void Accept( INodeVisitor visitor )
        {
            visitor.VisitSolutionFolder( this );            
        }

        public override void GetResources( System.Collections.IList list, bool getChildItems, ResourceFilterCallback filter )
        {
            this.GetChildResources( list, getChildItems, filter );

        }

        protected override bool RemoveTreeNodeIfResourcesDeleted()
        {
            // It's not our business to remove solution folders, even if they're empty.
            return false;
        }

        protected override IList DeletedItems
        {
            get { throw new Exception( "The method or operation is not implemented." ); }
        }

        protected override void UnhookEvents()
        {
            // nothing
        }


        public override IVsHierarchy Hierarchy
        {
            get { throw new Exception( "The method or operation is not implemented." ); }
        }
    }
}
