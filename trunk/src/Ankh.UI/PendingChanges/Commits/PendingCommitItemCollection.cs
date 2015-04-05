using System;
using System.Collections.Generic;
using Ankh.Scc;

namespace Ankh.UI.PendingChanges.Commits
{
    class PendingCommitItemCollection : KeyedWrapNotifyCollection<string, PendingChange, PendingCommitItem>
    {
        PendingCommitsView _view;
        public PendingCommitItemCollection(IAnkhServiceProvider context, ReadOnlyKeyedNotifyCollection<string, PendingChange> collection)
            : base(collection, context)
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
