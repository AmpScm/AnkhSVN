using System;
using System.ComponentModel;
using Ankh.Scc;

namespace Ankh.WpfUI.Controls
{
    public class PendingChangeItem : INotifyPropertyChanged
    {
        readonly PendingChange _pc;
        bool _isChecked;

        public PendingChangeItem(PendingChange pc)
        {
            _pc = pc;
            _isChecked = true;
        }

        public string FullPath
        {
            get { return _pc.FullPath; }
        }

        public string Project
        {
            get { return _pc.Project; }
        }

        public string Path
        {
            get { return FullPath; }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; RaisePropertyChange("IsChecked"); }
        }

        public PendingChange PendingChange
        {
            get { return _pc; }
        }

        public string ChangeText
        {
            get { return _pc.ChangeText; }
        }

        private void RaisePropertyChange(string propertyName)
        {
            if (_propChanged != null)
                _propChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return FullPath;
        }

        PropertyChangedEventHandler _propChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propChanged += value; }
            remove { _propChanged -= value; }
        }
    }
}
