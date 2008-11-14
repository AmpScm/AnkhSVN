using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc;

namespace Ankh.UI.PendingChanges.Synchronize
{
    sealed class SynchronizeItem : AnkhPropertyGridItem
    {
        readonly IAnkhServiceProvider _context;
        readonly SynchronizeListItem _listItem;
        public SynchronizeItem(IAnkhServiceProvider context, SynchronizeListItem listItem)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (listItem == null)
                throw new ArgumentNullException("listItem");

            _context = context;
            _listItem = listItem;
        }

        protected override string ClassName
        {
            get { return "Recent Change"; }
        }

        protected override string ComponentName
        {
            get { return _listItem.SvnItem.Name; }
        }

        internal SynchronizeListItem ListItem
        {
            get { return _listItem; }
        }
    }
}
