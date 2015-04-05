using System;
using System.Collections.Generic;
using Ankh.Scc;

namespace Ankh.UI.PendingChanges.Commits
{
    class PendingCommitItemCollection : KeyedWrapCollectionWithNotify<PendingChange, string, PendingCommitItem>
    {
        PendingCommitsView _view;
        public PendingCommitItemCollection(IAnkhServiceProvider context, ReadOnlyKeyedCollectionWithNotify<string, PendingChange> collection)
            : base(context, collection)
        {

        }

        protected override string GetKeyForItem(PendingCommitItem item)
        {
            return item.FullPath;
        }

        protected override PendingCommitItem GetWrapItem(PendingChange inner)
        {
            if (_view == null)
                _view = Context.GetService<PendingCommitsPage>().PendingCommitsView;

            return new PendingCommitItem(_view, inner);
        }
    }
}
