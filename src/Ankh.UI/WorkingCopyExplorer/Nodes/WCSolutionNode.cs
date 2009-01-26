using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VS;

namespace Ankh.UI.WorkingCopyExplorer.Nodes
{
    class WCSolutionNode : WCTreeNode
    {
        readonly int _imageIndex;
        public WCSolutionNode(IAnkhServiceProvider context)
            : base(context, null)
        {
            _imageIndex = context.GetService<IFileIconMapper>().GetIconForExtension(".sln");
        }

        public override void GetResources(System.Collections.ObjectModel.Collection<SvnItem> list, bool getChildItems, Predicate<SvnItem> filter)
        {
//            throw new NotImplementedException();
        }

        public override void Refresh(bool rescan)
        {
//            throw new NotImplementedException();
        }

        public override int ImageIndex
        {
            get { return _imageIndex; }
        }
    }
}
