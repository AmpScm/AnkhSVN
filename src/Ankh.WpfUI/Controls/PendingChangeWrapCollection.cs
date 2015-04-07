using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Ankh.Collections;
using Ankh.Scc;
using Ankh.UI;

namespace Ankh.WpfUI.Controls
{
    class PendingChangeWrapCollection : KeyedWrapNotifyCollection<string, PendingChange, PendingChangeItem>
    {
        IAnkhServiceProvider _context;

        public PendingChangeWrapCollection(IAnkhServiceProvider context, IKeyedNotifyCollection<string, PendingChange> pendingChanges)
            : base(pendingChanges, context)
        {

        }

        public new IAnkhServiceProvider Context
        {
            get { return _context ?? (_context = base.Context as IAnkhServiceProvider);  }
        }

        protected override string GetKeyForItem(PendingChangeItem item)
        {
            return item.FullPath;
        }

        protected override PendingChangeItem GetWrapItem(PendingChange inner)
        {
            return new PendingChangeItem(inner);
        }
    }
}
