using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.Scc;

namespace Ankh.UI
{
    public interface IPendingChangeSource
    {
        bool HasPendingChanges { get; }
        IEnumerable<PendingChange> PendingChanges { get; }
    }

    public interface IPendingChangeControl
    {
        Control Control { get; }

        IPendingChangeSource PendingChangeSource { get; }

        ReadOnlyKeyedCollectionWithNotify<string, PendingChange> PendingChanges
        {
            get;
            set;
        }
    }

    public interface IPendingChangeControlFactory
    {
        IPendingChangeControl Create(IAnkhServiceProvider context, IContainer container);
    }
}
