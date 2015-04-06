using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.Collections;
using Ankh.Scc;

namespace Ankh.UI
{
    public interface IPendingChangeUI
    {
        bool HasCheckedItems { get; }
        IEnumerable<PendingChange> CheckedItems { get; }

        void OnChange(string fullPath);

        IKeyedNotifyCollection<string, PendingChange> Items { get; set; }
    }

    public interface IPendingChangeControl
    {
        Control Control { get; }

        IPendingChangeUI UI { get; }

    }

    public interface IPendingChangeControlFactory
    {
        IPendingChangeControl Create(IAnkhServiceProvider context, IContainer container);
    }
}
