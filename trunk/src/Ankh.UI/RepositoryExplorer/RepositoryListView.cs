using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;

namespace Ankh.UI.RepositoryExplorer
{
    class RepositoryListView : ListViewWithSelection<RepositoryListItem>
    {
        IAnkhServiceProvider _context;

        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set 
            { 
                _context = value;
                if (value != null)
                    OnContextChanged(EventArgs.Empty);
            }
        }

        private void OnContextChanged(EventArgs eventArgs)
        {
            IFileIconMapper fim = Context.GetService<IFileIconMapper>();

            if (fim != null)
                SmallImageList = fim.ImageList;
        }

        protected override void OnResolveItem(ResolveItemEventArgs e)
        {
            e.Item = ((RepositoryExplorerItem)e.SelectionItem).ListViewItem;
            base.OnResolveItem(e);
        }

        protected override void OnRetrieveSelection(RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new RepositoryExplorerItem(e.Item);
            base.OnRetrieveSelection(e);
        }    }
}
