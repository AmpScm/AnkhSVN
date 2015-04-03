using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
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

            public PendingChangeControlWrapper(Control control, PendingChangesUserControl puc)
            {
                _puc = puc;
                control.Disposed += OnControlDisposed;
                _control = control;
            }

            private void OnControlDisposed(object sender, EventArgs e)
            {
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
                get { return false; }
            }

            IEnumerable<Scc.PendingChange> IPendingChangeSource.PendingChanges
            {
                get { return new Scc.PendingChange[0]; }
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
            ReadOnlyObservableWrapper<Scc.PendingChange> _observeWrapper;

            ReadOnlyKeyedCollectionWithNotify<string, Scc.PendingChange> IPendingChangeControl.PendingChanges
            {
                get { return _list; }
                set
                {
                    if (_list == value)
                        return;

                    if (_observeWrapper != null)
                        _observeWrapper.Dispose();
                    _list = value;
                    _observeWrapper = new ReadOnlyObservableWrapper<Scc.PendingChange>(value);
                    _puc.PendingChangesList.ItemsSource = _observeWrapper;
                }
            }
        }
    }
}
