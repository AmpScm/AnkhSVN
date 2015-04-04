using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
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

            return new PendingChangeControlWrapper(host, puc);
        }

        sealed class PendingChangeControlWrapper : IPendingChangeControl, IDisposable,  IPendingChangeSource
        {
            Control _control;
            PendingChangesUserControl _puc;
            PendingChangeWrapCollection _wc;

            public PendingChangeControlWrapper(Control control, PendingChangesUserControl puc)
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

            void Dispose(bool disposing)
            {
                _control.Disposed -= OnControlDisposed;
            }

            public Control Control
            {
                get { return _control; }
            }

            bool IPendingChangeSource.HasPendingChanges
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

            IEnumerable<Scc.PendingChange> IPendingChangeSource.PendingChanges
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

            public IPendingChangeSource PendingChangeSource
            {
                get { return this; }
            }

            ReadOnlyKeyedCollectionWithNotify<string, Scc.PendingChange> _list;

            ReadOnlyKeyedCollectionWithNotify<string, Scc.PendingChange> IPendingChangeControl.PendingChanges
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
                        _wc = new PendingChangeWrapCollection(value);
                        _puc.PendingChangesList.ItemsSource = new ReadOnlyObservableWrapper<PendingChangeItem>(_wc);
                    }
                    else
                    {
                        _wc = null;
                        _puc.PendingChangesList.ItemsSource = null;
                    }
                }
            }

            class PendingChangeWrapCollection : KeyedWrapCollectionWithNotify<PendingChange, string, PendingChangeItem>
            {
                public PendingChangeWrapCollection(ReadOnlyKeyedCollectionWithNotify<string, PendingChange> pendingChanges)
                    : base(pendingChanges)
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
        }
    }
}
