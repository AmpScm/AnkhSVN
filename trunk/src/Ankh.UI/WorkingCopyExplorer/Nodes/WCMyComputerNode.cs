using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VS;

namespace Ankh.UI.WorkingCopyExplorer.Nodes
{
    class WCMyComputerNode : WCTreeNode
    {
        readonly int _imageIndex;
        public WCMyComputerNode(IAnkhServiceProvider context)
            : base(context, null)
        {
            _imageIndex = context.GetService<IFileIconMapper>().GetSpecialFolderIcon(WindowsSpecialFolder.MyComputer);
        }

        public override void GetResources(System.Collections.ObjectModel.Collection<SvnItem> list, bool getChildItems, Predicate<SvnItem> filter)
        {
        }

        public override void Refresh(bool rescan)
        {
        }

        public override int ImageIndex
        {
            get { return _imageIndex; }
        }
    }
}
