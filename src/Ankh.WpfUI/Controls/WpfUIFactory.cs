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
    [GlobalService(typeof(IPendingChangeControlFactory))]
    class WpfUiFactory : AnkhService, IPendingChangeControlFactory
    {
        public WpfUiFactory(IAnkhServiceProvider context)
            : base(context)
        {

        }
        public IPendingChangeControl Create(IAnkhServiceProvider context, IContainer container)
        {
            ElementHost host = new ElementHost();

            PendingChangesUserControl puc = new PendingChangesUserControl();
            puc.Context = context;
            host.Child = puc;

            return new PendingChangeControlWrapper(this, host, puc);
        }

        sealed class PendingChangeControlWrapper : AnkhService, IPendingChangeControl, IDisposable,  IPendingChangeUI
        {
            Control _control;
            PendingChangesUserControl _puc;
            PendingChangeWrapCollection _wc;

            public PendingChangeControlWrapper(IAnkhServiceProvider context, Control control, PendingChangesUserControl puc)
                : base(context)
            {
                _puc = puc;
                control.Disposed += OnControlDisposed;
                _control = control;
            }

            private void OnControlDisposed(object sender, EventArgs e)
            {
                if (_wc != null)
                {
                    _wc.Dispose();
                    _wc = null;
                }
                Dispose(true);
            }

            protected override void Dispose(bool disposing)
            {
                try
                {
                    _control.Disposed -= OnControlDisposed;
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }

            public Control Control
            {
                get { return _control; }
            }

            bool IPendingChangeUI.HasCheckedItems
            {
                get
                {
                    foreach (PendingChangeItem pci in _puc.PendingChangesList.Items)
                    {
                        if (pci.IsChecked)
                            return true;
                    }
                    return false;
                }
            }

            public IEnumerable<Scc.PendingChange> CheckedItems
            {
                get
                {
                    foreach (PendingChangeItem pci in _puc.PendingChangesList.Items)
                    {
                        if (pci.IsChecked)
                            yield return pci.PendingChange;
                    }
                }
            }

            public void Dispose()
            {
                Dispose(true);
            }

            public IPendingChangeUI UI
            {
                get { return this; }
            }

            IKeyedNotifyCollection<string, Scc.PendingChange> _list;

            public IKeyedNotifyCollection<string, Scc.PendingChange> Items
            {
                get { return _list; }
                set
                {
                    if (_list == value)
                        return;

                    if (_wc != null)
                        _wc.Dispose();

                    _list = value;
                    if (value != null)
                    {
                        _wc = new PendingChangeWrapCollection(this, value);
                        _puc.PendingChangesList.ItemsSource = new ReadOnlyObservableWrapper<PendingChangeItem>(_wc);
                    }
                    else
                    {
                        _wc = null;
                        _puc.PendingChangesList.ItemsSource = null;
                    }
                }
            }

            class PendingChangeWrapCollection : KeyedWrapNotifyCollection<string, PendingChange, PendingChangeItem>
            {
                public PendingChangeWrapCollection(IAnkhServiceProvider context, IKeyedNotifyCollection<string, PendingChange> pendingChanges)
                    : base(pendingChanges, context)
                {

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

            public void OnChange(string fullPath)
            {
                PendingChangeItem pci;
                if (_wc != null && _wc.TryGetValue(fullPath, out pci))
                    pci.InvokePropertyChange(null);
            }
        }
    }
}
