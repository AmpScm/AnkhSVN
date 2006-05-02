using System;
using System.Collections;


namespace Ankh.Solution
{
    public sealed class SolutionFolderNode : TreeNode
    {
        public SolutionFolderNode(EnvDTE.UIHierarchyItem item, IntPtr hItem, Explorer explorer, TreeNode parent, EnvDTE.Project project)
            : base(item, hItem, explorer, parent)
        {
            this.Explorer.AddResource( project, this );
            this.FindChildren();
        }

        public override string Directory
        {
            get { return null; }
        }


        public override void Refresh(bool rescan)
        {
            foreach (TreeNode child in this.Children)
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

        protected override void CheckForSvnDeletions()
        {
            // nothing, solution folders are virtual only.
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

    }
}
