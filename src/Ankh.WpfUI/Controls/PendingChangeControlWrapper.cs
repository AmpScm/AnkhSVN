using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Ankh.Collections;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.UI;
using WFControl = System.Windows.Forms.Control;

namespace Ankh.WpfUI.Controls
{
    sealed class PendingChangeControlWrapper : AnkhService, IPendingChangeControl, IDisposable, IPendingChangeUI, INotifyPropertyChanged, ISelectionMapOwner<PendingChangeItem>
    {
        WFControl _control;
        PendingChangesUserControl _puc;
        PendingChangeWrapCollection _wc;
        SelectionItemMap _sim;

        public PendingChangeControlWrapper(IAnkhServiceProvider context, WFControl control, PendingChangesUserControl puc)
            : base(context)
        {
            _puc = puc;
            //_puc.Context = context; // Done by cakker
            _puc.DataContext = this;

            control.Disposed += OnControlDisposed;
            _control = control;

            _sim = SelectionItemMap.Create(this);
            _sim.PublishHierarchy = true;
            _sim.Context = context;

            // Set Notify that we have a selection, otherwise the first selection request fails.
            _sim.NotifySelectionUpdated();
            puc.PendingChangesList.SelectionChanged += PendingChangesList_SelectionChanged;
        }

        void PendingChangesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _sim.NotifySelectionUpdated();
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

                if (_handleDestroyed != null)
                    _handleDestroyed(this, EventArgs.Empty);
            }
            finally
            {
                _handleDestroyed = null;
                base.Dispose(disposing);
            }
        }

        public WFControl Control
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

        bool? _headerChecked = true;
        public bool? IsHeaderChecked
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

        #region Selection owner
        System.Collections.IList ISelectionMapOwner<PendingChangeItem>.Selection
        {
            get { return _puc.PendingChangesList.SelectedItems; }
        }

        System.Collections.IList ISelectionMapOwner<PendingChangeItem>.AllItems
        {
            get { return _puc.PendingChangesList.SelectedItems; }
        }

        bool ISelectionMapOwner<PendingChangeItem>.SelectionContains(PendingChangeItem item)
        {
            return false;
        }

        IntPtr ISelectionMapOwner<PendingChangeItem>.GetImageList()
        {
            return IntPtr.Zero;
        }

        int ISelectionMapOwner<PendingChangeItem>.GetImageListIndex(PendingChangeItem item)
        {
            return -1;
        }

        string ISelectionMapOwner<PendingChangeItem>.GetText(PendingChangeItem item)
        {
            return item.FullPath;
        }

        object ISelectionMapOwner<PendingChangeItem>.GetSelectionObject(PendingChangeItem item)
        {
            return item.PendingChange;
        }

        PendingChangeItem ISelectionMapOwner<PendingChangeItem>.GetItemFromSelectionObject(object item)
        {
            PendingChange pc = (PendingChange)item;
            PendingChangeItem pci = null;

            if (pc != null && _wc != null)
                _wc.TryGetValue(pc.FullPath, out pci);

            return pci;
        }

        delegate bool SetSelectedItems(System.Collections.IEnumerable items);
        SetSelectedItems _setSelectedItems;
        void ISelectionMapOwner<PendingChangeItem>.SetSelection(PendingChangeItem[] items)
        {
            if (_setSelectedItems == null)
            {
                MethodInfo setInfo = typeof(System.Windows.Controls.ListBox).GetMethod("SetSelectedItems", BindingFlags.NonPublic | BindingFlags.Instance);
                _setSelectedItems = (SetSelectedItems)Delegate.CreateDelegate(typeof(SetSelectedItems), _puc.PendingChangesList, setInfo);
            }

            _setSelectedItems(items);
        }

        EventHandler _handleDestroyed;
        event EventHandler ISelectionMapOwner<PendingChangeItem>.HandleDestroyed
        {
            add { _handleDestroyed += value; }
            remove { _handleDestroyed -= value; }
        }

        string ISelectionMapOwner<PendingChangeItem>.GetCanonicalName(PendingChangeItem item)
        {
            return item.FullPath;
        }
#endregion
    }
}
