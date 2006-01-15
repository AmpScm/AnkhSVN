using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Solution
{
    public sealed class SolutionFolderItem : ProjectNode
    {
        public SolutionFolderItem(EnvDTE.UIHierarchyItem item, IntPtr hItem, Explorer explorer, TreeNode parent, EnvDTE.Project project)
            : base(item, hItem, explorer, parent, project)
        {
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

    }
}
