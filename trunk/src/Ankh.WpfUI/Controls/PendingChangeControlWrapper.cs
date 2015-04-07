using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Ankh.Collections;
using Ankh.Commands;
using Ankh.Scc;
using Ankh.UI;
using Point = System.Drawing.Point;

namespace Ankh.WpfUI.Controls
{
    sealed class PendingChangeControlWrapper : AnkhService, IPendingChangeControl, IDisposable, IPendingChangeUI, INotifyPropertyChanged
    {
        Control _control;
        PendingChangesUserControl _puc;
        PendingChangeWrapCollection _wc;

        public PendingChangeControlWrapper(IAnkhServiceProvider context, Control control, PendingChangesUserControl puc)
            : base(context)
        {
            _puc = puc;
            _puc.Context = context;
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

        bool _headerChecked;
        public bool IsHeaderChecked
        {
            get { return _headerChecked; }
            set { _headerChecked = value; RaisePropertyChanged("IsHeaderChecked"); }
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
                    _puc.PendingChangesList.DataContext = this;
                    _puc.DataContext = this;
                }
                else
                {
                    _wc = null;
                    _puc.PendingChangesList.ItemsSource = null;
                }
            }
        }

        public void OnChange(string fullPath)
        {
            PendingChangeItem pci;
            if (_wc != null && _wc.TryGetValue(fullPath, out pci))
                pci.InvokePropertyChange(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
