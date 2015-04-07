using System;
using System.Collections.Generic;
using Ankh.Collections;
using Ankh.Scc;

namespace Ankh.UI.PendingChanges.Commits
{
    class PendingCommitItemCollection : KeyedWrapNotifyCollection<string, PendingChange, PendingCommitItem>
    {
        PendingCommitsView _view;
        public PendingCommitItemCollection(PendingCommitsView view, IKeyedNotifyCollection<string, PendingChange> collection)
            : base(collection, view)
        {

        }

        protected PendingCommitsView View
        {
            get { return _view ?? (_view = (PendingCommitsView)Context); }
        }

        protected override string GetKeyForItem(PendingCommitItem item)
        {
            return item.FullPath;
        }

        protected override PendingCommitItem GetWrapItem(PendingChange inner)
        {
            return new PendingCommitItem(View, inner);
        }
    }
}
